using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models.Books;
// using Bell.Areas;
using Microsoft.AspNetCore.Identity;


namespace Bell.Models.Loans
{
    public class LoanHistoric
    {
        [Key]
        public int LoanHistoricId { get; set; }

        [Required] public int LoanId { get; set; }

        [Required]
        public int LoanBookId { get; set; }

        [ForeignKey("LoanBookId")]
        public virtual Book Books { get; set; }

        [Required]
        public string LoanBookIdent { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Client { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime BorrowDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime ScheduleReturnDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime ActualReturnDate { get; set; }

        public int TimesRenewed { get; set; }
    }
}
