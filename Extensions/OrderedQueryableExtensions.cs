using PWANews.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.Extensions
{
    public static class OrderedQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IOrderedQueryable<T> queryable, PaginationInputModel pagination)
        {
            return queryable.Skip(pagination.Offset).Take(pagination.Size);
        }
    }
}
