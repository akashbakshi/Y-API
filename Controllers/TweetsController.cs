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

    [HttpGet]
    public async Task<IActionResult> GetAllTweets([FromQuery] int page)
    {

        List<Tweet> allTweets = _dbContext.Tweets.Where(t=> !t.IsArchived).Include(t=>t.Author).ToList();

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

        var token = HttpContext.Request.Headers.Authorization.ToString();

        token = token.Replace("Bearer ", "");
        var tokenClaims = _jwtService.GetTokenClaims(token);

        var username = tokenClaims.FirstOrDefault(t => t.Type == "unique_name");

        if (username == null)
            return BadRequest("No username found in token");
        
        AppUser? tweetAuthor = await _userManager.FindByNameAsync(username.Value);

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