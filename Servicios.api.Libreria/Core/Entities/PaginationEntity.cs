using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core.Entities
{
    public class PaginationEntity<TDocument>
    {
        public int PageSize { set; get; }
        public int Page { set; get; }
        public string Sort { set; get; }
        public string SortDirection { set; get; }
        public string Filter { set; get; }
        public FilterValue Filters { set; get; }
        public int PagesQuantity { set; get; }
        public long TotalDocuments { set; get; }
        public IEnumerable<TDocument> Data { set; get; }
    }
}
