using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using YApi.DTOs;

namespace YApi.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    
    public string Content { get; set; }
    
    [JsonIgnore]
    public virtual AppUser CommentedBy { get; set; }
    
    [NotMapped]
    public UserDto CommentedByUser {
        get
        {
            return new UserDto()
            {
                DisplayName = CommentedBy.DisplayName, Email = CommentedBy.Email, Followers = new List<string>(),
                Following = new List<string>(), Username = CommentedBy.UserName
            };
        }
        set
        {
            CommentedByUser = value;  
        } 
        
    }
    
    public DateTime CommentedAt { get; set; }
    
    
}