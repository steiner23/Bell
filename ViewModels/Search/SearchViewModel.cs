using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Books;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Search
{
    public class SearchViewModel
    {
        public List<BookWithAvailability> BooksWithAvailibilities { get; set; }
        public string Ctrl { get; set; }
        public string Act1 { get; set; }
        public string Act2 { get; set; }
    }
}









