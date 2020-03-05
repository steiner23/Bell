using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bell.Models.Books;
using Microsoft.AspNetCore.Identity;


namespace Bell.Models.Loans
{
    public class LoanWithBookDetails : Book
    {
        
        public string UserId { get; set; }

        public string LoanBookIdent { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime ScheduleReturnDate { get; set; }

        public DateTime ActualReturnDate { get; set; }
               
        public int TimesRenewed { get; set; }

        public DateTime LatestRenewDate { get; set; }

    }
}



