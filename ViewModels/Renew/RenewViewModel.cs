using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Renew
{
    public class RenewViewModel
    {
        public IEnumerable<LoanWithBookDetails> LoanRenewList { get; set; }
        public string LoanMessage { get; set; }
        public int LoanBookId { get; set; }
        public bool TextColorRed { get; set; }
    }
}
