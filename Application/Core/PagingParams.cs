using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    public class PagingParams
    {
        // only allow user to set max page size to 50
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        // initial pageSize = 10
        public int _pageSize = 10;
        // allow user to edit page size but max is 50
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}