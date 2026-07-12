using System.Collections.Generic;

namespace Shared.DataTransferObjects
{
    public record PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = new List<T>();
        public int TotalCount { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 25;
    }
}
