using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIRecommender.Entities;
namespace AIRecommender.DataLoader
{
    public interface IDataLoader
    {
        BookDetails Load();
    }
}
