using CreditTree.Services;
using System;
using System.Collections.Generic;

namespace CreditTree
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialise the required variables and service
            var parentGroupLimit = 0;
            var parentGroupDirectCreditUtilisation = 0;
            var combinedGroupEntityCreditUtilisation = 0;
            var parentGroupEntities = string.Empty;
            
            var _creditDataService = new CreditDataService();

            // Read the CSV data
            var csvData = _creditDataService.ReadCSVFile(_creditDataService.filePath);

            // Get the Parent entities
            var rootEntities = _creditDataService.GetRootEntities(csvData);

            #region Calculate the parent and its realted entities credit utilisation
            List<string> entityNames = new List<string>();
            foreach (var rootEntity in rootEntities)
            {
                var relatedGroupEntities = _creditDataService.GetRootAndItsRelatedEntities(csvData, rootEntity.Entity);
                Report groupEntitiesUtilisationReport = _creditDataService.GenerateReport(rootEntity, 0, relatedGroupEntities);
                
                Console.WriteLine($"Entities: {groupEntitiesUtilisationReport.Entities}\n\n\t{groupEntitiesUtilisationReport.Result}\n");
                
                parentGroupLimit += rootEntity.Limit;
                parentGroupEntities += $"{rootEntity.Entity}/";
                parentGroupDirectCreditUtilisation += rootEntity.Utilisation;
                entityNames.Add(groupEntitiesUtilisationReport.Entities);
                combinedGroupEntityCreditUtilisation += groupEntitiesUtilisationReport.GroupEntityCreditUtilised;
            }
            string allEntities= string.Join("/", entityNames);
            #endregion



            #region Combined Parent and it's related entities
            CreditData combineddGrouEentityData = new CreditData()
            {
                Entity = parentGroupEntities,
                Limit = parentGroupLimit,
                Parent = parentGroupEntities,
                Utilisation = parentGroupDirectCreditUtilisation
            };

            //Display combined Parent and it's related entities credit utilisation result
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***************COMBINED GROUP ENTITIES REPORT***************");
            Report combinedEntitiesUtilisationReport = _creditDataService.GenerateReport(combineddGrouEentityData, combinedGroupEntityCreditUtilisation);
            
            Console.WriteLine($"Entities: {allEntities}\n\n\t{combinedEntitiesUtilisationReport.Result}\n");
            
            Console.ResetColor();
            #endregion
            Console.ReadLine();
        }

    }
}
