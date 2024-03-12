using AIRecommender.DataAggrigator;
using AIRecommender.Entities;
using AIRecommender.PearsonRecommender;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using AIRecommendation.DataService;
using System.Collections.Concurrent;
using System;

namespace AIRecommendation.CoreEngine
{
    public class AIRecommendationEngine
    {
        
        static Logger timeLogger = LogManager.GetLogger("TimeLogger");
        //static Logger test = LogManager.GetLogger("Test");
        public static List<Book> Recommend(Preference preference, int limit)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Check if any argument is invalid
            if (preference == null || preference.ISBN == null)
            {
                throw new InvalidInputException("Preference Input is null");
            }
            if (limit < 0)
            {
                throw new InvalidInputException("Recommendation Limit is invalid");
            }

            // Load Books Data 
            BookDetails bookDetails = BooksDataService.GetBookDetails();
            if (bookDetails == null)
            {
                throw new InvalidInputException("Book Details is null");
            }

            timeLogger.Info($"Data Load successfull in : {sw.ElapsedMilliseconds}");
            sw.Restart();

            // Aggregate Ratings based on User Preference
            IRatingsAggregator ratingAggregator = RatingsAggregator.GetInstance();

            Dictionary<string, List<int>> ratingsList = ratingAggregator.Aggregate(bookDetails, preference);
            timeLogger.Info($"Data Aggregation Successful in: {sw.ElapsedMilliseconds}");
            sw.Restart();
            
            List<Book> recommendedBooks = GetRecommendations(preference, limit, bookDetails, ratingsList);

            timeLogger.Info($"Recommendations Generated in : {sw.ElapsedMilliseconds}");
            sw.Restart();

            return recommendedBooks;
        }

        private static List<Book> GetRecommendations(Preference preference, int limit, BookDetails bookDetails, Dictionary<string, List<int>> ratingsList)
        {
            // Finally processing the aggregated ratings based on preffered book

            ConcurrentBag<CustomBook> customBookList = GenerateRecommendationsList(bookDetails, ratingsList, preference, limit);

            // Sort the Books based on positive coefficients
            List<Book> recommendedBooks = GetFirstNRecommendations(customBookList, limit);
            return recommendedBooks;
        }

        private static ConcurrentBag<CustomBook> GenerateRecommendationsList(BookDetails bookdetails, Dictionary<string, List<int>> ratingslist, Preference preference, int limit)
        {
            // preprocess ratingslist to a dictionary for faster lookup
            Dictionary<string, int[]> ratingsdictionary = ratingslist
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
            
            // Get Recommendation Engine dynamically 
            RecommendationEngineFactory factory = RecommendationEngineFactory.Instance;
            IRecommender recommender = factory.GetRecommender();

            // store ratings for the preferred book
            int[] preferredbookratings = bookdetails.BookUserRatings
                .Where(rating => rating.ISBN.Equals(preference.ISBN))
                .Select(rating => rating.Rating)
                .ToArray();

            ConcurrentBag<CustomBook> custombooklist = new ConcurrentBag<CustomBook>();

            // parallelize the loop that calculates pearson coefficients
            Parallel.ForEach(bookdetails.Books, book =>
            {
                if (!book.ISBN.Equals(preference.ISBN))
                {
                    if (ratingsdictionary.TryGetValue(book.ISBN,
                        out int[] currentbookratings) && currentbookratings != null)
                    {
                        // calculate pearson coefficient
                        double coefficient = recommender.GetCorrelation(preferredbookratings, currentbookratings);
                        
                        //test.info($@"isbn: {book.isbn} coefficient: {coefficient}");

                        custombooklist.Add(new CustomBook { Book = book, Coefficient = coefficient });
                    }
                }
            });

            return custombooklist;
        }

        private static List<Book> GetFirstNRecommendations(ConcurrentBag<CustomBook> customBookList, int limit)
        {
            // Filter objects with positive double values
            List<CustomBook> filteredList = customBookList.Where(item => item.Coefficient > 0).ToList();

            // Sort the filtered list based on the Coefficient
            filteredList.Sort((x, y) => y.Coefficient.CompareTo(x.Coefficient));

            // Take the first n elements
            List<CustomBook> result = filteredList.Take(limit).ToList();

            // Extract Book instances
            List<Book> objectList = result.Select(item => item.Book).ToList();

            return objectList;
        }

    }
    public class CustomBook
    {
        public double Coefficient { get; set; }
        public Book Book { get; set; }
    }
}
