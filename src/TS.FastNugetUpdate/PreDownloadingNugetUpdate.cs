using System;
using System.IO;
using System.Linq;
using Functional.Maybe;
using Ionic.Zip;
using JetBrains.Annotations;
using MoreLinq;

namespace TS.FastNugetUpdate
{
	public sealed class PreDownloadingNugetUpdate : INugetUpdate
	{
		private readonly string[] _sources;
		private readonly string _name;
		private readonly string _version;
		private readonly Action<string> _message;
		private readonly Action<string> _error;
		private readonly INugetUpdate _other;

		public PreDownloadingNugetUpdate([NotNull]string sources, [NotNull]string name, [NotNull]string version,
			[NotNull]Action<string> message, [NotNull]Action<string> error, [NotNull]INugetUpdate other)
		{
			_sources = sources
				.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.ToArray();
			_name = name;
			_version = version;
			_message = message;
			_error = error;
			_other = other;
		}

		public bool Apply(string root)
		{
			var maybePackage = (
				from source in _sources
				let candidate = Path.Combine(source, _name, $"{_name}.{_version}.nupkg")
				where File.Exists(candidate)
				select candidate
			).FirstMaybe();

			return maybePackage
				.Select(package =>
				{
					var destination = Path.Combine(root, "packages", $"{_name}.{_version}");
					_message($"Restoring the {_name}.{_version} to {destination}");
					Restore(package, destination);
					return _other.Apply(root);
				})
				.OrElse(() =>
				{
					_error($"Not found the package {_name} in {string.Join("; ", _sources)} to restore!");
					return false;
				});
		}

		private static void Restore(string package, string destination)
		{
			if (Directory.Exists(destination))
				Directory.Delete(destination, true);
			using (var zip = new ZipFile(package))
				zip.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
			var rels = Path.Combine(destination, "_rels");
			if (Directory.Exists(rels))
				Directory.Delete(rels, true);
			var libs = Path.Combine(destination, "lib");
			Directory
				.GetDirectories(libs)
				.Select(d => d.ToLowerInvariant())
				.Where(d => d.Contains("%2b"))
				.ForEach(d => Directory.Move(d, d.Replace("%2b", "+")));
		}
	}
}
