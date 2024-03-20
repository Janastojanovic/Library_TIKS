using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    [Table("Members")]
    public class Member
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; } = string.Empty;
        [Required]
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required]
        public int Age { get; set; } = 0;
        public DateOnly? MembershipExpiration { get; set; }
    }
}
