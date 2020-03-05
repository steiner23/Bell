using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.Models.Books;
using Bell.Models.Loans;
using Bell.ViewModels.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Bell.Controllers
{
    public class LoanController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;

        // constructor method, links the controller to model elements
        public LoanController(IBookRepository bookRepository, ILoanRepository loanRepository,
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
        public IActionResult LoanList()
        {
            string userId = _userManager.GetUserId(User);
            List<Loan> userLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            List<LoanWithBookDetails> loansWithDetails = new List<LoanWithBookDetails>();

            foreach (var loan in userLoans)
            {
                var loanPlusDetails = new LoanWithBookDetails();

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

            var loansViewModel = new LoanViewModel()
            {
                LoanMessage = loanMessage,
                LoansList = loansWithDetails
            };

            return View(loansViewModel);
        }


        // ********


        [Authorize]
        [HttpPost]
        public IActionResult LoanList(string includePrevious)
        {
            bool showOldLoans = bool.Parse(includePrevious);
            string userId = _userManager.GetUserId(User);
            List<Loan> userLoans = _loanRepository.GetCurrentLoansByUserId(userId).ToList();
            List<LoanWithBookDetails> loansWithDetails = new List<LoanWithBookDetails>();
            string loanHistoricMessage = null;

            foreach (var loan in userLoans)
            {
                var loanPlusDetails = new LoanWithBookDetails();

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

            List<LoanWithBookDetails> loansHistoricWithDetails = new List<LoanWithBookDetails>();
            if (showOldLoans == true)
            {
                List<LoanHistoric> userLoansHistoric = _loanRepository.GetHistoricLoansByUserId(userId).ToList();

                if (userLoansHistoric.Count > 0) {

                    loanHistoricMessage = "Previous Loans:";

                    foreach (var loan in userLoansHistoric)
                    {
                        var loanPlusDetails = new LoanWithBookDetails();
                        loanPlusDetails.BookId = loan.LoanBookId;
                        string bookIdent = _bookRepository.GetBookIdentById(loan.LoanBookId);
                        loanPlusDetails.BookIdent = bookIdent;                           
                        loanPlusDetails.UserId = loan.UserId;
                        loanPlusDetails.BorrowDate = loan.BorrowDate;
                        loanPlusDetails.ActualReturnDate = loan.ActualReturnDate;
                        loanPlusDetails.TimesRenewed = loan.TimesRenewed;

                        var bookDetails = _bookRepository.GetBookByIdent(bookIdent);
                        loanPlusDetails.Name = bookDetails.Name;
                        loanPlusDetails.Author = bookDetails.Author;
                        loanPlusDetails.Description = bookDetails.Description;
                        loanPlusDetails.BookCoverThumbnailUrl = bookDetails.BookCoverThumbnailUrl;

                        loansHistoricWithDetails.Add(loanPlusDetails);
                    }
                }
                else
                {
                    loanHistoricMessage = "No Previous Loans:";
                    loansHistoricWithDetails = null;
                }
            }

            var loanMessage = "My Loans";
            if (!userLoans.Any())
            {
                loanMessage = loanMessage + ": No Current Loans";
            }
                       
            var loansViewModel = new LoanViewModel()
            {
                LoansList = loansWithDetails,
                LoansHistoricWithDetails = loansHistoricWithDetails,
                LoanMessage = loanMessage,
                LoanHistoricMessage = loanHistoricMessage,                
                showHistoric = showOldLoans
            };

            return View(loansViewModel);
        }


        // ********


        public IActionResult LoanDetails(string id)
        {
            var book = _bookRepository.GetBookByIdent(id);
            if (book == null)
            {
                return NotFound();
            }

            var loanViewModel = new LoanDetailsViewModel()
            {
                LocalBook = book
            };

            return View(loanViewModel);
        }


        // ********


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}


