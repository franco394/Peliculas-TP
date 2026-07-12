using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Peliculas.Config;
using Peliculas.Models.Genre;
using Peliculas.Models.Role;
using Peliculas.Repositories;
using Peliculas.Services;
using Peliculas.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresá el token JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    options.OperationFilter<AuthOperationFilter>();
});

//DATABASE
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("devConnection"));
});

//SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEncoderService, EncoderService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<RatingService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<MovieListService>();

//REPOSITORIES
builder.Services.AddScoped<IRepository<Genre>, Repository<Genre>>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IMovieListRepository, MovieListRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();

//MAPPER
builder.Services.AddAutoMapper(config => { }, typeof(MappingConfig));
//JWT
string secret = builder.Configuration["Secrets:jwt"]
    ?? throw new Exception("invalid jwt secret");

builder.Services.AddAuthentication(options => { 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(secret);
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

//FILTER
builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
        );
        ResponseValidation validation = new ResponseValidation(errors);
        return new BadRequestObjectResult(validation);
    };
});

var app = builder.Build();

app.UseCors(option =>
{
    option.AllowAnyOrigin();
    option.AllowAnyMethod();
    option.AllowAnyHeader();
});

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
