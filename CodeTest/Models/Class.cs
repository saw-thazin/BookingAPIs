using Microsoft.Identity.Client;

namespace CodeTest.Models
{
    public class Class
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RequiredCredit { get; set; }
        public bool IsFull { get; set; }
        public bool? IsBook { get; set; }
        public int TotalNoOfAcceptance { get; set; }
        public int TotalCurrentAcceptance { get; set; }
        
    }
}
