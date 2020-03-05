using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models.Books;
using Bell.Models.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace Bell.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanHistoric> LoansHistoric { get; set; }
        public LoanWithBookDetails LoansWithBookDetails {get; set;}

    }
}
