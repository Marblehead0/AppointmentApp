using System.ComponentModel.DataAnnotations;

namespace Chipsoft.Assignments.EPDConsole.Domain
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} ({Email})";
        }
    }
}
