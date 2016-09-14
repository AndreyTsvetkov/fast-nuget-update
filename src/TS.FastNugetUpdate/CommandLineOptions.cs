using CommandLine;

namespace TS.FastNugetUpdate
{
	public class CommandLineOptions
	{
		[Option('n', "name", HelpText = "name of the package to find", Required = true)]
		public string PackageName { get; set; }

		[Option('v', "version", HelpText = "version of the package to set", Required = true)]
		public string Version { get; set; }

		[Option('s', "skip-downloading", HelpText = "if set, no attempts would occur to download the package from package sources in the app.config file", Required = true)]
		public bool DontTryDownloadingPackage { get; set; }
	}
}
