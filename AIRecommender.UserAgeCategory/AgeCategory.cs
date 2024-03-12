using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.UserAgeCategory
{
    public class AgeCategory : IAgeCategory
    {
        public AgeGroup Categorize(int age)
        {
            if(age >= 1 && age <= 16)
            {
                return AgeGroup.Teen;
            }
            else if(age >= 17 && age <= 30)
            {
                return AgeGroup.Young;
            }
            else if(age >= 31 && age <= 50)
            {
                return AgeGroup.Mid;
            }
            else if(age >= 51 && age <= 60)
            {
                return AgeGroup.Old;
            }
            else if(age >= 61 && age <= 100)
            {
                return AgeGroup.Senior;
            }
            else
            {
                return AgeGroup.Invalid;
            }
        }
    }
}
