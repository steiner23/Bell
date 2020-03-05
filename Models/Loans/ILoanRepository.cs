using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bell.Models.Loans
{
    public interface ILoanRepository
    {

        IEnumerable<Loan> GetAllLoans();
        Loan GetLoanByBookIdent(string bookIdent);
        bool GetIsBookOnLoan(string bookIdent);
        IEnumerable<Loan> GetCurrentLoansByUserId(string userId);
        IEnumerable<LoanHistoric> GetHistoricLoansByUserId(string userId); 
        bool AddNewLoan(string bookIdent, string userId);
        bool RenewLoan(string bookIdent, string userId);
        bool EndLoan(string bookIdent, string userId);
        float CalcOverdue(int loanId, string userId);
        
    }
}




