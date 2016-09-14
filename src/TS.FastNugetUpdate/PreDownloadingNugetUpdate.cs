using System;
using JetBrains.Annotations;

namespace TS.FastNugetUpdate
{
	public sealed class PreDownloadingNugetUpdate : INugetUpdate
	{
		public PreDownloadingNugetUpdate([NotNull]string sources, [NotNull]string name, [NotNull]string version,
			[NotNull]Action<string> message, [NotNull]Action<string> error, INugetUpdate other) { }

		public bool Apply(string root)
		{
			return false;
		}
	}

}