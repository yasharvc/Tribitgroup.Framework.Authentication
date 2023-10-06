using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class Query
    {
        dynamic FromTable { get; set; }
        List<dynamic> Joins { get; set; } = new List<dynamic>();
        List<Column> SelectedColumns { get; set; } = new List<Column>();
        ICollection<Func<ConditionMaker>> Conditions { get; set; } = new List<Func<ConditionMaker>>();
        Query FromSelect { get; set; }
        string Alias { get; set; }
        private Query()
        {

        }

        public static Query From<ENTITY, IDTYPE>() where IDTYPE : notnull where ENTITY : IEntity<IDTYPE>, new()
        {
            var res = new Query
            {
                FromTable = new ENTITY(),
                Alias = new ENTITY().GetTableName()
            };
            return res;
        }

        public static Query From<T>() where T : IEntity<Guid>, new() => From<T, Guid>();

        public static Query From(Query from)
        {
            var res = new Query
            {
                FromSelect = from,
            };
            return res;
        }

        public void SetInnerSelect(Query innerSelect) => FromSelect = innerSelect;

        public void InnerJoin<ENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where ENTITY : IEntity<IDTYPE>, new()
        {
            var toEntity = new ENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = GetFromTable<IDTYPE>(),
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.InnerJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }
        public void InnerJoin<FROMENTITY, TOENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where FROMENTITY : IEntity<IDTYPE>, new()
            where TOENTITY : IEntity<IDTYPE>, new()
        {
            var fromEntity = new FROMENTITY();
            var toEntity = new TOENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = fromEntity,
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.InnerJoin,
                Relation = join(),
                IsDetailToDetailJoin = typeof(FROMENTITY) != FromTable.GetType(),
            };
            Joins.Add(entityJoin);
        }

        public void LeftJoin<ENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where ENTITY : IEntity<IDTYPE>, new()
        {
            var toEntity = new ENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = GetFromTable<IDTYPE>(),
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.LeftJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }

        public void LeftJoin<FROMENTITY, TOENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where FROMENTITY : Entity<IDTYPE>, new()
            where TOENTITY : Entity<IDTYPE>, new()
        {
            var fromEntity = new FROMENTITY();
            var toEntity = new TOENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = fromEntity,
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.LeftJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }

        public void Select(Func<IEnumerable<Column>> cols)
            => SelectedColumns.AddRange(cols());
        public void Where(Func<ConditionMaker> func) => Conditions.Add(func);

        public override string ToString()
        {
            var selectCols = string.Join(", ", SelectedColumns.Select(m => m.ToString()));
            var joins = "";
            if (Joins.Any())
            {
                joins = string.Join(" ", Joins.Select(m => m.ToString()));
            }
            else
            {
                joins = string.Join(", ", SelectedColumns.Select(m => m.TableName).Distinct());
            }
            var where = GetWhereClause();
            where = where.Any() ? $" WHERE {where}" : "";
            return $"SELECT {(string.IsNullOrEmpty(selectCols) ? "*" : selectCols)} FROM {GetFromTablePart()} {joins} {where}";
        }

        private string GetWhereClause()
        {
            var where = "";

            foreach (var item in Conditions)
            {
                where += $"{(where.Any() ? " AND " : "")} ({item()})";
            }

            return where;
        }

        private string GetFromTablePart()
        {
            if (FromSelect == null)
                return FromTable.GetTableName();
            return $"({FromSelect}) as {FromSelect.Alias}";
        }
        private IEntity<T> GetFromTable<T>() where T : notnull
        {
            if (FromSelect == null)
                return FromTable;
            return FromSelect.FromTable;
        }
    }
}
