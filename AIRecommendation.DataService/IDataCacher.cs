using AIRecommender.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRecommendation.DataService
{
    internal interface IDataCacher
    {
        BookDetails GetData();
        void SetData(BookDetails bookDetails);
    }
}
