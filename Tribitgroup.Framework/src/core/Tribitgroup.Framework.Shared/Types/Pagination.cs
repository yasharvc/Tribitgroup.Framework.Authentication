namespace Tribitgroup.Framework.Shared.Types
{
    public class Pagination
	{
		public int Page { get; set; } = 1;
		public int Count { get; set; } = 0;
		public Pagination()
		{
			Page = 1;
			Count = 100;
		}
	}
}