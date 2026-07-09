using Peliculas.Models;
using Peliculas.Models.Movie;
using Peliculas.Models.MovieList;
using Peliculas.Models.Rating;
using Peliculas.Models.Review;
using Peliculas.Models.Role;
using Peliculas.Models.User;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Peliculas.Models.Genre;

namespace Peliculas.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }
        internal DbSet<Movie> Movies { get; set; }
        internal DbSet<Genre> Genres { get; set; }
        internal DbSet<Rating> Ratings { get; set; }
        internal DbSet<Review> Reviews { get; set; }
        internal DbSet<MovieList> MovieLists { get; set; }
        internal DbSet<MovieListItem> MovieListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );

            // User
            modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            // User → Roles (many to many)
            modelBuilder.Entity<User>()
                .HasMany(x => x.Roles)
                .WithMany()
                .UsingEntity(
                    "RoleUser",
                    l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                    r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId")
                );

            // Rating
            modelBuilder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();

            modelBuilder.Entity<Rating>()
                .Property(r => r.Score)
                .HasPrecision(3, 1);

            // Review
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique();

            // MovieListItem
            modelBuilder.Entity<MovieListItem>()
                .HasIndex(i => new { i.MovieListId, i.MovieId })
                .IsUnique();

            // Movies
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.Genres)
                .WithMany()
                .UsingEntity<MovieGenres>(
                    l => l.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId),
                    r => r.HasOne<Movie>().WithMany().HasForeignKey(x => x.MovieId)
                );
        }
    }
}