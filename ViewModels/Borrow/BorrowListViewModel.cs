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
    public class BorrowListViewModel
    {
        public List<Book> AvailableBooks { get; set; }
        public List<Book> UserLoanedBooks { get; set; }
        public int LoanBookCount { get; set; }
        public string UserId { get; set; }
        public string LoanMessage { get; set; }
        public bool TextColorRed { get; set; } 
    }
}
