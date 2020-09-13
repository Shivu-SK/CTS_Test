using CreditTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CreditUtilisationTest
{
    [TestClass]
    public class CreditUtilisationUnitTest
    {
        [TestMethod]
        public void Test_ReadCsvFile()
        {
            CreditTree.Services.CreditDataService testCreditDataService = new CreditTree.Services.CreditDataService();
            List<CreditData> csvData =  testCreditDataService.ReadCSVFile(testCreditDataService.filePath);
            Assert.IsTrue(csvData.Count > 0);

        }

        [TestMethod]
        public void Test_CSV_Data_Should_Have_Parent_Entities()
        {
            CreditTree.Services.CreditDataService testCreditDataService = new CreditTree.Services.CreditDataService();
            List<CreditData> csvData = testCreditDataService.ReadCSVFile(testCreditDataService.filePath);
            List<CreditData> parentEntities = testCreditDataService.GetRootEntities(csvData);
            Assert.IsNotNull(parentEntities);
            Assert.IsTrue(parentEntities.Count > 0);
        }

        [TestMethod]
        public void Test_Parent_And_Its_Related_Entities()
        {
            CreditTree.Services.CreditDataService testCreditDataService = new CreditTree.Services.CreditDataService();
            List<CreditData> csvData = testCreditDataService.ReadCSVFile(testCreditDataService.filePath);
            List<CreditData> parentEntities = testCreditDataService.GetRootEntities(csvData);
            CreditData parentEntity = parentEntities[0];

            // Parent entity validation test
            Assert.IsNotNull(parentEntity);
            Assert.IsTrue(parentEntity.Limit > 0);
            Assert.IsTrue(string.IsNullOrEmpty(parentEntity.Parent));
            
            var relatedGroupEntities = testCreditDataService.GetRootAndItsRelatedEntities(csvData, parentEntity.Entity);
            Assert.IsNotNull(relatedGroupEntities);
            Assert.IsTrue(relatedGroupEntities.Count > 0);
        }

        [TestMethod]
        public void Test_GenerateReport_No_Limit_Breach()
        {
            CreditTree.Services.CreditDataService testCreditDataService = new CreditTree.Services.CreditDataService();
            List<CreditData> csvData = testCreditDataService.ReadCSVFile(testCreditDataService.filePath);
            List<CreditData> parentEntities = testCreditDataService.GetRootEntities(csvData);

            CreditData parentEntity = parentEntities[0];
            var relatedGroupEntities = testCreditDataService.GetRootAndItsRelatedEntities(csvData, parentEntity.Entity);
            
            Report report = testCreditDataService.GenerateReport(parentEntity, 0, relatedGroupEntities);
            
            Assert.AreEqual(report.Result, "No Limit Breaches");
        }

        [TestMethod]
        public void Test_GenerateReport_With_Limit_Breach()
        {
            CreditTree.Services.CreditDataService testCreditDataService = new CreditTree.Services.CreditDataService();
            List<CreditData> csvData = testCreditDataService.ReadCSVFile(testCreditDataService.filePath);
            List<CreditData> parentEntities = testCreditDataService.GetRootEntities(csvData);

            CreditData parentEntity = parentEntities[1];
            var relatedGroupEntities = testCreditDataService.GetRootAndItsRelatedEntities(csvData, parentEntity.Entity);

            Report report = testCreditDataService.GenerateReport(parentEntity, 0, relatedGroupEntities);

            Assert.AreNotEqual(report.Result, "No Limit Breaches");
        }
    }
}
