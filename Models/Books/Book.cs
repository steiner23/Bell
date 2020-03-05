using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Bell.Models.Loans;


namespace Bell.Models.Books
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string BookIdent { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Author { get; set; }

        public string Description { get; set; }

        public string BookCoverThumbnailUrl { get; set; }
    }
}
