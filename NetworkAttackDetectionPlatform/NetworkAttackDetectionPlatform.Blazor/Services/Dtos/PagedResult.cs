using System.Collections.Generic;
using System.Collections.Generic;

namespace NetworkAttackDetectionPlatform.Blazor.Services.Dtos
{
    public sealed class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
