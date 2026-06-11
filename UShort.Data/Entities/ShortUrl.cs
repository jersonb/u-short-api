using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UShort.Data.Entities;

[Table("short_url")]
[Index(nameof(Code), IsUnique = true)]
public class ShortUrl
{
    [Key]
    [Column("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [Column("short_url_id")]
    public Guid ShortUrlId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(20)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    [Column("url")]
    public string Url { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedtAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("user_id")]
    public int UshortUserId { get; set; } = default!;

    public UshortUser UshortUser { get; set; } = default!;
}