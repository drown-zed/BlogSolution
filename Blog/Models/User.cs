using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(200)]
        public string Nickname { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [MaxLength(200)]
        public string Email { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [MaxLength(200)]
        public string Gender { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
