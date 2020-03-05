using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.ViewModels.Renew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Bell.Models.Books;
using Bell.Models.Loans;


namespace Bell.Controllers
{
    public class RenewController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;

        // constructor method, links the controller to model elements
        public RenewController(IBookRepository bookRepository, ILoanRepository loanRepository,
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
        public IActionResult RenewList()
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

            var renewViewModel = new RenewViewModel()
            {
                LoanMessage = loanMessage,
                LoanRenewList = loansWithDetails
            };

            return View(renewViewModel);
        }


        // ********


        [Authorize]
        [HttpPost]
        public IActionResult RenewList(string bookIdent)
        {
            string userId = _userManager.GetUserId(User);
            var localLoan = _loanRepository.GetLoanByBookIdent(bookIdent);
            var localBook = _bookRepository.GetBookByIdent(bookIdent);
            var overdueFee = _loanRepository.CalcOverdue(localLoan.LoanId, userId);

            var renewed = false;
            var canRenew = false;
            var renewalNotTooSoon = true;
            bool tempTextColRed = false;
            bool recentBorrow = false;

            var timesRenewed = localLoan.TimesRenewed;
            if (timesRenewed < 5)
            {
                canRenew = true;
            }

            if (localLoan.LatestRenewDate.AddDays(7) > DateTime.Now)
            {
                renewalNotTooSoon = false;
                if (localLoan.LatestRenewDate == localLoan.BorrowDate)
                {
                    recentBorrow = true;
                }
            }

            if ((canRenew == true) && (renewalNotTooSoon == true))
            {
                renewed = _loanRepository.RenewLoan(localLoan.LoanBookIdent, userId);
            }

            var loanMessage = "";
            if (renewed == true)
            {
                loanMessage = "'" + localBook.Name + "' was Renewed Successfully";
                if (overdueFee > 0)
                {
                    int euros = ((int)overdueFee / 100);
                    int cents = ((int)overdueFee % 100);
                    loanMessage = loanMessage +
                        "An Overdue fee of " + euros + "," + cents + "€ will apply";
                }
            }
            else if ((renewed == false) && (canRenew == true) && (renewalNotTooSoon == true))
            {
                loanMessage = "Loan Renewal was Unsuccessful";
                tempTextColRed = true;
            }
            else if ((canRenew == false) && (renewalNotTooSoon == true))
            {
                loanMessage = "Loan Renewal was Unsuccessful. This book has been Renewed a Maximum 5 times";
                if (overdueFee > 0)
                {
                    int euros = ((int)overdueFee / 100);
                    int cents = ((int)overdueFee % 100);
                    loanMessage = loanMessage +
                        "An Overdue fee of " + euros + "," + cents + "€ will apply";
                }
                tempTextColRed = true;
            }
            else if ((canRenew == true) && (renewalNotTooSoon == false) && recentBorrow == false)
            {
                loanMessage = "Loan Renewal was Unsuccessful. This book was Renewed less than 1 week ago";
                tempTextColRed = true;
            }
            else if ((canRenew == true) && (renewalNotTooSoon == false) && recentBorrow == true)
            {
                loanMessage = "Loan Renewal was Unsuccessful. This book was Borrowed less than 1 week ago";
                tempTextColRed = true;
            }


            var userLoans = _loanRepository.GetCurrentLoansByUserId(userId);
            var loansWithDetails = new List<LoanWithBookDetails>();

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

            var renewViewModel = new RenewViewModel()
            {
                LoanRenewList = loansWithDetails,
                LoanMessage = loanMessage,
                TextColorRed = tempTextColRed
            };

            return View(renewViewModel);
        }


        // ********


        public IActionResult RenewDetails(string id)
        {
            var book = _bookRepository.GetBookByIdent(id);
            if (book == null)
            {
                return NotFound();
            }

            var renewViewModel = new RenewDetailsViewModel()
            {
                LocalBook = book
            };

            return View(renewViewModel);
        }


        // ********


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
