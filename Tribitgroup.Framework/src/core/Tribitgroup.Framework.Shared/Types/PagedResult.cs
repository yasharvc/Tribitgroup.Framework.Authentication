namespace Tribitgroup.Framework.Shared.Types
{
    public class PagedResult<T>
    {
        public ICollection<T> Result { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public PagedResult()
        {

        }

        public static implicit operator PagedResult<T>(System.Linq.Dynamic.Core.PagedResult<T> paged)
        {
            var list = paged.Queryable.ToList();
            var result = new PagedResult<T>
            {
                CurrentPage = paged.CurrentPage,
                PageSize = paged.PageSize,
                RowCount = paged.RowCount,
                PageCount = paged.PageCount
            };
            foreach (var item in list)
            {
                result.Result.Add(item);
            }
            return result;
        }
    }
}