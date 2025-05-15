using event_horizon_backend.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace event_horizon_backend.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            TotalRecords = count,
            Data = items
        };
    }
}