using AIRecommender.DataLoader;
using AIRecommender.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommendation.DataService
{
    public class BooksDataService
    {

        public static BookDetails GetBookDetails()
        {
            MemDataCacher memDataCacher = new MemDataCacher();

            BookDetails bookDetails = memDataCacher.GetData();

            if (bookDetails == null)
            {
                DataLoaderFactory factory = DataLoaderFactory.Instance;
                bookDetails = factory.GetDataLoader().Load();
                memDataCacher.SetData(bookDetails);
            }

            return bookDetails;
        }

        //public static BookDetails GetBookDetails()
        //{
        //    RedisDataCacher dataCacher = new RedisDataCacher();

        //    BookDetails bookDetails = dataCacher.GetData();
        //    if ( bookDetails == null )
        //    {
        //        DataLoaderFactory factory = DataLoaderFactory.Instance;
        //        bookDetails = factory.GetDataLoader().Load();
        //    }

        //    return bookDetails;
        //}
    }
}
