using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace Bell.ViewModels.Peruse
{
    public class PeruseListViewModel
    {

        public List<Book> AvailableBooks { get; set; }
        public List<Book> UnAvailableBooks { get; set; }
        public List<BookWithAvailability> BooksWithAvailabilities { get; set; }

    }
}

