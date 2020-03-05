using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Identity;

namespace Bell.Models.Books
{
    public interface IBookRepository
    {

        IEnumerable<Book> GetAllBooks();

        IEnumerable<Book> GetAvailableBooks();

        Book GetBookByIdent(string bookIdent);

        string GetBookIdentById(int bookId);

        IEnumerable<BookWithAvailability> GetAllBooksWithAvailability();

        IEnumerable<Book> GetUsersLoanedBooks(IEnumerable<Loan> userLoans, string userId);

        string GenNewBookIdent();

        IEnumerable<Book> GetSearchBooks(string scope, string searchTerm);

    }
}


