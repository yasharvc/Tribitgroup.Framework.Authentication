using Tribitgroup.Framework.Shared.Enums;

namespace Tribitgroup.Framework.Shared.Types
{
    public class Sort
	{
		public string Column { get; set; } = string.Empty;
		public SortTypeEnum SortType { get; set; }
		public override string ToString() => $"{Column}{(SortType == SortTypeEnum.ASC ? "" : " DESC")}";
	}
}
