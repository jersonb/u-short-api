using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShort.Data.Entities;

[Table("user")]
[Index(nameof(Email), IsUnique = true)]
public class UshortUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("registred_at")]
    public DateTimeOffset RegistredAt { get; set; } = DateTimeOffset.UtcNow;
}