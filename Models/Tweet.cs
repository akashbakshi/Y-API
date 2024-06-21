using System.ComponentModel.DataAnnotations;

namespace YApi.Models;

public class Tweet
{
    [Required]
    [Key]
    public long Id { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsArchived { get; set; }
}