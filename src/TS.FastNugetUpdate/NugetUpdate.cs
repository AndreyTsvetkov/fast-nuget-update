using System;
using JetBrains.Annotations;

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

		public bool Apply()
		{
			return false;
		}
	}
}