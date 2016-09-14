using System;
using System.Configuration;
using CommandLine;
using Functional.Maybe;
using MoreLinq;

namespace TS.FastNugetUpdate
{
	class Program
	{
		static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<CommandLineOptions>(args)
				.MapResult(
					options =>
					{
						INugetUpdate update = new NugetUpdate(options.PackageName, options.Version, Console.Out.WriteLine);
						if (!options.DontTryDownloadingPackage)
							update = new PreDownloadingNugetUpdate(
								ConfigurationManager.AppSettings["packageSources"],
								options.PackageName, options.Version,
								Console.Out.WriteLine, Console.Error.WriteLine,
								update
							);
						try
						{
							return update
								.Apply(Environment.CurrentDirectory)
								.Then(0)
								.OrElse(1);
						}
						catch (Exception ex)
						{
							Console.Error.WriteLine(ex);
							return 10;
						}
					},
					errors =>
					{
						errors.ForEach(error => Console.Error.WriteLine(error));
						return -1;
					}
				);
		}
	}
}
