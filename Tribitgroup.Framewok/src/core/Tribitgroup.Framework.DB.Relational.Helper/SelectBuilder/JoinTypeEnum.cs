namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public enum JoinTypeEnum : byte
    {
        InnerJoin = 0,
        Join = 0,
        RightJoin,
        LeftJoin,
        FullOuterJoin,
        CrossJoin
    }
}
