using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Borrow
{
    public class BookBorrowDetailsViewModel
    {
        public Book Book { get; set; }
        public bool Available { get; set; }
    }
}
