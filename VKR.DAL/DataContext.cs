using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKR.DAL.Entities;

namespace VKR.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.
                Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.
                Entity<Avatar>().
                ToTable(nameof(Avatars));

            modelBuilder.
                Entity<PostContent>().
                ToTable(nameof(Contents));

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("VKR.API"));

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> Sessions => Set<UserSession>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostContent> Contents => Set<PostContent>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
    }
}
