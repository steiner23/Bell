using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bell.Models;
using Bell.Models.Books;
using Bell.Models.Loans;
using Bell.ViewModels.Peruse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace Bell.Controllers
{
    public class PeruseController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;

        // constructor method, links the controller to model elements
        public PeruseController(IBookRepository bookRepository, ILoanRepository loanRepository, 
            UserManager<ApplicationUser> userManager, AppDbContext appDbContext)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }


        // ************************************************************************************


        public IActionResult PeruseList()
        {
            var peruseListViewModel = new PeruseListViewModel()
            {
                BooksWithAvailabilities = _bookRepository.GetAllBooksWithAvailability().ToList()
            };

            return View(peruseListViewModel);
        }


        // *********


        public IActionResult PeruseDetails(string id)
        {
            var book = _bookRepository.GetBookByIdent(id);
            if (book == null)
            {
                return NotFound();
            }

            Boolean bookAvailable = false;
            var loan = _loanRepository.GetLoanByBookIdent(id);
            if (loan == null)
            {
                bookAvailable = true;
            }

            var peruseViewModel = new PeruseDetailsViewModel()
            {
                Book = book,
                Available = bookAvailable
            };

            return View(peruseViewModel);
        }


        // *********


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}


