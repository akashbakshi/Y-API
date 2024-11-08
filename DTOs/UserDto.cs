namespace YApi.DTOs;

public class UserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    
    public List<string> Followers { get; set; }
    
    public List<string> Following { get; set; }
}