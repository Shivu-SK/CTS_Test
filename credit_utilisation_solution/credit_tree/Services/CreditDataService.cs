using CreditTree.Mappers;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CreditTree.Services
{
    public class CreditDataService
    {
        public string currentDirectory;
        public string filePath;

        /// <summary>
        /// Constructor method
        /// </summary>
        public CreditDataService()
        {
            currentDirectory = Directory.GetCurrentDirectory().Split("bin")[0];
            filePath = Path.Combine(currentDirectory, "Data", "credit_data.csv");
        }

        /// <summary>
        /// Method to read the CSV file
        /// </summary>
        /// <param name="location">Location of the file</param>
        /// <returns></returns>
        public List <CreditData> ReadCSVFile(string location)
        {
            try
            {
                using (var reader = new StreamReader(location, Encoding.Default))
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csv.Configuration.RegisterClassMap<CreditDataMap>();
                    var records = csv.GetRecords<CreditData>().ToList();
                    return records;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method to get the Parent entities
        /// </summary>
        /// <param name="creditData">Collection of the entities data</param>
        /// <returns></returns>
        public List<CreditData> GetRootEntities(List <CreditData> creditData)
        {
            return creditData.Where(data => string.IsNullOrEmpty(data.Parent))
                                    .Select(data => data)
                                    .ToList();
        }

        /// <summary>
        /// Method to get root(parent) and its related entities
        /// </summary>
        /// <param name="csvData">List of entity data</param>
        /// <param name="rootEntity">Parent entity name</param>
        /// <returns></returns>
        public List<CreditData> GetRootAndItsRelatedEntities(List<CreditData> csvData, string rootEntity)
        {
            var relatedEntities = GetRelatedChildEntities(rootEntity, csvData);
            if (relatedEntities.Count < 0)
                return null;

            List<CreditData> groupedEntities = new List<CreditData>();
            foreach (var entity in relatedEntities)
            {
                groupedEntities.Add(entity);
            }
            return groupedEntities;

        }

        /// <summary>
        /// Method to generate the credit utilisation report
        /// </summary>
        /// <param name="rootEntityData">Collection of parent and it's related entities</param>
        /// <param name="parentEntity">Parent entity data</param>
        /// <param name="creditUtilised">Initial credit utilisation</param>
        /// <returns></returns>
        public Report GenerateReport(CreditData parentEntity, int creditUtilised, List<CreditData> realtedGroupEntities = null)
        {
            Report report = new Report();
            List<string> entityNames = new List<string>();
            if(realtedGroupEntities != null)
            {
                foreach (var entity in realtedGroupEntities)
                {
                    creditUtilised += entity.Utilisation;
                    entityNames.Add(entity.Entity);                   
                    report.GroupEntityCreditUtilised = creditUtilised;
                }
                entityNames.Sort();
                report.Entities = string.Join("/", entityNames);
            }
            
            if (parentEntity.Limit >= creditUtilised)
                report.Result = $"No Limit Breaches";
            else
                report.Result = $"Limit Breaches at {parentEntity.Entity} (limit = {parentEntity.Limit}, direct utilisation = {parentEntity.Utilisation}, combined utilisation = {creditUtilised}).";
            
            return report;
        }

        /// <summary>
        /// Method to retrive all the childrens from it parent entity name
        /// </summary>
        /// <param name="enitity">Refrence entity name</param>
        /// <param name="creditData">CSV file data</param>
        /// <returns></returns>
        private List<CreditData> GetChildrens(string enitity, List<CreditData> creditData)
        {
            return creditData.Where(d => d.Parent == enitity)
                             .Select(d => d)
                             .ToList();
        }

        /// <summary>
        /// Method to get the collection of parent and its entities
        /// </summary>
        /// <param name="rootEntity">Parent entity name</param>
        /// <param name="csvData">CSV file Data</param>
        /// <returns></returns>
        private List<CreditData> GetRelatedChildEntities(string rootEntity, List<CreditData> csvData)
        {
            List<CreditData> rootAndItsRelatedEntities = new List<CreditData>();
            var childrenEntity = GetChildren(rootEntity, csvData);
            rootAndItsRelatedEntities.Add(childrenEntity);
            List<CreditData> childrenEntities = GetChildrens(childrenEntity.Entity, csvData);

            if (childrenEntities.Count == 0)
                return rootAndItsRelatedEntities;
            childrenEntities.AddRange(childrenEntities);

            foreach (var data in childrenEntities)
            {
                List<CreditData> tempList = GetRelatedChildEntities(data.Entity, csvData);
                if (tempList.Count > 0)
                    rootAndItsRelatedEntities.AddRange(tempList);
            }
            return GetUniqueEntities(rootAndItsRelatedEntities);
        }

        /// <summary>
        /// Method to get the realted entity from the collection of entities
        /// </summary>
        /// <param name="enitity">Entity Name</param>
        /// <param name="creditData">CSV file data</param>
        /// <returns></returns>
        private CreditData GetChildren(string enitity, List<CreditData> creditData)
        {
            return creditData.FirstOrDefault(d => d.Entity == enitity);
                             
        }

        /// <summary>
        /// Get Unique entities from the collection of all related entities
        /// </summary>
        /// <param name="creditData">Collection of entities</param>
        /// <returns></returns>
        private List<CreditData> GetUniqueEntities(List<CreditData> creditData)
        {
            return creditData.GroupBy(enitity => enitity.Entity)
                             .Select(entity => entity.First())
                             .ToList();
        }
    }
}
