using Shouldly;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;
using Tribitgroup.Framework.Shared.Enums;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    public class ConditionMakerTests
    {
        [Fact]
        public void StringValue_Should_enclose_with_Qoute()
        {
            var conditionMaker = ConditionMaker.Create("A", ConditionOperatorEnum.Equal, "Yashar");
            var res = conditionMaker.ToString();

            res.ShouldBe("A = 'Yashar'");
        }
        [Fact]
        public void In_Operator_Should_Create_Values_Inside_Parantesses()
        {
            var conditionMaker = ConditionMaker.Create("A", ConditionOperatorEnum.In, "Yashar", "Test");
            var res = conditionMaker.ToString();

            res.ShouldBe("A IN ('Yashar','Test')");
        }
        //[Fact]
        //public void Between_Operator_Should_Take_Just_First_Two_Parameters()
        //{
        //    var conditionMaker = new ConditionMaker
        //    {
        //        Operator = ConditionOperatorEnum.Between,
        //        VariableName = "A"
        //    };
        //    var res = conditionMaker.GetConditionString("first", "second", "C");

        //    res.ShouldBe("A BETWEEN 'first' AND 'second'");
        //}

        //[Fact]
        //public void Guid_Value_Should_Embrace_With_Qoutes()
        //{
        //    var guid = Guid.NewGuid();
        //    var conditionMaker = new ConditionMaker
        //    {
        //        Operator = ConditionOperatorEnum.Equal,
        //        VariableName = "ID"
        //    };
        //    var res = conditionMaker.GetConditionString(guid);

        //    res.ShouldBe($"ID = '{guid}'");
        //}
    }
}
