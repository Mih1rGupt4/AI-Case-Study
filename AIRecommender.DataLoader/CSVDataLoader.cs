using AIRecommender.Entities;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using System.Runtime.Caching;
using System.Runtime.Remoting.Contexts;

namespace AIRecommender.DataLoader
{
    public class CSVDataLoader : IDataLoader
    {
        private static Logger logger = LogManager.GetLogger("DataLoadLogger");

        public BookDetails Load()
        {
            logger.Info("\n\n\nCSV Data Reading Initiated");
            BookDetails bookDetails = new BookDetails();


            string directoryPath = @"BX-CSV-Dump/";
            
            // If CSV Directory Not Found
            if (!Directory.Exists(directoryPath))
            {
                logger.Error("Directory containing CSV files not found");
                throw new DirectoryNotFoundException("Directory containing CSV files not found");
            }
            
            List<string> fileNames = Directory.GetFiles(directoryPath).Select(Path.GetFileName).ToList();

            //string[] fileNames = { "BX-Book-Ratings.csv", "BX-Books.csv", "BX-Users.csv" };


            // Parallelize file loading operations
            Parallel.ForEach(fileNames, fileName =>
            {
                string filePath = Path.Combine(directoryPath, fileName);
                if (!File.Exists(filePath))
                {
                    logger.Error($"File not found: {fileName}");
                    throw new FileNotFoundException($"File not found: {fileName}");
                }

                // load details based on file path
                try
                {
                    if (fileName == "BX-Users.csv")
                        bookDetails.Users = LoadUsers(filePath);
                    else if (fileName == "BX-Books.csv")
                        bookDetails.Books = LoadBooks(filePath);
                    else if (fileName == "BX-Book-Ratings.csv")
                        bookDetails.BookUserRatings = LoadBookUserRatings(filePath);
                }
                catch (FileNotFoundException ex)
                {
                    logger.Error($"{ex.Message} \nFailed to load {ex.FileName}");
                    throw new FileNotFoundException($"{ex.Message} \nFailed to load files");
                }
            });
            
            return bookDetails;
        }

        // While Reading If value does not exist according to the entities
        // it will store null for string values and 0 for integer values
        public List<User> LoadUsers(string BXUsersFilePath)
        {
            List<User> users = new List<User>();

            using (TextFieldParser parser = new TextFieldParser(BXUsersFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                parser.HasFieldsEnclosedInQuotes = true;

                // Skip header
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch (MalformedLineException ex)
                    {
                        logger.Warn($"File Path: {BXUsersFilePath}, Error: {ex.Message}");
                        continue;
                    }

                    if (fields.Length < 3)
                    {
                        logger.Warn("A row in User file does not have all field values");
                        logger.Info(string.Join("; ", fields));
                        continue;
                    }
                    User user = new User();
                    user.UserID = int.TryParse(fields[0].Trim(), out int userId) ? userId : 0;
                    string[] location = fields[1].Split(',');
                    user.City = location.Length > 0 ? (location[0].Contains("n/a") ? null : location[0].Trim()) : null;
                    user.State = location.Length > 1 ? (location[1].Contains("n/a") ? null : location[1].Trim()) : null;
                    user.City = location.Length > 2 ? (location[2].Contains("n/a") ? null : location[2].Trim()) : null;
                    user.Age = int.TryParse(fields[2].Trim(), out int age) ? age : 0;

                    users.Add(user);
                }
            }

            return users;
        }

        public List<Book> LoadBooks(string BXBooksFilePath) 
        {
            List<Book> books = new List<Book>();

            using (TextFieldParser parser = new TextFieldParser(BXBooksFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                parser.HasFieldsEnclosedInQuotes = true;

                // Skip header
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch (MalformedLineException ex)
                    {
                        logger.Warn($"File Path: {BXBooksFilePath}, Error: {ex.Message}");
                        continue;
                    }

                    if (fields.Length < 8)
                    {
                        logger.Warn("A row in Book file does not have all field values");
                        continue;
                    }
                    Book book = new Book
                    {
                        ISBN = fields[0].Contains("n/a") ? null : fields[0].Trim(),
                        BookTitle = fields[1].Contains("n/a") ? null : fields[1].Trim(),
                        BookAuthor = fields[2].Contains("n/a") ? null : fields[2].Trim(),
                        YearOfPublication = int.TryParse(fields[3].Trim(), out int year) ? year : 0,
                        Publisher = fields[4].Contains("n/a") ? null : fields[4].Trim(),
                        ImageUrlSmall = fields[5].Contains("n/a") ? null : fields[5].Trim(),
                        ImageUrlMedium = fields[6].Contains("n/a") ? null : fields[6].Trim(),
                        ImageUrlLarge = fields[7].Contains("n/a") ? null : fields[7].Trim()
                    };

                    books.Add(book);
                }
            }

            return books;
        }

        public List<BookUserRating> LoadBookUserRatings(string BXBooksRatingFilePath)
        {
            List<BookUserRating> bookUserRatings = new List<BookUserRating>();

            using (TextFieldParser parser = new TextFieldParser(BXBooksRatingFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                parser.HasFieldsEnclosedInQuotes = true;

                // Skip header
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields = null;
                    try
                    {
                        fields = parser.ReadFields();

                    }
                    catch (MalformedLineException ex)
                    {
                        logger.Warn($"File Path: {BXBooksRatingFilePath}, Error: {ex.Message}");
                        continue;
                    }

                    if (fields.Length < 3)
                    {
                        logger.Warn("A row in Book User Ratings file does not have all field values");
                    }
                    BookUserRating userRating = new BookUserRating
                    {
                        UserID = int.TryParse(fields[0].Trim(), out int userId) ? userId : 0,
                        ISBN = fields[1].Contains("n/a") ? null : fields[1].Trim(),
                        Rating = int.TryParse(fields[2].Trim(), out int rating) ? rating : 0
                    };

                    bookUserRatings.Add(userRating);
                }
            }

            return bookUserRatings;
        }
    }
}
