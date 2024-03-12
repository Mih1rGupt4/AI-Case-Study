using AIRecommendation.CoreEngine;
using AIRecommender.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.UIClient
{
    internal class Program
    {
        static Logger timeLogger = LogManager.GetLogger("TimeLogger");

        static void Main(string[] args)
        {
            Preference preference1 = new Preference
            {
                //"0449223612"; "n is for noose"; 
                ISBN = "0449223612",
                Age = 30,
                State = "arizona",
            };

            Preference preference2 = new Preference
            {
                // book isbn doesnt exist
                ISBN = "033390804X",
                Age = 30,
                State = "barcelona",
            };
            Preference preference3 = new Preference
            {
                //"0771099886"; "A Jest of God";
                ISBN = "0771099886",
                Age = 18,
                State = "new york",
            };
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            GetRecommendedBooks(preference1);
            GetRecommendedBooks(preference2);
            GetRecommendedBooks(preference3);

            timeLogger.Info($"Total Time taken : {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
        }

        private static void GetRecommendedBooks(Preference preference)
        {
            Console.WriteLine($"Preffered Book \nISBN : {preference.ISBN}");
            Console.WriteLine($"Age: {preference.Age}");
            Console.WriteLine($"State: {preference.State}\n");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                List<Book> recommenedBooks = AIRecommendationEngine.Recommend(preference, 10);

                timeLogger.Info($"Time taken : {stopwatch.ElapsedMilliseconds}");
                stopwatch.Stop();

                Console.Out.WriteLine("\nBook count: " + recommenedBooks.Count);

                foreach (Book book in recommenedBooks)
                {
                    Console.WriteLine($"Title : {book.BookTitle}");
                }
                //foreach (Book book in recommenedBooks)
                //{
                //    Console.WriteLine($"\nTitle : {book.BookTitle}"
                //        + $"\nAuthor : {book.BookAuthor} " +
                //        $"\nISBN: {book.ISBN} " +
                //        $"\nPublisher: {book.Publisher} " +
                //        $"\nRatings: {book.UserRatings} " +
                //        $"\nYear: {book.YearOfPublication}");
                //}
            }
            catch (InvalidInputException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidAgeException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("------------------");
        }
    }
}
