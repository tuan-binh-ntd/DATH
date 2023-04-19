using Bussiness.Services;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Helper
{
    public static class Helper
    {
        public static int Ceiling(int pageSize, int totalCount)
        {
            return (int)Math.Ceiling((decimal) totalCount /pageSize);
        }

        public static IQueryable<TSource> PageBy<TSource>(this IQueryable<TSource> query, PaginationInput input)
        {
            return query!.Skip(input.PageSize * (input.PageNum - 1)).Take(input.PageSize);
        }

        public static async Task<PaginationResult<TSource>> Pagination<TSource>(this IQueryable<TSource> query, PaginationInput input)
        {
            int totalCocunt = await query.CountAsync();

            query = query.PageBy(input);

            PaginationResult<TSource> data = new()
            {
                TotalCount = totalCocunt,
                TotalPage = Helper.Ceiling(input.PageSize, totalCocunt),
                Content = await query.ToListAsync()
            };

            return data;
        }
    }
}
