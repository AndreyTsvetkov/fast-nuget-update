using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TS.FastNugetUpdate.Tests
{
	[TestClass]
	public class PreDownloadingNugetUpdateTests
	{
		//[TestMethod]
		public void ApplyTest()
		{
			var fileRoot = Environment.CurrentDirectory;
			var repo = Path.Combine(fileRoot, "repo");
			var newPackagePlace = Path.Combine(fileRoot, "packages", "My.Package.0.0.1");
			if (Directory.Exists(newPackagePlace))
				Directory.Delete(newPackagePlace, true);

			var sut = new PreDownloadingNugetUpdate(repo, "My.Package", "0.0.1", s => { }, s => { },
				new FakeUpdate(true));

			Assert.IsTrue(sut.Apply(fileRoot));
			Assert.IsTrue(Directory.Exists(newPackagePlace));
		}
	}
}