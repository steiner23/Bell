using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Bell.Models.Books
{
    public class BookWithAvailability : Book
    {
        public Book Book { get; set; }
        public bool Available { get; set; }
    }
}
