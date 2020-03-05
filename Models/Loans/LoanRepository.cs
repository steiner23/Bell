using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bell.Models.Books;
using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.DependencyModel.Resolution;
// using Remotion.Linq.Clauses;


namespace Bell.Models.Loans
{
    public class LoanRepository : ILoanRepository
    {

        private readonly AppDbContext _appDbContext;
        private readonly IBookRepository _bookRepository;

        
        public LoanRepository(AppDbContext appDbContext, IBookRepository bookRepository)
        {
            _appDbContext = appDbContext;
            _bookRepository = bookRepository;
        }


        // ************************************************************************************


        public IEnumerable<Loan> GetAllLoans()
        {
            return _appDbContext.Loans.OrderBy(l => l.LoanId);
        }


        // ************************************************************************************


        public Loan GetLoanByBookIdent(string bookIdent)
        {
            return _appDbContext.Loans.FirstOrDefault(l => l.LoanBookIdent.Equals(bookIdent));
        }


        // ************************************************************************************


        public bool GetIsBookOnLoan(string bookIdent)
        {
            bool bookOnLoan = false;
            var loan = _appDbContext.Loans.FirstOrDefault(l => l.LoanBookIdent.Equals(bookIdent));
            if (loan != null)
            {
                bookOnLoan = true;
            }

            return bookOnLoan;
        }


        // ************************************************************************************


        public bool GetIsBookOnLoanById(int bookId)
        {
            bool bookOnLoan = false;
            var loan = _appDbContext.Loans.FirstOrDefault(l => l.LoanBookId.Equals(bookId));
            if (loan != null)
            {
                bookOnLoan = true;
            }

            return bookOnLoan;
        }


        // ************************************************************************************


        public IEnumerable<Loan> GetCurrentLoansByUserId(string userId)
        {
            return _appDbContext.Loans.OrderBy(l => l.LoanBookIdent).Where(u => u.UserId.Equals(userId));
        }


        // ************************************************************************************


        public IEnumerable<LoanHistoric> GetHistoricLoansByUserId(string userId)
        {
            return _appDbContext.LoansHistoric.OrderByDescending(h => h.BorrowDate).Where(u => u.UserId.Equals(userId));
        }


        // ************************************************************************************


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddNewLoan(string bookIdent, string userId)
        {
            var book = _bookRepository.GetBookByIdent(bookIdent);
            DateTime currentDateTime = DateTime.Now;
            var renewComplete = false;

            if (GetIsBookOnLoanById(book.BookId) == false)
            {
                _appDbContext.Loans.Add(
                    new Loan
                    {
                        LoanBookId = book.BookId,
                        LoanBookIdent = bookIdent,
                        UserId = userId,
                        BorrowDate = currentDateTime,
                        ScheduleReturnDate = currentDateTime.AddMonths(1),
                        LatestRenewDate = currentDateTime,
                        TimesRenewed = 0,
                    }
                );
                _appDbContext.SaveChanges();
                renewComplete = true;
            }
            return renewComplete;
        }


        // ************************************************************************************


        public bool RenewLoan(string bookIdent, string userId)
        {
            var loanRenewed = false;
            Loan loanRecord = _appDbContext.Loans.FirstOrDefault(
                l => l.LoanBookIdent.Equals(bookIdent) && l.UserId.Equals(userId));

            if ((loanRecord != null) && (loanRecord.TimesRenewed < 5))
            {
                loanRecord.ScheduleReturnDate = DateTime.Now.AddMonths(1);
                loanRecord.LatestRenewDate = DateTime.Now;
                loanRecord.TimesRenewed++;

                _appDbContext.Loans.Update(loanRecord);
                _appDbContext.SaveChanges();
                loanRenewed = true;
            }
            return (loanRenewed);
        }


        // ************************************************************************************


        public bool EndLoan(string bookIdent, string userId)
        {
            var loanReturned = false;
            var loanRecord = _appDbContext.Loans.FirstOrDefault(
                l => l.LoanBookIdent.Equals(bookIdent) && l.UserId.Equals(userId));

            if (loanRecord != null)
            {
                _appDbContext.Loans.Remove(loanRecord);
                _appDbContext.SaveChanges();
                loanReturned = true;
            }
            return (loanReturned);
        }


        // ************************************************************************************


        public float CalcOverdue(int loanId, string userId)
        {
            var overdueFee = 0;
            var dailyRate = 10;
            var loan = _appDbContext.Loans.FirstOrDefault(
                l => l.LoanId == loanId && l.UserId.Equals(userId));

            if(loan != null) 
            { 
                if (loan.ScheduleReturnDate < DateTime.Now)
                {
                    var totalDays = (loan.ScheduleReturnDate - DateTime.Now);
                    overdueFee = ((int)totalDays.TotalDays * dailyRate);
                }
            }
            return (overdueFee);
        }


        // ************************************************************************************
                     
    }
}
