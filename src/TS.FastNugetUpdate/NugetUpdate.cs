using System;
using System.Linq;
using System.IO;
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
				Rewrite(p.csproj, FixAssemblyReference(_name, _version));
				Rewrite(p.config, FixNugetReference(_name, _version));
			});

			return projects.Any();
		}

		[Pure]
		private static Func<string, string> FixAssemblyReference(string name, string version)
		{
			var regex = new Regex($@"<Reference Include=""{RegexEncode(name)}, Version=[^""]+"">\s*" +
				$@"<HintPath>packages\\{RegexEncode(name)}\.\d+\.\d+\.\d+(\.\d+)?\\(?<restPath>[^<]+)</HintPath>\s*" + 
				$@"<Private>True</Private>\s*</Reference>");
			return content => regex.Replace(content,
				m => $@"					<Reference Include=""{name}, Version={version}"">
						<HintPath>packages\{name}.{version}\{m.Groups["restPath"].Value}</HintPath>
						<Private>True</Private>
					</Reference>"
			);
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