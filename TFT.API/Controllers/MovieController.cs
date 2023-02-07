using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.UriParser;
using System.Security.Claims;
using TFT.API.Business.Model;
using TFT.API.Security;

namespace TFT.API.Controllers
{
    public class MovieController : Controller
    {
        public MovieController(IConfiguration appConfig, Entities entites, IDataProtector protector)
        {
            _appConfig = appConfig;
            _entites = entites;
            _protector = protector;
        }

        private IConfiguration _appConfig;
        private Entities _entites;
        private IDataProtector _protector;

        [HttpGet]
        public IActionResult GetGenreID(String Name)
        {
            Genre genre = _entites.Genres.FirstOrDefault(g => g.Name == Name);
            if (genre == null)
            {
                return BadRequest();
            }

            return Ok(genre);
        }

        [HttpGet]
        public IActionResult GetDirectorID(String Username)
        {
            Director director = _entites.Directors.FirstOrDefault(d => d.Username == Username);
            if (director == null)
            {
                return BadRequest();
            }

            return Ok(director);
        }

        [HttpPost]
        public IActionResult CreateMovie()
        {
            if (new[] {
                nameof(Movie.Title) , nameof(Movie.Budget) , nameof(Movie.Description), 
                nameof(Movie.Duration),nameof(Movie.StartProduction) , nameof(Movie.EndProduction) , 
                nameof(Director),nameof(GenreMovie)
             }.All(k => Request.Form.ContainsKey(k)) && Request.Headers.ContainsKey("Authorization"))
            {
                String formattedToken = Request.Headers["Authorization"];
                if (formattedToken.StartsWith("Bearer "))
                {
                    String token = formattedToken.Replace("Bearer", "").TrimStart();
                    if (String.IsNullOrEmpty(token) == false)
                    {
                        IEnumerable<Claim> claims = AuthenticationController.GetClaimsFromJWT(token);
                        String roleName = AuthenticationController.GetClaimByName(claims, nameof(Business.Model.User.Role));
                        if (roleName != "Admin")
                        {
                            return BadRequest("Korisnik nije administrator.");
                        }
                    }
                }

                Director director = _entites.Directors.FirstOrDefault(d => d.Username == Request.Form[nameof(Director)].FirstOrDefault());
                if (director != null)
                {

                    List<GenreMovie> genreList =
                        Request.Form[nameof(GenreMovie)]
                        .Join(
                        _entites.Genres,
                        (outer) => outer,
                        (inner) => inner.Name,
                        (outer, inner) => inner
                        )
                        .Select(g => new GenreMovie() { Genres = g, Genres_ID = g.ID }).ToList();


                    Movie movie = new Movie()
                    {
                        Title = Request.Form[nameof(Movie.Title)],
                        Budget = decimal.Parse(Request.Form[nameof(Movie.Budget)]),
                        Description = Request.Form[nameof(Movie.Description)],
                        Duration = DateTimeOffset.Parse(Request.Form[nameof(Movie.Duration)]),
                        StartProduction = DateTime.Parse(Request.Form[nameof(Movie.StartProduction)]),
                        EndProduction = DateTime.Parse(Request.Form[nameof(Movie.EndProduction)]),
                        Director = director,
                        DirectorID = director.ID,
                        GenreMovies = genreList
                    };

                    

                    _entites.Movies.Add(movie);

                    try
                    {
                        _entites.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        _entites.ChangeTracker.Clear();
                        String msg = ex.Message;
                        Console.WriteLine(msg);
                        Console.WriteLine(ex.StackTrace);
                        return StatusCode(Response.StatusCode, msg);
                    }
                }
                else
                    return BadRequest("Nepoznati direktor kod stvaranja filma.");


                return Ok();
            }

                return BadRequest("Neka ulazna polja nedostaju kod kreiranja filma ili korisnik nije administrator.");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
