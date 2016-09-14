using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TS.FastNugetUpdate.Tests
{
	[TestClass]
	public class PreDownloadingNugetUpdateTests
	{
		[TestMethod]
		public void ApplyTest()
		{
			var fileRoot = Environment.CurrentDirectory;
			var repo = Path.Combine(fileRoot, "repo");
			var newPackagePlace = Path.Combine(fileRoot, "packages", "MK.Core.1.3.1.2");
			if (Directory.Exists(newPackagePlace))
				Directory.Delete(newPackagePlace, true);

			var sut = new PreDownloadingNugetUpdate(repo, "MK.Core", "1.3.1.2", 
				Console.Out.WriteLine, Console.Error.WriteLine,
				new FakeUpdate(true));

			Assert.IsTrue(sut.Apply(fileRoot));
			Assert.IsTrue(Directory.Exists(newPackagePlace));
		}
	}
}