using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.PearsonRecommender
{
    public class RecommendationEngineFactory
    {
        IRecommender _recommender;

        private RecommendationEngineFactory() { }

        public static readonly RecommendationEngineFactory Instance = new RecommendationEngineFactory();

        public IRecommender GetRecommender()
        {
            string className = ConfigurationManager.AppSettings["ENGINE"];
            // Reflextion
            Type theType = Type.GetType(className);
            return (IRecommender)Activator.CreateInstance(theType);
        }
    }
}
