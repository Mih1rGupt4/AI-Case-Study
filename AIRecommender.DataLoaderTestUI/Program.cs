using AIRecommender.DataLoader;
using AIRecommender.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmaoTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Loading(0);
                Loading(1);
                Loading(2);
                Loading(3);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"{ex.Message} : {ex.FileName} : Failed to load files");
            }
        }

        private static void Loading(int n)
        {
            Console.WriteLine($"Initiating Loader");
            CSVDataLoader dataLoader = new CSVDataLoader();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            BookDetails bookDetails = dataLoader.Load();
            Console.WriteLine($"Book: {bookDetails.Books[n].ISBN}");
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
            Console.WriteLine("Dataloaded");
        }
    }
}
