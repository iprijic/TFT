using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.UriParser;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TFT.API.Security
{
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IConfiguration appConfig) 
        {
            _appConfig = appConfig;
        }

        private IConfiguration _appConfig;


        //public class Credentials
        //{
        //    public string Username { get; set; } = "";
        //    public string Password { get; set; } = "";
        //}

        [HttpPost]
        public IActionResult SignInApi()
        {
            if(Request.Form.ContainsKey("Username") && Request.Form.ContainsKey("Password") && Request.Form["Username"] == "iprijic21@gmail.com" && Request.Form["Password"] == "test1234")
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig["Jwt:Key"]));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new JwtSecurityToken(_appConfig["Jwt:Issuer"],
                  _appConfig["Jwt:Issuer"],
                  expires: DateTime.Now.AddDays(7),
                  signingCredentials: creds);

                String tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                tokenString = "Bearer " + tokenString;

                IActionResult response = Ok(new { token = tokenString });
                return response;

                // Request.Headers.Add("Authorization", "Bearer " + tokenString);
            }

            return Content("", "application/json");

        }

        [HttpGet]
        public IActionResult SignOutApi()
        {
           // return new SignOutResult();
            return Content("", "application/json");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return Content("", "application/json");
        }


        public IActionResult Index()
        {
            return Content("", "application/json");
        }
    }
}
