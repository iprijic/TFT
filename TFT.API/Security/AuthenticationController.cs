using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TFT.API.Business.Model;

namespace TFT.API.Security
{
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IConfiguration appConfig, Entities entites, IDataProtector protector)
        {
            _appConfig = appConfig;
            _entites = entites;
            _protector = protector;
        }

        private IConfiguration _appConfig;
        private Entities _entites;
        private IDataProtector _protector;

        public static IEnumerable<Claim> GetClaimsFromJWT(String token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return securityToken.Claims;
        }

        public static ClaimsPrincipal GetIdentity(IEnumerable<Claim> claims) => new ClaimsPrincipal(new ClaimsIdentity(claims));
        public static String? GetClaimByName(IEnumerable<Claim> claims, String claimName) => claims.FirstOrDefault(c => c.Type == claimName)?.Value;


        private KeyValuePair<String, String> Hashing(String credential, String salt)
        {
            Byte[] spice = new byte[128 / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(spice);
            }

            spice = String.IsNullOrEmpty(salt) ? spice : Convert.FromBase64String(salt);

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            String hashed = Convert.ToBase64String(Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
                password: credential,
                salt: spice,
                prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8)
            );

            return new KeyValuePair<String, String>(hashed, Convert.ToBase64String(spice));
        }

        private IActionResult UserIsNotUnique(String roleName)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

            ODataError error = new ODataError()
            {
                ErrorCode = Response.StatusCode.ToString(),
                Message = $"This user ({roleName}) already exists.",
                Target = "API"
            };

            return BadRequest(error);
        }

        private IActionResult CreateSubEntity(String roleName, KeyValuePair<String, String> saltedHash)
        {

            ODataError error = new ODataError()
            {
                ErrorCode = Response.StatusCode.ToString(),
                Message = "Ne može se kreirati novi zapis za glumca ili direktora zbog toga što nedostaju neka ulazna obvezna polja",
                Target = "API"
            };

            if (nameof(Actor) == roleName && new[] { nameof(Actor.ActorID) }.Intersect(Request.Form.Keys).Any())
            {
                Actor actor = new Actor()
                {
                    Username = Request.Form[nameof(Business.Model.User.Username)],
                    Firstname = Request.Form[nameof(Business.Model.User.Firstname)],
                    Lastname = Request.Form[nameof(Business.Model.User.Lastname)],
                    Role = roleName,
                    Hash = saltedHash.Key,
                    Salt = saltedHash.Value,
                    ActorID= Request.Form[nameof(Actor.ActorID)]
                };

                if(_entites.Actors.Any(a => a.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault()))
                {
                    return UserIsNotUnique(roleName);
                }

                _entites.Add(actor);
                error = null;
            }

            if (nameof(Director) == roleName && new[] { nameof(Director.DirectorID) }.Intersect(Request.Form.Keys).Any())
            {
                Director director = new Director()
                {
                    Username = Request.Form[nameof(Business.Model.User.Username)],
                    Firstname = Request.Form[nameof(Business.Model.User.Firstname)],
                    Lastname = Request.Form[nameof(Business.Model.User.Lastname)],
                    Role = roleName,
                    Hash = saltedHash.Key,
                    Salt = saltedHash.Value,
                    DirectorID = Request.Form[nameof(Director.DirectorID)]
                };

                if (_entites.Directors.Any(a => a.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault()))
                {
                    return UserIsNotUnique(roleName);
                }

                _entites.Add(director);
                error = null;
            }

            if (error != null)
            {
                //Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return BadRequest(error);
            }

            try
            {
                _entites.SaveChanges();
            }
            catch (Exception ex)
            {
                _entites.ChangeTracker.Clear();
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

                error = new ODataError()
                {
                    ErrorCode = Response.StatusCode.ToString(),
                    Message = ex.Message,
                    Target = "API"
                };

                return StatusCode(Response.StatusCode, error);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult RegisterUser()
        {
             if (Request.Form.Keys.Intersect(new[] {
                nameof(Business.Model.User.Username) , nameof(Business.Model.User.Firstname) , nameof(Business.Model.User.Lastname),
                "PasswordInitial" , "PasswordConfirmed"
            }).Any())
            {

                if (String.IsNullOrEmpty(Request.Form["PasswordInitial"]) || String.IsNullOrEmpty(Request.Form["PasswordConfirmed"]))
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

                    ODataError error = new ODataError()
                    {
                        ErrorCode = Response.StatusCode.ToString(),
                        Message = "Initial and confirmed password cannot be empty or null string. Please try again.",
                        Target = "API"
                    };

                    return BadRequest(error);
                }

                if (Request.Form["PasswordInitial"] != Request.Form["PasswordConfirmed"])
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;

                    ODataError error = new ODataError()
                    {
                        ErrorCode = Response.StatusCode.ToString(),
                        Message = "Initial and confirmed password mismatch. Please try again.",
                        Target = "API"
                    };

                    return BadRequest(error);
                }

                if (_entites.Users.Any() == false)
                {
                    KeyValuePair<String, String> saltedHash = Hashing(Request.Form["PasswordInitial"], null);

                    User user = new User()
                    {
                        Username = Request.Form[nameof(Business.Model.User.Username)],
                        Firstname = Request.Form[nameof(Business.Model.User.Firstname)],
                        Lastname = Request.Form[nameof(Business.Model.User.Lastname)],
                        Role = "Admin",
                        Hash = saltedHash.Key,
                        Salt = saltedHash.Value
                    };

                    // Ne može se desiti da postoje direktori i glumci ,a niti jedan admin.
                    if (_entites.Directors.Any())
                    {
                        _entites.Directors.RemoveRange(_entites.Directors);
                    }

                    if (_entites.Actors.Any())
                    {
                        _entites.Actors.RemoveRange(_entites.Actors);
                    }

                    _entites.Add(user);

                    try
                    {
                        _entites.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _entites.ChangeTracker.Clear();
                        Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

                        ODataError error = new ODataError()
                        {
                            ErrorCode = Response.StatusCode.ToString(),
                            Message = ex.Message,
                            Target = "API"
                        };

                        return StatusCode(Response.StatusCode, error);
                    }

                }
                else if (Request.Headers.ContainsKey("Authorization") && Request.Form.Keys.Contains(nameof(Business.Model.User.Role)))
                {
                    String formattedToken = Request.Headers["Authorization"];
                    String token = null;

                    if (formattedToken.StartsWith("Bearer "))
                    {
                        token = formattedToken.Replace("Bearer", "").TrimStart();
                        if(String.IsNullOrEmpty(token) == false)
                        {
                            IEnumerable<Claim> claims = GetClaimsFromJWT(token);
                            String roleName = GetClaimByName(claims, nameof(Business.Model.User.Role));
                            if (roleName == "Admin")
                            {
                                // Sada administrstor ima pravo da kreira novog glumca i/ili direktora.

                                roleName = Request.Form[nameof(Business.Model.User.Role)];
                                if (new[] { nameof(Actor), nameof(Director) }.Contains(roleName))
                                {
                                    KeyValuePair<String, String> saltedHash = Hashing(Request.Form["PasswordInitial"], null);

                                    IActionResult result = CreateSubEntity(roleName, saltedHash);
                                    if ((result is OkResult) == false)
                                        return result;
                                    return GenerateToken(roleName);
                                }
                            }
                        }
                    }
                       
                }

                return GenerateToken(String.Empty);
            }

            return Content("", "application/json");
        }

        private IActionResult GenerateToken(String roleName)
        {
            String salt = String.Empty;
            String hash = String.Empty;


            User user = _entites.Users.FirstOrDefault(u => u.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault());
            if (roleName == String.Empty && user == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                ODataError error = new ODataError()
                {
                    ErrorCode = Response.StatusCode.ToString(),
                    Message = "Unknown user",
                    Target = "API"
                };

                return Unauthorized();
            }

            Director director = _entites.Directors.FirstOrDefault(u => u.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault());
            if (roleName == nameof(Director) && director == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                ODataError error = new ODataError()
                {
                    ErrorCode = Response.StatusCode.ToString(),
                    Message = "Unknown director",
                    Target = "API"
                };

                return Unauthorized();
            }

            Actor actor = _entites.Actors.FirstOrDefault(u => u.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault());
            if (roleName == nameof(Actor) && actor == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                ODataError error = new ODataError()
                {
                    ErrorCode = Response.StatusCode.ToString(),
                    Message = "Unknown actor",
                    Target = "API"
                };

                return Unauthorized();
            }

            if(roleName == String.Empty && user != null)
            {
                salt = user.Salt;
                hash = user.Hash;
            }
            else if(roleName == nameof(Actor) && actor != null)
            {
                salt = actor.Salt;
                hash = actor.Hash;
            }
            else if (roleName == nameof(Director) && director != null)
            {
                salt = director.Salt;
                hash = director.Hash;
            }


            KeyValuePair<String, String> saltedHash = Hashing(Request.Form["PasswordInitial"].FirstOrDefault() ?? Request.Form["Password"].FirstOrDefault(), salt);

            if(saltedHash.Key != hash) 
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                ODataError error = new ODataError()
                {
                    ErrorCode = Response.StatusCode.ToString(),
                    Message = "Unknown user",
                    Target = "API"
                };

                return Unauthorized(error);
            }

            IEnumerable<Claim> claims = new Claim[] { };
            if (roleName == String.Empty)
            {
                claims = new[]
                {
                    new Claim(nameof(Business.Model.User.Username), user.Username),
                    new Claim(nameof(Business.Model.User.Firstname), user.Firstname),
                    new Claim(nameof(Business.Model.User.Lastname), user.Lastname),
                    new Claim(nameof(Business.Model.User.Role), "Admin")
                };
            }
            else if(roleName == nameof(Director))
            {
                claims = new[]
                {
                    new Claim(nameof(Business.Model.User.Username), director.Username),
                    new Claim(nameof(Business.Model.User.Firstname), director.Firstname),
                    new Claim(nameof(Business.Model.User.Lastname), director.Lastname),
                    new Claim(nameof(Business.Model.User.Role), roleName)
                };
            }
            else if (roleName == nameof(Actor))
            {
                claims = new[]
                {
                    new Claim(nameof(Business.Model.User.Username), actor.Username),
                    new Claim(nameof(Business.Model.User.Firstname), actor.Firstname),
                    new Claim(nameof(Business.Model.User.Lastname), actor.Lastname),
                    new Claim(nameof(Business.Model.User.Role), roleName)
                };
            }
            else
                throw new NotImplementedException("Claims");

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig["Jwt:Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(_appConfig["Jwt:Issuer"],
              _appConfig["Jwt:Issuer"],
              expires: DateTime.Now.AddDays(7),
              signingCredentials: creds,
              claims: claims
              );

            String tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            IActionResult response = Ok(new FormatToken { Token = tokenString, FullTokenFormat = "Bearer " + tokenString });
            return response;
        }

        [HttpPost]
        public IActionResult SignInApi()
        {
            if (Request.Form.ContainsKey("Username") && Request.Form.ContainsKey("Password"))
            {
                return GenerateToken(String.Empty);

                // Request.Headers.Add("Authorization", "Bearer " + tokenString);
            }

            return BadRequest();
        }
 
        [HttpGet]
        public IActionResult ResetPassword()
        {
            throw new NotImplementedException();

            //return BadRequest();
        }


        //public IActionResult Index()
        //{
        //    return Content("Index", "application/json");
        //}
    }
}
