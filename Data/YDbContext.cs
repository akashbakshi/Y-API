using Microsoft.EntityFrameworkCore;
using YApi.Models;

namespace YApi.Data;

public class YDbContext : DbContext
{
    public YDbContext(DbContextOptions<YDbContext> options) : base(options)
    {

    }

    public DbSet<Tweet> Tweets { get; set; }
}
