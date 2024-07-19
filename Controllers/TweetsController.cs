using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YApi.Data;
using YApi.DTOs;
using YApi.Models;
using YApi.Services;

namespace YApi.Controllers;


[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
public class TweetsController: Controller
{
    private YDbContext _dbContext;
    private UserManager<AppUser> _userManager;
    private JwtService _jwtService;
    
    public TweetsController(YDbContext dbContext, UserManager<AppUser> userManager, JwtService jwtService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    private string? FetchUsernameFromRequestHeader(string token)
    {
        token = token.Replace("Bearer ", "");
        var tokenClaims = _jwtService.GetTokenClaims(token);

        return tokenClaims.FirstOrDefault(t => t.Type == "unique_name")?.Value;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTweets([FromQuery] int page)
    {
        
        //1. Get the current user's Appuser object so we can access their following list
        //2. Cross reference all the usernames in the following list and fetch only tweets by them

        string? username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString());

        if (String.IsNullOrEmpty(username))
            return BadRequest("Could not fetch username from token");

        AppUser? user = _dbContext.Users
            .Include(f => f.Following)
            .FirstOrDefault(u => u.UserName == username);

        if (user is null)
            return BadRequest($"User {username} does not exist");
        

        List<Tweet> allTweets = _dbContext.Tweets
            .Include(t=>t.Author)
            .Where(t=> !t.IsArchived && user.Following.Select(f=>f.UserName).Contains(t.Author.UserName))
            .ToList();

        foreach (var tweet in allTweets)
        {
            tweet.UserInfo = new UserDto()
            {
                
                Username = tweet.Author.UserName, Email = tweet.Author.Email, DisplayName = tweet.Author.DisplayName
            };
        }
        return Ok(allTweets);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetTweetById(long id)
    {
        Tweet? tweetToFind = await _dbContext.Tweets.FirstOrDefaultAsync(t => t.Id == id);

        if (tweetToFind == null)
            return BadRequest($"Cannot find a tweet with the provided ID: {id}");


        return Ok(tweetToFind);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewTweet([FromBody] TweetDto tweetDto)
    {

        string? username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString());
        
        if(String.IsNullOrEmpty(username)) 
            return StatusCode(500,"Failed to find Bearer token or username in Header");

        AppUser? tweetAuthor = await _userManager.FindByNameAsync(username);

        if (tweetAuthor == null)
        {
            return BadRequest("Invalid username for tweet author");
        }
        
        var tweet = new Tweet { Content = tweetDto.Content, CreatedAt = DateTime.UtcNow, Author = tweetAuthor};

        try
        {
            _dbContext.Tweets.Add(tweet);
            await _dbContext.SaveChangesAsync();
            return Created($"/api/v1/tweets/{tweet.Id}", tweet);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(500, "Unable to create new tweet");
        }
        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditTweet(long id, [FromBody] TweetDto editedTweet)
    {
        Tweet? tweetToFind = await _dbContext.Tweets.FirstOrDefaultAsync(t => t.Id == id);
        
        if (tweetToFind == null)
            return BadRequest($"Cannot find a tweet with the provided ID: {id}");

        if (!String.IsNullOrEmpty(editedTweet.Content) && editedTweet.Content != tweetToFind.Content)
        {
            tweetToFind.Content = editedTweet.Content;
        }

        try
        {
            _dbContext.Tweets.Update(tweetToFind);
            await _dbContext.SaveChangesAsync();
            return Ok(tweetToFind);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(500, $"Unable to edit tweet: {id}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> HideTweet(long id)
    {
        Tweet? tweetToFind = await _dbContext.Tweets.FirstOrDefaultAsync(t => t.Id == id);
        
        if (tweetToFind == null)
            return BadRequest($"Cannot find a tweet with the provided ID: {id}");

        tweetToFind.IsArchived = true;
        
        try
        {
            _dbContext.Tweets.Update(tweetToFind);
            await _dbContext.SaveChangesAsync();
            return Ok(tweetToFind);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return StatusCode(500, $"Unable to archive tweet: {id}");
        }
    }
}