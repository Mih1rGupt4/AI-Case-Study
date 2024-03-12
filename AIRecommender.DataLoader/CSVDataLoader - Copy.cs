//using AIRecommender.Entities;
//using Microsoft.VisualBasic.FileIO;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using NLog;
//using Newtonsoft.Json;

//namespace AIRecommender.DataLoader
//{
//    public class CSVDataLoader : IDataLoader
//    {
//        private static Logger logger = LogManager.GetCurrentClassLogger();
//        private static string cacheFilePath = "cachedBookDetails.json";

//        public BookDetails Load()
//        {
//            // Check if data is already cached in file
//            if (File.Exists(cacheFilePath))
//            {
//                logger.Info("Data found in cache file. Returning cached data.");

//                // Deserialize cached data from file
//                string json = File.ReadAllText(cacheFilePath);
//                return JsonConvert.DeserializeObject<BookDetails>(json);
//            }

//            logger.Info("\n\n\nCSV Data Reading Initiated");
//            BookDetails bookDetails = new BookDetails();


//            string directoryPath = @"BX-CSV-Dump/";
//            string[] fileNames = { "BX-Book-Ratings.csv", "BX-Books.csv", "BX-Users.csv" };

//            if (!Directory.Exists(directoryPath))
//            {
//                logger.Error("Directory containing CSV files not found");
//                throw new DirectoryNotFoundException("Directory containing CSV files not found");
//            }

//            // Parallelize file loading operations
//            Parallel.ForEach(fileNames, fileName =>
//            {
//                string filePath = Path.Combine(directoryPath, fileName);
//                if (!File.Exists(filePath))
//                {
//                    logger.Error($"File not found: {fileName}");
//                    throw new FileNotFoundException($"File not found: {fileName}");
//                }

//                try
//                {
//                    if (fileName == "BX-Users.csv")
//                        bookDetails.Users = LoadUsers(filePath);
//                    else if (fileName == "BX-Books.csv")
//                        bookDetails.Books = LoadBooks(filePath);
//                    else if (fileName == "BX-Book-Ratings.csv")
//                        bookDetails.BookUserRatings = LoadBookUserRatings(filePath);
//                }
//                catch (FileNotFoundException ex)
//                {
//                    logger.Error($"{ex.Message} \nFailed to load {ex.FileName}");
//                    throw new FileNotFoundException($"{ex.Message} \nFailed to load files");
//                }
//            });

//            string jsonCache = JsonConvert.SerializeObject(bookDetails);
//            File.WriteAllText(cacheFilePath, jsonCache);
//            logger.Info("CSV Data Loading successful");

//            return bookDetails;
//        }

//        // While Reading If value does not exist according to the entities
//        // it will store null for string values and 0 for integer values
//        public List<User> LoadUsers(string BXUsersFilePath)
//        {
//            List<User> users = new List<User>();

//            using (TextFieldParser parser = new TextFieldParser(BXUsersFilePath))
//            {
//                parser.TextFieldType = FieldType.Delimited;
//                parser.SetDelimiters(";");
//                parser.HasFieldsEnclosedInQuotes = true;

//                // Skip header
//                parser.ReadLine();

//                // Read data
//                while (!parser.EndOfData)
//                {
//                    string[] fields;
//                    try
//                    {
//                        fields = parser.ReadFields();
//                    }
//                    catch (MalformedLineException ex)
//                    {
//                        logger.Warn($"{ex.Message}");
//                        continue;
//                    }
//                    if (fields.Length < 3)
//                    {
//                        logger.Warn("A row in User file does not have all field values");
//                        logger.Info(string.Join("; ", fields));
//                        continue;
//                    }
//                    User user = new User();
//                    user.UserID = int.TryParse(fields[0], out int userId) ? userId : 0;
//                    string[] location = fields[1].Split(',');
//                    user.City = location.Length > 0 ? (location[0].Contains("n/a") ? null : location[0]) : null;
//                    user.State = location.Length > 1 ? (location[1].Contains("n/a") ? null : location[1]) : null;
//                    user.City = location.Length > 2 ? (location[2].Contains("n/a") ? null : location[2]) : null;
//                    user.Age = int.TryParse(fields[2], out int age) ? age : 0;

//                    users.Add(user);
//                }
//            }

//            return users;
//        }

//        public List<Book> LoadBooks(string BXBooksFilePath)
//        {
//            List<Book> books = new List<Book>();

//            using (TextFieldParser parser = new TextFieldParser(BXBooksFilePath))
//            {
//                parser.TextFieldType = FieldType.Delimited;
//                parser.SetDelimiters(";");
//                parser.HasFieldsEnclosedInQuotes = true;

//                // Skip header
//                parser.ReadLine();

//                // Read data
//                while (!parser.EndOfData)
//                {
//                    string[] fields;
//                    try
//                    {
//                        fields = parser.ReadFields();
//                    }
//                    catch (MalformedLineException ex)
//                    {
//                        logger.Warn($"{ex.Message}");
//                        continue;
//                    }
//                    if (fields.Length < 8)
//                    {
//                        logger.Warn("A row in Book file does not have all field values");
//                        continue;
//                    }
//                    Book book = new Book();
//                    book.ISBN = fields[0].Contains("n/a") ? null : fields[0];
//                    book.BookTitle = fields[1].Contains("n/a") ? null : fields[1];
//                    book.BookAuthor = fields[2].Contains("n/a") ? null : fields[2];
//                    book.YearOfPublication = int.TryParse(fields[3], out int year) ? year : 0;
//                    book.Publisher = fields[4].Contains("n/a") ? null : fields[4];
//                    book.ImageUrlSmall = fields[5].Contains("n/a") ? null : fields[5];
//                    book.ImageUrlMedium = fields[6].Contains("n/a") ? null : fields[6];
//                    book.ImageUrlLarge = fields[7].Contains("n/a") ? null : fields[7];

//                    books.Add(book);
//                }
//            }

//            return books;
//        }

//        public List<BookUserRating> LoadBookUserRatings(string BXBooksRatingFilePath)
//        {
//            List<BookUserRating> bookUserRatings = new List<BookUserRating>();

//            using (TextFieldParser parser = new TextFieldParser(BXBooksRatingFilePath))
//            {
//                parser.TextFieldType = FieldType.Delimited;
//                parser.SetDelimiters(";");
//                parser.HasFieldsEnclosedInQuotes = true;

//                // Skip header
//                parser.ReadLine();

//                // Read data
//                while (!parser.EndOfData)
//                {
//                    string[] fields = null;
//                    try
//                    {
//                        fields = parser.ReadFields();

//                    }
//                    catch (MalformedLineException ex)
//                    {
//                        logger.Warn($"{ex.Message}");
//                        continue;
//                    }
//                    if (fields.Length < 3)
//                    {
//                        logger.Warn("A row in Book User Ratings file does not have all field values");
//                    }
//                    BookUserRating userRating = new BookUserRating();
//                    userRating.UserID = int.TryParse(fields[0], out int userId) ? userId : 0;
//                    userRating.ISBN = fields[1].Contains("n/a") ? null : fields[1];
//                    userRating.Rating = int.TryParse(fields[2], out int rating) ? rating : 0;

//                    bookUserRatings.Add(userRating);
//                }
//            }

//            return bookUserRatings;
//        }
//    }
//}
