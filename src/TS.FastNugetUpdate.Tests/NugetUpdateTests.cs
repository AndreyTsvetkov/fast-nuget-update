using Microsoft.VisualStudio.TestTools.UnitTesting;
using TS.FastNugetUpdate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TS.FastNugetUpdate.Tests
{
	[TestClass]
	public class NugetUpdateTests
	{
		[TestMethod]
		public void Apply_ChangesVersionToCorrect()
		{
			var fileRoot = Environment.CurrentDirectory;

			var sut = new NugetUpdate("My.Package", "0.0.2", s => { }, s => { });

			Assert.IsTrue(sut.Apply(fileRoot));
			var projectFile = Path.Combine(fileRoot, "demo", "demo.csproj");
			var packagesFile = Path.Combine(fileRoot, "demo", "packages.config");
			Assert.IsTrue(File.ReadAllText(projectFile).Contains("0.0.2"));
			Assert.IsTrue(File.ReadAllText(packagesFile).Contains("0.0.2"));
		}

		[TestMethod]
		public void Apply_RemainsLinesNumberTheSame()
		{ 
			var fileRoot = Environment.CurrentDirectory;
			var projectFile = Path.Combine(fileRoot, "demo", "demo.csproj");
			var packagesFile = Path.Combine(fileRoot, "demo", "packages.config");
			var numbersBefore = new
			{
				csproj = File.ReadAllLines(projectFile).Length,
				packages = File.ReadAllLines(packagesFile).Length
			};

			var sut = new NugetUpdate("My.Package", "0.0.2", s => { }, s => { });

			sut.Apply(fileRoot);

			var numbersAfter = new
			{
				csproj = File.ReadAllLines(projectFile).Length,
				packages = File.ReadAllLines(packagesFile).Length
			};

			Assert.AreEqual(numbersBefore, numbersAfter);
		}
	}
}