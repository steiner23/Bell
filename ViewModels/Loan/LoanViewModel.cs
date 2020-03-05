using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Loan
{
    public class LoanViewModel
    {
        public IEnumerable<LoanWithBookDetails> LoansList { get; set; }
        public IEnumerable<LoanWithBookDetails> LoansHistoricWithDetails { get; set; }
        public string LoanMessage { get; set; }
        public string LoanHistoricMessage { get; set; }
        public int LoanBookId { get; set; }
        public bool showHistoric { get; set; }
    }
}
