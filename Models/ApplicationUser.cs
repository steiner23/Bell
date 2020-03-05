using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bell.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityRegion { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        
    }
}
