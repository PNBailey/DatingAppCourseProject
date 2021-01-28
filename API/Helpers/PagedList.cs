using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// This class is created to enable Pagination. This class stores our pagination information and it holds the method needed to create our pagination response which will be returned to the client in the header 

namespace API.Helpers
{
    public class PagedList<T> : List<T> // The T makes this class generic. We are inheriting from the 'List' base class. 
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize); // We calcuate the total pages and we cast it to an int. We cast the page Size to a double type
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items); 

        }
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) { // This method wil return a task of our paged list of type generic. The source is the source data. 
            var count = await source.CountAsync(); // This makes the database call. If we want to calcualte the number of results we return, we have to use this countAsync method

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // If we were on page number 1 for instance and the page size was 5, it would be 1-0 = 0. 0*5 = 5. So this would mean that we are going to skip 0 records and we are going to take 5 records

            return new PagedList<T>(items, count, pageNumber, pageSize); // This returns a new instance of this class and passes in the required arguments for this classes constructor  
        }
    }
}