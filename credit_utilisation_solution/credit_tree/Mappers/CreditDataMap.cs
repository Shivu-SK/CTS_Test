using CsvHelper.Configuration;

namespace CreditTree.Mappers
{
    /// <summary>
    /// Class to map the data filed name with the properties
    /// </summary>
    public sealed class CreditDataMap: ClassMap <CreditData>
    {
        public CreditDataMap()
        {
            Map(x => x.Entity).Name("entity");
            Map(x => x.Parent).Name("parent");
            Map(x => x.Limit).Name("limit");
            Map(x => x.Utilisation).Name("utilisation");
        }

    }
    
}
