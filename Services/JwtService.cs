using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace YApi.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(List<Claim> claims)
    {
        return new JwtSecurityTokenHandler().WriteToken(GetToken(claims));
    }

    public SecurityToken GetToken(List<Claim> claims)
    {

        var key = _config["JWT:Key"];
        var issuer = _config["JWT:Issuer"];
        var audience = _config["JWT:Audience"];
        var expirationFromConfig = _config["JWT:ExpirationInMinutes"];

        double? expirationInMins = null;

        if (String.IsNullOrWhiteSpace(expirationFromConfig))
            throw new Exception("Failed to fetch expiration time from config file");
        try
        {
            expirationInMins = Double.Parse(expirationFromConfig);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            throw new Exception("Failed to convert expiration time to double");    
        }
        
        
        if(expirationInMins == null)
            throw new Exception("Failed to convert expiration time to double");
        
        if (String.IsNullOrWhiteSpace(key))
            throw new Exception("Failed to fetch key from config file");

        if (String.IsNullOrWhiteSpace(issuer))
            throw new Exception("Failed to fetch issuer from config file");

        if (String.IsNullOrWhiteSpace(audience))
            throw new Exception("Failed to fetch audience from config file");

        var authKey = Encoding.ASCII.GetBytes(key);

        /*var token = new JwtSecurityToken(issuer: issuer,
            audience: audience,
            expires: DateTime.Now.AddMinutes(expirationInMins.Value) ,
            claims: claims, 
            signingCredentials: new SigningCredentials(authKey,SecurityAlgorithms.HmacSha256) );*/

        var tokenDescription = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(expirationInMins.Value),
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(authKey),
                SecurityAlgorithms.HmacSha512Signature)
        };

        return new JwtSecurityTokenHandler().CreateToken(tokenDescription);
    }
}