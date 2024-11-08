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


[ApiController]
[Route("/api/v1/[controller]")]
public class TweetsController: Controller
{
    private YDbContext _dbContext;
    private UserManager<AppUser> _userManager;
    private JwtService _jwtService;
    private ILogger<TweetsController> _logger;
    
    public TweetsController(YDbContext dbContext, UserManager<AppUser> userManager, JwtService jwtService, ILogger<TweetsController> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
        
    }

    private string? FetchUsernameFromRequestHeader(string token)
    {
        token = token.Replace("Bearer ", "");
        
        
        var tokenClaims = _jwtService.GetTokenClaims(token);

        return tokenClaims.FirstOrDefault(t => t.Type == "unique_name")?.Value;
    }

    private List<Tweet> GetUserTweets(string token)
    {
        string? username = FetchUsernameFromRequestHeader(token);

        if (String.IsNullOrEmpty(username))
            throw new MissingMemberException("Could not fetch username from token");
     
        
        AppUser? user = _dbContext.Users
            .Include(f => f.Following)
            .FirstOrDefault(u => u.UserName == username);
        
        if (user is null)
            throw new NullReferenceException($"User {username} does not exist");
        

        List<Tweet> allTweets = _dbContext.Tweets
            .Include(t=>t.Author)
            .Include(t=>t.Likes)
            .Include(t=>t.Comments)
            .Where(t=> !t.IsArchived && user.Following.Select(f=>f.UserName).Contains(t.Author.UserName))
            .ToList();


        return allTweets;
    }
    
    private List<Tweet> GetAnonTweets()
    {
        List<Tweet> anonTweets = _dbContext.Tweets
            .Include(t => t.Author)
            .Include(t => t.Likes)
            .Include(t => t.Comments)
            .Where(t => !t.IsArchived).ToList();

        return anonTweets;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTweets([FromQuery] int page)
    {
        
        //1. Get the current user's Appuser object so we can access their following list
        //2. Cross reference all the usernames in the following list and fetch only tweets by them

        string token = HttpContext.Request.Headers.Authorization.ToString();

        List<Tweet> allTweets = new List<Tweet>();

        try
        {
            
            if (!String.IsNullOrEmpty(token))
            {
                allTweets = GetUserTweets(token);
            }
            else
            {
                allTweets = GetAnonTweets();
            }
            



            foreach (var tweet in allTweets)
            {
                tweet.UserInfo = new UserDto()
                {

                    Username = tweet.Author.UserName, Email = tweet.Author.Email, DisplayName = tweet.Author.DisplayName
                };

                tweet.LikesByUsername.AddRange(tweet.Likes.Select(l => l.UserName)!);
                tweet.Comments = tweet.Comments.OrderByDescending(c => c.CommentedAt).ToList();

            }

            return Ok(allTweets);
        }
        catch (MissingMemberException missingEx)
        {

            _logger.LogError(missingEx.Message);
            _logger.LogError(missingEx.StackTrace);
            return BadRequest(missingEx.Message);
        }
        catch (NullReferenceException nullEx)
        {
            
            _logger.LogError(nullEx.Message);
            _logger.LogError(nullEx.StackTrace);
            return BadRequest(nullEx.Message);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            _logger.LogError(ex.StackTrace);
            return StatusCode(500, "Could not fetch tweets. Please try again!");
        }
       
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetTweetById(long id)
    {
        Tweet? tweetToFind = await _dbContext.Tweets.FirstOrDefaultAsync(t => t.Id == id);

        if (tweetToFind == null)
            return BadRequest($"Cannot find a tweet with the provided ID: {id}");


        tweetToFind.LikesByUsername.AddRange(tweetToFind.Likes.Select(l => l.UserName)!);
        return Ok(tweetToFind);
    }

    //Eager loading  - Fetches all relations and models/tables associated to a model anytime it's fetched by default
        
    //Lazy loading - Doesn't fetch any relation models/tables by default UNLESS the user tells to 
    
    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpPatch("like/{id}")]
    public async Task<IActionResult> TriggerLikeTweet(long id)
    {
        
        string? username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString()); // get the username from our JWT token

        if (String.IsNullOrEmpty(username))
            return BadRequest("Invalid username in token");

        try
        {
            Tweet? tweetToLike = await _dbContext.Tweets
                .Include(t => t.Likes)
                .FirstOrDefaultAsync(t => t.Id == id); // find the tweet we're going to 'like'

            if (tweetToLike is null)
                return BadRequest($"Cannot find a tweet with the provided ID: {id}");

            //fetch the user object
            AppUser? user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return BadRequest("User does not exist");



            //check if the user has already 'liked' the tweet, if so then 'unlike' it
            if (tweetToLike.Likes.FirstOrDefault(l => l.UserName == username) != null)
                tweetToLike.Likes.Remove(user);
            else
                tweetToLike.Likes.Add(user);

            _dbContext.Update(tweetToLike);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to like/unlike tweet");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            
            
            return StatusCode(500, "Could not like/unline tweet");
        }
    }

    [Authorize]
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

    [Authorize]
    [HttpPost("comments")]
    public async Task<IActionResult> AddCommentToTweet([FromBody] AddCommentDto addCommentDto)
    {

        try
        {
            
            //Fetch the current app user
            string username = FetchUsernameFromRequestHeader(HttpContext.Request.Headers.Authorization.ToString());

            if (String.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid username in token");
            }
            
            AppUser? commentUser = await _userManager.FindByNameAsync(username);

            if (commentUser == null)
            {
                return BadRequest("User does not exist in system");
            }
            
            // Fetch the tweet model

            Tweet? tweet = await _dbContext.Tweets.FirstOrDefaultAsync(t => t.Id == addCommentDto.tweetId);


            if (tweet == null)
            {
                return BadRequest($"Failed to find tweet with ID {addCommentDto.tweetId}");
            }
            
            Comment newComment = new Comment()
            {
                CommentedAt = DateTime.UtcNow,
                Content = addCommentDto.content,
                CommentedBy = commentUser,
            };
            
            tweet.Comments.Add(newComment);

            _dbContext.Comments.Add(newComment);
            
            _dbContext.Update(tweet);
            await _dbContext.SaveChangesAsync();
            
            return Created($"/api/v1/tweets/comments/{newComment.Id}",newComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            _logger.LogError(ex.StackTrace);
            return StatusCode(500, $"Failed to add comment to tweet {addCommentDto.tweetId}");
        }
    }
}