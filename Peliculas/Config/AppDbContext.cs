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

namespace Peliculas.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }
        internal DbSet<Movie> Movies { get; set; }
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

            // Seed películas
            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "El Padrino", Director = "Francis Ford Coppola", Year = 1972, Genre = "Drama", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Movie { Id = 2, Title = "Pulp Fiction", Director = "Quentin Tarantino", Year = 1994, Genre = "Crimen", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Movie { Id = 3, Title = "Parasite", Director = "Bong Joon-ho", Year = 2019, Genre = "Thriller", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}