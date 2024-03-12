using AIRecommender.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIRecommender.PearsonRecommender.UnitTest
{
    /// <summary>
    /// Summary description for AIRecommenderPearsonEngineUnitTest
    /// </summary>
    [TestClass]
    public class AIRecommenderPearsonEngineUnitTest
    {
        PearsonRecommender p = null;
        [TestInitialize]
        public void Initialize()
        {
            p = new PearsonRecommender();
        }
        
        [TestMethod]
        public void GetCorrelationTest_WithValidInputs_ReturnsExpectedOutput()
        {
            double actual = p.GetCorrelation(new int[] { 20, 24, 17}, new int[] { 30, 20, 27 });
            double expected = -0.739853246;
            actual = Math.Round(actual, 9);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetCorrelationTest_WithBaseArrayLengthGreaterThanOtherArrayLength_ReturnsExpectedOutput()
        {
            double actual = p.GetCorrelation(new int[] { 10, 20, 30, 40 }, new int[] { 20, 30, 40 });
            double expected = -0.386661;
            actual = Math.Round(actual, 6);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetCorrelationTest_WithOtherArrayLengthGreaterThanBaseArrayLength_ReturnsExpectedOutput()
        {
            double actual = p.GetCorrelation(new int[] { 20, 24, 17 }, new int[] { 30, 20, 27, 1 });
            double expected = -0.739853246;
            actual = Math.Round(actual, 9);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidInputException))]
        public void GetCorrelationTest_WithNullArray_ThrowsException()
        {
            double actual = p.GetCorrelation(null, null);
            double expected = -0.739853;
            actual = Math.Round(actual, 6);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetCorrelationTest_WithEmptyBaseDataArray_ReturnsMinus1()
        {
            double actual = p.GetCorrelation(new int[] {}, new int[] { });
            Assert.AreEqual(-1, actual);
        }
    }
}
