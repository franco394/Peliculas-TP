using AutoMapper;
using Peliculas.Models.Genre;
using Peliculas.Models.Genre.DTO;
using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;
using Peliculas.Models.MovieList;
using Peliculas.Models.MovieList.Dto;
using Peliculas.Models.Rating;
using Peliculas.Models.Rating.Dto;
using Peliculas.Models.Review;
using Peliculas.Models.Review.Dto;
using Peliculas.Models.Role;
using Peliculas.Models.Role.Dto;
using Peliculas.Models.User;
using Peliculas.Models.User.Dto;

namespace Peliculas.Config
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //---------TIPOS
            CreateMap<string?, string>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<decimal?, decimal>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<List<int>?, List<int>>().ConvertUsing((src, dest) => src ?? dest);

            //---------MOVIES
            CreateMap<Movie, MovieDTO>().ForMember(
                    dest => dest.Genres,
                    opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName))
                );
            CreateMap<CreateMovieDTO, Movie>();
            CreateMap<UpdateMovieDTO, Movie>()
                .ForAllMembers(
                    config => config.Condition((_, _, value) => value != null)
                );

            //---------MOVIES LIST
            CreateMap<MovieList, MovieListDTO>();
            CreateMap<MovieList, MovieListItemDTO>();
            CreateMap<CreateListDTO, MovieList>();
            CreateMap<AddMovieToListDTO, MovieList>();

            //---------RATINGS
            CreateMap<Rating, RatingDTO>();
            CreateMap<UpsertRatingDTO, RatingDTO>();

            //---------REVIEWS
            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();

            //---------GENRES
            CreateMap<Genre, GenresDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<UpdateGenreDTO, Genre>()
                .ForAllMembers(
                    config => config.Condition(( _, _, value) => value != null)
                );

            //---------USERS
            CreateMap<User, UserDTO>()
                .ForMember(
                    dest => dest.Roles,
                    opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToList())
                );
            CreateMap<RegisterDTO, User>().ReverseMap();
            CreateMap<User, UpdateUserDTO>()
                .ForAllMembers(
                    config => config.Condition((_, _, value) => value != null)
                );
            //---------ROLES
            CreateMap<Role, RoleDTO>().ReverseMap();
        }
    }
}
