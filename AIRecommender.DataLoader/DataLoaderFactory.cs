using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.DataLoader
{
    public class DataLoaderFactory
    {
        IDataLoader dataLoader;
        
        protected DataLoaderFactory()
        {

        }

        public static readonly DataLoaderFactory Instance = new DataLoaderFactory();
        
        public IDataLoader GetDataLoader()
        {
            string className = ConfigurationManager.AppSettings["DATALOADER"];
            // Reflextion
            Type theType = Type.GetType(className);
            return (IDataLoader)Activator.CreateInstance(theType);
        }
    }
}
