using Bussiness.Services.Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Bussiness.Helper
{
    public static class Helper
    {
        public static int Ceiling(int pageSize, int totalCount)
        {
            return (int)Math.Ceiling((decimal)totalCount / pageSize);
        }

        public static IQueryable<TSource> PageBy<TSource>([NotNull] this IQueryable<TSource> query, [NotNull] PaginationInput input)
        {
            return query!.Skip((int)input.PageSize! * ((int)input.PageNum! - 1)).Take((int)input.PageSize);
        }

        public static async Task<PaginationResult<TSource>> Pagination<TSource>([NotNull] this IQueryable<TSource> query, [NotNull] PaginationInput input, CancellationToken cancellationToken = default)
        {
            int totalCount = await query.CountAsync(cancellationToken: cancellationToken);

            query = query.PageBy(input);

            PaginationResult<TSource> data = new()
            {
                TotalCount = totalCount,
                TotalPage = Ceiling((int)input.PageSize!, totalCount),
                Content = await query.ToListAsync(cancellationToken: cancellationToken)
            };

            return data;
        }

        public static PaginationResult<TSource> Paging<TSource>([NotNull] this IQueryable<TSource> query, [NotNull] PaginationInput input)
        {
            int totalCount = query.Count();

            query = query.PageBy(input);

            PaginationResult<TSource> data = new()
            {
                TotalCount = totalCount,
                TotalPage = Ceiling((int)input.PageSize!, totalCount),
                Content = query.ToList()
            };

            return data;
        }
    }
}
