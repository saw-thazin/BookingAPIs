namespace CodeTest.Models
{
    public class Package
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int RequiredCredit { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
    }
}
