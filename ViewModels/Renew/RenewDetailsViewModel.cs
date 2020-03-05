using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models;
using Bell.Models.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace Bell.ViewModels.Renew
{
    public class RenewDetailsViewModel
    {
        public Book LocalBook { get; set; }
    }
}
