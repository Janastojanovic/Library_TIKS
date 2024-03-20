using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    [Table("Loans")]
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(MemberId))]
        public int MemberId { get; set; }

        [ForeignKey(nameof(BookId))]
        public int BookId { get; set; }
        public DateOnly LoanDate { get; set; }
        public bool Returned { get; set; } = false;
    }
}
