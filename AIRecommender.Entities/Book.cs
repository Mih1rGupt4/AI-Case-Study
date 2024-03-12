using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.Entities
{
    public class Book
    {
        public string BookTitle { get; set; }

        public string BookAuthor { get; set; }

        public string ISBN { get; set; }

        public string Publisher {  get; set; }

        public int YearOfPublication { get; set; }

        public string ImageUrlSmall { get; set; }

        public string ImageUrlMedium { get; set; }

        public string ImageUrlLarge { get; set; }

        public List<BookUserRating> UserRatings { get; set; }
    }
}
