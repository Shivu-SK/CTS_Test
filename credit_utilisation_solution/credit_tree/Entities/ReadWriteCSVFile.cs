namespace CreditTree
{
    /// <summary>
    ///  Mapping credit data with csv data
    /// </summary>
    public class CreditData
    {
        public string Entity { get; set; }
        public string Parent { get; set; }
        public int Limit { get; set; }
        public int Utilisation { get; set; }
    }
}
