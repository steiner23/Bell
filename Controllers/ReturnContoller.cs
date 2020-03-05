using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.ViewModels.Return;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Bell.Models.Books;
using Bell.Models.Loans;


namespace Bell.Controllers
{
    public class ReturnController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;

        // constructor method, links the controller to model elements
        public ReturnController(IBookRepository bookRepository, ILoanRepository loanRepository,
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
        public IActionResult ReturnList()
        {
            string userId = _userManager.GetUserId(User);
            List<Loan> userLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            List<LoanWithBookDetails> loansWithDetails = new List<LoanWithBookDetails>();

            foreach (var loan in userLoans)
            {
                var loanPlusDetails = new LoanWithBookDetails();
                // var loanPlusDetails = _appDbContext.LoansWithBookDetails;

                loanPlusDetails.BookId = loan.LoanBookId;
                loanPlusDetails.BookIdent = loan.LoanBookIdent;
                loanPlusDetails.UserId = loan.UserId;
                loanPlusDetails.BorrowDate = loan.BorrowDate;
                DateTime tempScheduleReturnDate = loan.ScheduleReturnDate;
                loanPlusDetails.ScheduleReturnDate = tempScheduleReturnDate.Date;
                loanPlusDetails.TimesRenewed = loan.TimesRenewed;

                var bookDetails = _bookRepository.GetBookByIdent(loan.LoanBookIdent);
                loanPlusDetails.Name = bookDetails.Name;
                loanPlusDetails.Author = bookDetails.Author;
                loanPlusDetails.Description = bookDetails.Description;
                loanPlusDetails.BookCoverThumbnailUrl = bookDetails.BookCoverThumbnailUrl;

                loansWithDetails.Add(loanPlusDetails);
            }

            var loanMessage = "My Loans";
            if (!userLoans.Any())
            {
                loanMessage = loanMessage + ": No Current Loans";
            }

            var returnViewModel = new ReturnViewModel()
            {
                LoanMessage = loanMessage,
                LoanReturnList = loansWithDetails
            };

            return View(returnViewModel);
        }


        // ********


        [Authorize]
        [HttpPost]
        public IActionResult ReturnList(string bookIdent)
        {
            string userId = _userManager.GetUserId(User);
            Loan localLoan = _loanRepository.GetLoanByBookIdent(bookIdent);
            Book localBook = _bookRepository.GetBookByIdent(bookIdent);
            var overdueFee = _loanRepository.CalcOverdue(localLoan.LoanId, userId);

            var Returned = false;
            Returned = _loanRepository.EndLoan(localLoan.LoanBookIdent, userId);

            string loanMessage = null;
            if (Returned == true)
            {
                loanMessage = "'" + localBook.Name + "' was Successfully Returned";
            }
            else
            {
                loanMessage = "Return of  " + localBook.Name + " was Unsuccessful";
            }

            if(overdueFee > 0)
            {
                int euros = ((int)overdueFee / 100);
                int cents = ((int)overdueFee % 100);
                loanMessage = loanMessage +
                    "An Overdue fee of " + euros + "," + cents + "€  will apply";
            }
            
            var userLoans = _loanRepository.GetCurrentLoansByUserId(userId);
            var loansWithDetails = new List<LoanWithBookDetails>();

            foreach (var loan in userLoans)
            {
                var loanPlusDetails = new LoanWithBookDetails();
                // var loanPlusDetails = _appDbContext.LoansWithBookDetails;

                loanPlusDetails.BookId = loan.LoanBookId;
                loanPlusDetails.BookIdent = loan.LoanBookIdent;
                loanPlusDetails.BorrowDate = loan.BorrowDate;
                DateTime tempScheduleReturnDate = loan.ScheduleReturnDate;
                loanPlusDetails.ScheduleReturnDate = tempScheduleReturnDate.Date;
                loanPlusDetails.TimesRenewed = loan.TimesRenewed;

                var bookDetails = _bookRepository.GetBookByIdent(loan.LoanBookIdent);
                loanPlusDetails.Name = bookDetails.Name;
                loanPlusDetails.Author = bookDetails.Author;
                loanPlusDetails.Description = bookDetails.Description;
                loanPlusDetails.BookCoverThumbnailUrl = bookDetails.BookCoverThumbnailUrl;

                loansWithDetails.Add(loanPlusDetails);
            }

            var returnViewModel = new ReturnViewModel()
            {
                LoanReturnList = loansWithDetails,
                LoanMessage = loanMessage
            };

            return View(returnViewModel);
        }


        // ********


        public IActionResult ReturnDetails(string id)
        {
            var book = _bookRepository.GetBookByIdent(id);
            if (book == null)
            {
                return NotFound();
            }

            var ReturnViewModel = new ReturnDetailsViewModel()
            {
                LocalBook = book
            };

            return View(ReturnViewModel);
        }


        // ********


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
