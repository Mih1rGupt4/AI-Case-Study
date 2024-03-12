using AIRecommender.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommendation.DataService
{
    internal class MemDataCacher : IDataCacher
    {
        static Logger dataLoadLogger = LogManager.GetLogger("DataLoadLogger");
        public BookDetails GetData()
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains("bookdetails"))
            {
                dataLoadLogger.Info("\n\n--------\n\nData found in cache file. Returning cached data.\n\n--------\n\n");
                return memoryCache["bookdetails"] as BookDetails;
            }
            else
            {
                dataLoadLogger.Info("Data not found in cache.");
                return null;
            }
        }

        public void SetData(BookDetails bookDetails)
        {
            var memoryCache = MemoryCache.Default;

            var expiration = DateTimeOffset.UtcNow.AddMinutes(5);
            memoryCache.Add("bookdetails", bookDetails, expiration);
            dataLoadLogger.Info("\n--------\nMemory Caching Successfull\n--------\n");
        }
    }
}
