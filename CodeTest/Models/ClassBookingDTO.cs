using System.ComponentModel.DataAnnotations;

namespace CodeTest.Models
{
    public class ClassBookingDTO
    {
        [Key]
        public int BookingId { get; set; }
        public int ClassId { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; }

        public bool IsBook { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
