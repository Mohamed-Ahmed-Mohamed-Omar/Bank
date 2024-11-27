using Microsoft.EntityFrameworkCore;

namespace Bank.Core.Wrappers
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
            where T : class
        {
            if (source == null)
            {
                throw new Exception("Empty");
            }

            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;

            int count = source.Count(); 
            if (count == 0) return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // يستخدم ToList بدلًا من ToListAsync
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
            where T : class
        {
            if (source == null)
            {
                throw new Exception("Empty");
            }

            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;

            int count = await source.AsNoTracking().CountAsync(); // CountAsync هنا لأنها IQueryable
            if (count == 0) return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // ToListAsync هنا لأنها IQueryable
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
    }
}
