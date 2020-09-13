namespace CreditTree
{
    /// <summary>
    /// get set proprty class to generrate the each parent credit utilisation report
    /// </summary>
    public class Report
    {
        public string Entities { get; set; }
        public string Result { get; set; }
        public int GroupEntityCreditUtilised { get; set; }
    }
}
