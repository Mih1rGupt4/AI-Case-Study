using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommender.UserAgeCategory
{
    public interface IAgeCategory
    {
        AgeGroup Categorize(int age);
    }
}
