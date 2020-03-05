using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.Models.Books;
using Bell.Models.Loans;
using Bell.ViewModels;
using Bell.ViewModels.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Bell.Controllers
{
    public class SearchController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly AppDbContext _appDbContext;

        // constructor method
        public SearchController(IBookRepository bookRepository, ILoanRepository loanRepository, AppDbContext appDbContext)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _appDbContext = appDbContext;
        }


        // ************************************************************************************

        [HttpPost]
        public IActionResult SearchList(string Ctrl, string Act1, string Act2, string scope, string searchTerm)
        {
            List<BookWithAvailability> booksWithAvailabilities = new List<BookWithAvailability>();
            List<Book> bookSearchList = _bookRepository.GetSearchBooks(scope, searchTerm).ToList();

            foreach (var book in bookSearchList)
            {
                BookWithAvailability bookWithAvailability = new BookWithAvailability();
                bookWithAvailability.Book = book;
                bookWithAvailability.Available = !_loanRepository.GetIsBookOnLoan(book.BookIdent);
                booksWithAvailabilities.Add(bookWithAvailability);
            }
            
            var searchViewModel = new SearchViewModel()
            {
                BooksWithAvailibilities = booksWithAvailabilities,
                Ctrl = Ctrl,
                Act1 = Act1,
                Act2 = Act2
            };

            return View(searchViewModel);
        }


        // *********


    }
}


