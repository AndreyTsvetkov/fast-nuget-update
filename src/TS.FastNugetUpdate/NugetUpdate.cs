using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Functional.Maybe;
using JetBrains.Annotations;
using MoreLinq;

namespace TS.FastNugetUpdate
{
	public sealed class NugetUpdate : INugetUpdate
	{
		private readonly string _name;
		private readonly string _version;
		private readonly Action<string> _message;
		private readonly Action<string> _error;

		public NugetUpdate([NotNull]string name, [NotNull]string version, 
			[NotNull]Action<string> message, [NotNull]Action<string> error)
		{
			_name = name;
			_version = version;
			_message = message;
			_error = error;
		}

		public bool Apply(string root)
		{
			var fixNugetReference = FixNugetReference(_name, _version);
			var realAssemblyVersion = ExtractAssemblyVersion(root, _name, _version);
			return realAssemblyVersion
				.Select(assemblyVersion =>
				{
					var fixAssemblyReference = FixAssemblyReference(_name, _version, assemblyVersion);
					var csprojs = Directory.GetFiles(root, "*.csproj", SearchOption.AllDirectories);
					var packages = Directory.GetFiles(root, "packages.config", SearchOption.AllDirectories);
					var projects = (
						from csproj in csprojs
						let folder = Path.GetDirectoryName(csproj)
						let maybeConfig = packages.FirstMaybe(InSame(folder))
						where maybeConfig.HasValue
						let config = maybeConfig.Value
						select new {csproj, config}
					).ToArray();
					projects.ForEach(p =>
					{
						_message($"Processing {p.config}");
						Rewrite(p.config, fixNugetReference);
						_message($"Processing {p.csproj}");
						Rewrite(p.csproj, fixAssemblyReference);
					});
					return projects.Any();
				})
				.OrElse(() =>
				{
					_error($"Couldn't find the assembly for package {_name}.{_version} in {root}");
					return false;
				});
		}

		private Maybe<string> ExtractAssemblyVersion(string root, string name, string version) =>
			Directory
				.GetFiles(Path.Combine(root, "packages", $"{name}.{version}"), $"{name}.dll", 
					SearchOption.AllDirectories)
				.FirstMaybe()
				.Select(file =>
					Assembly
						.LoadFile(file)
						.GetName()
						.Version
						.ToString());

		[Pure]
		private static Func<string, string> FixAssemblyReference(string name, string packageVersion, string assemblyVersion)
		{
			var reference = new Regex(
				$@"<Reference Include=""{RegexEncode(name)}, Version=[^""]+"">");
			var hintPath = new Regex(
				$@"<HintPath>(?<packageRoot>[\.\w\\]*?packages)\\{RegexEncode(name)}\.\d+\.\d+\.\d+(\.\d+)?\\(?<restPath>[^<]+)</HintPath>");

			return content =>
			{
				var referenceProcessed = reference.Replace(content, $@"<Reference Include=""{name}, Version={assemblyVersion}"">");
				var hintPathProcessed = hintPath.Replace(referenceProcessed, m =>
					$@"<HintPath>{m.Groups["packageRoot"].Value}\{name}.{packageVersion}\{m.Groups["restPath"].Value}</HintPath>");
				return hintPathProcessed;
			};
		}

		[Pure]
		private static Func<string, string> FixNugetReference(string name, string version)
		{
			var regex = new Regex($@"package id=""{RegexEncode(name)}"" version=""[^""]+""");
			return content => regex.Replace(content, $@"package id=""{name}"" version=""{version}""");
		}

		private static string RegexEncode(string data) =>
			data
				.Replace(@"\", @"\\")
				.Replace(".", @"\.");

		private static void Rewrite(string file, Func<string, string> fix) =>
			File.WriteAllText(file, fix(File.ReadAllText(file, Encoding.UTF8)), Encoding.UTF8);

		private static Func<string, bool> InSame(string folder) => file =>
			Path.GetDirectoryName(file) == folder;
	}
} 