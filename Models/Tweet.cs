using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YApi.DTOs;

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
    
    [JsonIgnore]
    public virtual AppUser Author { get; set; }
    
    [NotMapped]
    public UserDto UserInfo { get; set ; }
    
    public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    
    [JsonIgnore]
    public virtual List<AppUser> Likes { get; set; } = new List<AppUser>();


    [NotMapped] 
    public List<String> LikesByUsername { get; set; } = new List<string>();


}