using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Return
{
    public class ReturnViewModel
    {
        public IEnumerable<LoanWithBookDetails> LoanReturnList { get; set; }
        public string LoanMessage { get; set; }
    }
}
