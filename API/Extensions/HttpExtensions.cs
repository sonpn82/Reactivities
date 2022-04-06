using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        // to show additional information on the query request for activities
        // add more info about the paging besides the returned activity list
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
            {
                var paginationHeader = new 
                {
                    currentPage,
                    itemsPerPage,
                    totalItems,
                    totalPages
                };
                response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
                // have to add below line because Pagination is our customer header
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");  
            }
    }
}