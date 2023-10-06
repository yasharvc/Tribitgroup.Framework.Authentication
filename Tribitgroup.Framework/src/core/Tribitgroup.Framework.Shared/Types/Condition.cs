using Tribitgroup.Framework.Shared.Enums;

namespace Tribitgroup.Framework.Shared.Types
{
    public class Condition
	{
		public string PropertyName { get; set; } = string.Empty;
		public ConditionOperatorEnum Operator { get; set; } = ConditionOperatorEnum.Equal;
		public List<string> Values { get; set; } = new List<string>();
	}
}
