namespace TS.FastNugetUpdate
{
	public interface INugetUpdate
	{
		bool Apply();
	}

	public class FakeUpdate : INugetUpdate
	{
		private readonly bool _success;
		public FakeUpdate(bool success)
		{
			_success = success;
		}

		public bool Apply() => _success;
	}
}