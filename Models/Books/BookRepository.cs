
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bell.Models.Loans;
using Bell.Models.Books;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;


namespace Bell.Models.Books
{
    public class BookRepository : IBookRepository
    {

        // links the repository to the context
        private readonly AppDbContext _appDbContext;


        public BookRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        // ************************************************************************************


        public IEnumerable<Book> GetAllBooks()
        {
            return _appDbContext.Books.OrderBy(b => b.Name);
        }


        // ************************************************************************************


        public IEnumerable<BookWithAvailability> GetAllBooksWithAvailability()
        {
            var allBooks = _appDbContext.Books.OrderBy(b => b.Name).ToList();
            var localUnavailableBooks = from b in _appDbContext.Books
                join l in _appDbContext.Loans
                    on b.BookIdent equals l.LoanBookIdent
                select b;
            var unavailableBooks = localUnavailableBooks.ToList();

            var booksWithAvailabilities = new List<BookWithAvailability>();
            foreach (var localBook in allBooks)
            {
                var bookWithAvailability = new BookWithAvailability
                    {Book = localBook};

                var available = false;

                bool AvailabilityCheck(Book val)
                {
                    return val == localBook;
                }

                available = unavailableBooks.Exists(AvailabilityCheck);
                if (available == false)
                {
                    bookWithAvailability.Available = true;
                }

                booksWithAvailabilities.Add(bookWithAvailability);
            }

            return (booksWithAvailabilities);
        }


        // ************************************************************************************


        public Book GetBookByIdent(string bookIdent)
        {
            return _appDbContext.Books.FirstOrDefault(b => b.BookIdent.Equals(bookIdent));
        }


        // ************************************************************************************


        public string GetBookIdentById(int bookId)
        {
            Book book = _appDbContext.Books.FirstOrDefault(b => b.BookId.Equals(bookId));
            return book.BookIdent;
        }


        // ************************************************************************************


        public IEnumerable<Book> GetAvailableBooks()
        {
            var books = _appDbContext.Books.OrderBy(b => b.Name);

            var loanBooks = from b in _appDbContext.Books
                join l in _appDbContext.Loans
                    on b.BookIdent equals l.LoanBookIdent
                select b;

            var availableBooks = books.Except(loanBooks);

            return (availableBooks);
        }


        // ************************************************************************************


        public IEnumerable<Book> GetUnavailableBooks()
        {
            IEnumerable<Book> loanBooks =
                (from b in _appDbContext.Books
                    join l in _appDbContext.Loans
                        on b.BookIdent equals l.LoanBookIdent
                    select b);

            return (loanBooks);
        }


        // ************************************************************************************


        public IEnumerable<Book> GetUsersLoanedBooks(IEnumerable<Loan> userLoans, string userId)
        {
            var bookLoansQuery = (from b in _appDbContext.Books.OrderBy(b => b.Name)
                join l in userLoans on b.BookIdent equals l.LoanBookIdent
                where l.UserId == userId
                select b);

            IEnumerable<Book> bookLoans = bookLoansQuery;

            return (bookLoans);
        }


        // ************************************************************************************


        public string GenNewBookIdent()
        {
            int stringLength = 16;
            string valString = GetUniqueKey(stringLength);

            var bookIdent = (from b in _appDbContext.Books
               where b.BookIdent == valString
               select b.BookIdent.ToString());

            if (bookIdent.Any())
            {
                valString = GetUniqueKey(stringLength);
            }
            return (valString);
        }

        public string GetUniqueKey(int size)
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }
            return (result.ToString());
        }


        // ************************************************************************************


        public IEnumerable<Book> GetSearchBooks(string scope, string searchTerm)
        {
            var searchBooks = new List<Book>();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                switch (scope)
                {
                    case "title":
                        var targetString = "%" + searchTerm + "%";
                        var tempBooks = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Name, targetString)
                            select t;
                        searchBooks.AddRange(tempBooks.ToList());
                        break;

                    case "author":
                        var targetString2 = "%" + searchTerm + "%";
                        var tempBooks2 = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Author, targetString2)
                            select t;
                        searchBooks.AddRange(tempBooks2.ToList());
                        break;

                    case "description":
                        var targetString3 = "%" + searchTerm + "%";
                        var tempBooks3 = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Description, targetString3)
                            select t;
                        searchBooks.AddRange(tempBooks3.ToList());
                        break;

                    case "all":
                        var targetString4 = "%" + searchTerm + "%";
                        var tempBooks4 = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Name, targetString4)
                            select t;
                        searchBooks.AddRange(tempBooks4.ToList());

                        var targetString5 = "%" + searchTerm + "%";
                        var tempBooks5 = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Author, targetString5)
                            select t;
                        searchBooks.AddRange(tempBooks5.ToList());

                        var targetString6 = "%" + searchTerm + "%";
                        var tempBooks6 = from t in _appDbContext.Books
                            where EF.Functions.Like(t.Author, targetString6)
                            select t;
                        searchBooks.AddRange(tempBooks6.ToList());
                        break;
                }
            }

            HashSet<Book> distinctSet = new HashSet<Book>(searchBooks);
            searchBooks.Clear();
            searchBooks = distinctSet.ToList();

            return (searchBooks);
        }
    }

}






