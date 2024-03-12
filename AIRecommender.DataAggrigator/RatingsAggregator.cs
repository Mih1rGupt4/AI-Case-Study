using AIRecommender.Entities;
using AIRecommender.UserAgeCategory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AIRecommender.DataAggrigator
{
    public class RatingsAggregator : IRatingsAggregator
    {
        private static readonly RatingsAggregator Instance = new RatingsAggregator();
        private IAgeCategory ageCategory;

        private RatingsAggregator()
        {
            AgeCategoryFactory factory = AgeCategoryFactory.Instance;
            ageCategory = factory.GetAgeCategory();
        }

        public static RatingsAggregator GetInstance()
        {
            return Instance;
        }

        public Dictionary<string, List<int>> Aggregate(BookDetails bookDetails, Preference preference)
        {
            // Get age group of preferred age
            AgeGroup preferredAgeGroup = ageCategory.Categorize(preference.Age);

            // If preferred age is invalid
            if (preferredAgeGroup == AgeGroup.Invalid)
            {
                throw new InvalidInputException($"Preferred Age: {preference.Age} is Invalid");
            }

            // Using HashSet for quicker lookup
            HashSet<int> preferredUserIds = new HashSet<int>(bookDetails.Users
                .Where(user => ageCategory.Categorize(user.Age) == preferredAgeGroup && 
                user.State != null && user.State.Contains(preference.State))
                .Select(user => user.UserID));

            // Extract all ratings given by preffered users
            // Grouped by ISBN
            var ratingsByISBN = from rating in bookDetails.BookUserRatings
                                where preferredUserIds.Contains(rating.UserID)
                                group rating.Rating by rating.ISBN into g
                                select new { ISBN = g.Key, Ratings = g.ToList() };
           
            // Convert the result into a dictionary
            Dictionary<string, List<int>> dict = ratingsByISBN.ToDictionary(x => x.ISBN, x => x.Ratings);
            
            // If there are no ratings with preferred state and age group
            if (dict.Count == 0)
            {
                throw new InvalidInputException($"No users have the same preference as the user");
            }

            return dict;

            // Grouping BookUserRatings by ISBN
            // UNDO var ratingsByISBN = bookDetails.BookUserRatings.GroupBy(rating => rating.ISBN);

            // Parallel processing of groups
            //var parallelResult = ratingsByISBN.AsParallel().Select(group =>
            //{
            //    List<int> ratings = new List<int>();

            //    foreach (BookUserRating rating in group)
            //    {
            //        if (preferredUserIds.Contains(rating.UserID))
            //        {
            //            ratings.Add(rating.Rating);
            //        }
            //    }

            //    return new { ISBN = group.Key, Ratings = ratings };
            //});

            
            //Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            //foreach (var group in ratingsByISBN)
            //{
            //    if (!dict.TryGetValue(group.Key, out List<int> ratings))
            //    {
            //        ratings = new List<int>();
            //        dict[group.Key] = ratings;
            //    }

            //    foreach (BookUserRating rating in group)
            //    {
            //        if (preferredUserIds.Contains(rating.UserID))
            //        {
            //            ratings.Add(rating.Rating);
            //        }
            //    }
            //}

        }
    }
}
