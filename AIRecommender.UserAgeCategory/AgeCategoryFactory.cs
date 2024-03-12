using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.UserAgeCategory
{
    public class AgeCategoryFactory
    {
        IAgeCategory ageCategory;

        protected AgeCategoryFactory() { }

        public static readonly AgeCategoryFactory Instance = new AgeCategoryFactory();

        public IAgeCategory GetAgeCategory()
        {
            string className = ConfigurationManager.AppSettings["AGECATEGORY"];
            // Reflextion
            Type theType = Type.GetType(className);
            return (IAgeCategory)Activator.CreateInstance(theType);
        }
    }
}
