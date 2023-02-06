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

                if(_entites.Users.Any() == false)
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

                    //_entites.Attach(user);
                    //_entites.ChangeTracker.Clear();

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

                return GenerateToken();

                // Request.Headers.Add("Authorization", "Bearer " + tokenString);
            }

            return Content("", "application/json");
        }

        private IActionResult GenerateToken()
        {
            User user = _entites.Users.FirstOrDefault(u => u.Username == Request.Form[nameof(Business.Model.User.Username)].FirstOrDefault());
            if (user == null)
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

            KeyValuePair<String, String> saltedHash = Hashing(Request.Form["PasswordInitial"].FirstOrDefault() ?? Request.Form["Password"].FirstOrDefault(), user.Salt);

            if(saltedHash.Key != user.Hash) 
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


            IEnumerable<Claim> claims = new[]
            {
                    new Claim(nameof(Business.Model.User.Username), user.Username),
                    new Claim(nameof(Business.Model.User.Firstname), user.Firstname),
                    new Claim(nameof(Business.Model.User.Lastname), user.Lastname),
                    new Claim(nameof(Business.Model.User.Role), user.Role)
            };

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
                return GenerateToken();

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
