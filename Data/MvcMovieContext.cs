using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<WF_SendMail> WF_SendMail { get; set; }
        public DbSet<Verify> Verify { get; set; }



    }
}