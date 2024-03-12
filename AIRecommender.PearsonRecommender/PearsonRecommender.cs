using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIRecommender.Entities;
using NLog;

namespace AIRecommender.PearsonRecommender
{
    public class PearsonRecommender : IRecommender
    {
        //Logger logger = LogManager.GetLogger("Correlation");
        public double GetCorrelation(int[] baseData, int[] otherData)
        {
            // if inputs are null
            if (baseData == null || otherData == null)
            {
                throw new InvalidInputException("BaseData or OtherData  array is null");
            }

            // Prevent alteration to original data
            int[] baseArr = (int[])baseData.Clone();
            int[] otherArr = (int[])otherData.Clone();
            
            int baseLen = baseArr.Length;
            int otherLen = otherArr.Length;

            // if inputs are empty
            if (baseLen == 0)
            {
                return -1;
            }

            if (baseLen > otherLen)
            {
                Array.Resize(ref otherArr, baseLen);
            }

            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumXSquare = 0;
            double sumYSquare = 0;

            for (int i = 0; i < baseLen; i++)
            {
                if (baseArr[i] == 0 || otherArr[i] == 0)
                {
                    baseArr[i] += 1;
                    otherArr[i] += 1;
                }
                int x = baseArr[i];
                int y = otherArr[i];
                sumXY += x * y;
                sumXSquare += x * x;
                sumYSquare += y * y;
                sumX += x;
                sumY += y;
            }

            double numerator;
            double denominator;
            double correlation;

            numerator = (baseLen * sumXY - sumX * sumY);
            denominator = Math.Sqrt(
                    (baseLen * sumXSquare - sumX * sumX)
                    *
                    (baseLen * sumYSquare - sumY * sumY)
                );
            
            if (denominator == 0)
            {
                return 0;
            }
            
            correlation = numerator / denominator;
            //logger.Info($"base data : {string.Join(", ", baseData)}");
            //logger.Info($"other data : {string.Join(", ", otherData)}");
            //logger.Info($"corr: {correlation}");


            //logger.Info($"{string.Join(", ",baseData)}\n{string.Join(", ", otherData)}");
            //correlation = Math.Round(correlation, 6);
            //correlation = Math.Round(correlation, 9);

            return correlation;
        }
    }
}
