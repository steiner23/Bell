using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.Models.Books;
using Bell.Models.Loans;
using Bell.ViewModels.Borrow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Bell.Controllers
{
    public class BorrowController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private string _loansMessage = null;

        // constructor method, links the controller to model elements
        public BorrowController(IBookRepository bookRepository, ILoanRepository loanRepository, 
            UserManager<ApplicationUser> userManager, AppDbContext appDbContext)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }


        // ************************************************************************************


        [Authorize]
        [HttpGet]
        public IActionResult BorrowList()
        {
            // get the userId and their loans if any
            string userId = _userManager.GetUserId(User);
            List<Loan> userLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            var userLoansTotal = userLoans.Count();

            // prepare a message for the user advising them of their loan status
            var loansLeft = (5 - userLoansTotal);
            switch (userLoansTotal)
            {
                case int i when i == 0:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. You may borrow up to " + loansLeft + " books.";
                    break;
                case int i when i == 1:
                    _loansMessage = "You currently have " + userLoansTotal + " loan. You may borrow another " + loansLeft + " more books.";
                    break;
                case int i when i > 1 && i <= 3:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. You may borrow another " + loansLeft + " more books.";
                    break;
                case int i when i == 4:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. You may borrow 1 more book.";
                    break;
                case int i when i == 5:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. This is the maximum number of loans.";
                    break;
            }

            // get the books referenced by the loans if any
            var bookLoans = _bookRepository.GetUsersLoanedBooks(userLoans, userId);

            // get all available books not existing in the loan table
            List<Book> availableBooks = _bookRepository.GetAvailableBooks().ToList();

            var borrowListViewModel = new BorrowListViewModel()
            {
                AvailableBooks = availableBooks.ToList(),
                UserLoanedBooks = bookLoans.ToList(),
                UserId = _userManager.GetUserId(User),
                LoanMessage = _loansMessage
            };

            return View(borrowListViewModel);
        }


        // ********


        [Authorize]
        [HttpPost]
        public IActionResult BorrowList(string bookIdent)
        {
            string userId = _userManager.GetUserId(User);
            List<Loan> allUserLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            var userLoansTotal = allUserLoans.Count();
            var loanAdded = false;

            // add the new loan to the DB if the user doesn't have five books out on loan
            if (userLoansTotal < 5)
            {
                loanAdded = _loanRepository.AddNewLoan(bookIdent, userId);
            }

            // update loan data following the addition of a new loan to the loan table
            allUserLoans.Clear();
            allUserLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            userLoansTotal = allUserLoans.Count();

            if ((userLoansTotal != 5) && (loanAdded == false))
            {
                userLoansTotal = 100;
            }

            // prepare a message for the user advising them of their loan status
            var loansLeft = (5 - userLoansTotal);
            switch(userLoansTotal)
            {
                case int i when i == 0:
                    _loansMessage = "You currently have " + userLoansTotal + " loan. You may borrow up to " + loansLeft + " books.";
                    break;
                case int i when i == 1:
                    _loansMessage = "You currently have " + userLoansTotal + " loan. You may borrow another " + loansLeft + " books.";
                    break;
                case int i when i > 1 && i <= 3:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. You may borrow another " + loansLeft + " books.";
                    break;
                case int i when i == 4:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. You may borrow 1 more book.";
                    break;
                case int i when i == 5:
                    _loansMessage = "You currently have " + userLoansTotal + " loans. This is the maximum number of loans.";
                    break;
                case int i when i == 6:
                    _loansMessage = "You currently have 5 loans. This is the maximum number of loans.";
                    break;
                case int i when i == 100:
                    _loansMessage = "Borrow Request Failed";
                    break;
            }

            // get the user's loaned books
            List<Book> bookLoans = null;
            if (userLoansTotal > 0)
            {
                bookLoans = _bookRepository.GetUsersLoanedBooks(allUserLoans, userId).ToList();
            }

            // get all available books not existing in the loan table
            List<Book> availableBooks = _bookRepository.GetAvailableBooks().ToList();

            bool tempTextColRed = true;
            if ((userLoansTotal >= 0) && (userLoansTotal < 5))
            {
                tempTextColRed = false;
            }

            var borrowListViewModel = new BorrowListViewModel()
            {
                AvailableBooks = availableBooks,
                UserLoanedBooks = bookLoans,
                LoanBookCount = userLoansTotal,
                UserId = _userManager.GetUserId(User),
                LoanMessage = _loansMessage,
                TextColorRed = tempTextColRed
            };

            return View(borrowListViewModel);
        }


        // ********


        [Authorize]
        public IActionResult BorrowDetails(string id)
        {
            var book = _bookRepository.GetBookByIdent(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookAvailable = false;
            var loan = _loanRepository.GetLoanByBookIdent(id);
            if (loan == null)
            {
                bookAvailable = true;
            }

            var bookViewModel = new BookBorrowDetailsViewModel()
            {
                Book = book,
                Available = bookAvailable
            };

            return View(bookViewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}


