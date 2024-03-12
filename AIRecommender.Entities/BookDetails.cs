using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.Entities
{
    public class BookDetails
    {
        public List<User> Users { get; set; } = new List<User>();

        public List<Book> Books { get; set; } = new List<Book>();

        public List<BookUserRating> BookUserRatings { get; set; } = new List<BookUserRating>();
    }
}
