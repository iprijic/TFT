using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TFT.API.Business.Model;
using TFT.API.Security;

namespace TFT.API.Test
{
    public class AuthenticationControllerTest
    {
        public AuthenticationControllerTest()
        {
            _controllerHttpContext = new DefaultHttpContext();
            FormatToken = null;
            _testValues = new TestValues();
        }

        private TestValues _testValues;

        private DefaultHttpContext _controllerHttpContext;
        public HttpContext HttpContext => _controllerHttpContext.HttpContext;

        public FormatToken FormatToken { get; private set; }

        void CheckDbIsEmpty(Entities entites)
        {
            Assert.NotNull(entites);
            Assert.Equal(0, entites.Users.Count());
            Assert.Equal(0, entites.Actors.Count());
            Assert.Equal(0, entites.Directors.Count());
        }

        /// <summary>
        /// Gets the <see cref="HttpRequest"/> for the executing action.
        /// </summary>
        public HttpRequest Request => HttpContext?.Request!;

        /// <summary>
        /// Gets the <see cref="HttpResponse"/> for the executing action.
        /// </summary>
        public HttpResponse Response => HttpContext?.Response!;

        private Mock<HttpContext> httpContext;
        public Mock<HttpContext> HttpMockContext => httpContext;


        [Fact]
        public void Login_Test()
        {
            IConfigurationRoot appConfig = ConfigurationFactory.GetAppConfig();
            Assert.NotNull(appConfig);

            IDataProtector protector = DataFactory.GetDataProtector();
            Assert.NotNull(protector);

            Entities entites = DataFactory.GetDbContext();
            Assert.NotEqual(0, entites.Users.Count());

            HttpRequest request = Mock.Of<HttpRequest>();
            HttpResponse response = Mock.Of<HttpResponse>();
            response.StatusCode = StatusCodes.Status204NoContent;

            AuthenticationController authController = new AuthenticationController(appConfig, entites, protector);

            httpContext = new Mock<HttpContext>();

            httpContext.Setup(c => c.Request)
                           .Returns(request);
            httpContext.Setup(c => c.Response)
                           .Returns(response);

            authController.ControllerContext.HttpContext = httpContext.Object;

            FormCollection form;
            IActionResult result;
            Dictionary<string, StringValues> formDictionary = new Dictionary<string, StringValues>();

            formDictionary.Clear();

            formDictionary.Add(nameof(User.Username), "iperic21@gmail.com");
            formDictionary.Add("Password", "test1234");

            form = new FormCollection(formDictionary);
            httpContext.Object.Request.Form = form;
            result = authController.SignInApi();

            if (result is OkObjectResult)
            {
                Assert.Equal(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            }

            if (result is OkObjectResult && (result as OkObjectResult).Value is FormatToken)
            {
                FormatToken = (result as OkObjectResult).Value as FormatToken;
            }
            else
            {
                throw new Exception("Improper IActionResult cast in AuthenticationController");
            }

            IEnumerable<Claim> claims = SecurityHandler.GetClaimsFromJWT(FormatToken.Token);

            ClaimsPrincipal user = SecurityHandler.GetIdentity(claims);
            httpContext.Setup(c => c.User)
                          .Returns(user);
            httpContext.Setup(c => c.User.Identity.IsAuthenticated).Returns(true);
            Dictionary<string, StringValues> authHeader = new Dictionary<string, StringValues>() {
                { "Authorization" , new StringValues(FormatToken.FullTokenFormat) }
            };

            IHeaderDictionary hd = new HeaderDictionary(authHeader);
            httpContext.Setup(c => c.Request.Headers).Returns(hd);

            entites.Dispose();
        }

        [Fact]
        void Register_Director_Test()
        {
            Login_Test();

            IConfigurationRoot appConfig = ConfigurationFactory.GetAppConfig();
            Assert.NotNull(appConfig);

            IDataProtector protector = DataFactory.GetDataProtector();
            Assert.NotNull(protector);

            Entities entites = DataFactory.GetDbContext();
            Assert.NotEqual(0, entites.Users.Count());

            FormCollection form;
            IActionResult result;

            Dictionary<string, StringValues> formDictionary = new Dictionary<string, StringValues>();
            AuthenticationController authController = new AuthenticationController(appConfig, entites, protector);

            authController.ControllerContext.HttpContext = httpContext.Object;

            _testValues.FeedDirector.ToList().ForEach(d => 
            { 
                formDictionary.Clear();
                formDictionary.Add(nameof(User.Username), d.Username);
                formDictionary.Add(nameof(User.Firstname), d.Firstname);
                formDictionary.Add(nameof(User.Lastname), d.Lastname);
                formDictionary.Add(nameof(Director.DirectorID), "344356");
                formDictionary.Add("PasswordInitial", "test8899");
                formDictionary.Add("PasswordConfirmed", "test8899");
                formDictionary.Add("Role", nameof(Director));

                form = new FormCollection(formDictionary);
                httpContext.Object.Request.Form = form;

                result = authController.RegisterUser();
                if (result is OkObjectResult)
                {
                    Assert.Equal(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
                }

                if (result is OkObjectResult && (result as OkObjectResult).Value is FormatToken)
                {
                    FormatToken = (result as OkObjectResult).Value as FormatToken;
                }
                else
                {
                    throw new Exception("Improper IActionResult cast in AuthenticationController");
                }
            });
        }

        [Fact]
        void Register_Actor_Test()
        {
            Login_Test();

            IConfigurationRoot appConfig = ConfigurationFactory.GetAppConfig();
            Assert.NotNull(appConfig);

            IDataProtector protector = DataFactory.GetDataProtector();
            Assert.NotNull(protector);

            Entities entites = DataFactory.GetDbContext();
            Assert.NotEqual(0, entites.Users.Count());


            FormCollection form;
            IActionResult result;
            Dictionary<string, StringValues> formDictionary = new Dictionary<string, StringValues>();


            AuthenticationController authController = new AuthenticationController(appConfig, entites, protector);

            authController.ControllerContext.HttpContext = httpContext.Object;

            _testValues.FeedActor.ToList().ForEach(a =>
            {
                formDictionary.Clear();
                formDictionary.Add(nameof(User.Username), a.Username);
                formDictionary.Add(nameof(User.Firstname), a.Firstname);
                formDictionary.Add(nameof(User.Lastname), a.Lastname);
                formDictionary.Add(nameof(Actor.ActorID), "3456356");
                formDictionary.Add("PasswordInitial", "test4499");
                formDictionary.Add("PasswordConfirmed", "test4499");
                formDictionary.Add("Role", nameof(Actor));

                form = new FormCollection(formDictionary);
                httpContext.Object.Request.Form = form;

                result = authController.RegisterUser();
                if (result is OkObjectResult)
                {
                    Assert.Equal(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
                }

                if (result is OkObjectResult && (result as OkObjectResult).Value is FormatToken)
                {
                    FormatToken = (result as OkObjectResult).Value as FormatToken;
                }
                else
                {
                    throw new Exception("Improper IActionResult cast in AuthenticationController");
                }
            });

        }

        /// <summary>
        /// Kreiranje samo jednog i glavnog administratora u potpuno praznoj bazi podataka.
        /// Ovaj administrator će moći kreirati glumce i direktore.
        /// </summary>

        [Fact]
        void Register_Admin_Test()
        {

            IConfigurationRoot appConfig = ConfigurationFactory.GetAppConfig();
            Assert.NotNull(appConfig);

            IDataProtector protector = DataFactory.GetDataProtector();
            Assert.NotNull(protector);

            FormatToken = null;
            DataFactory.DropAndCreateDb();
            Entities entites = DataFactory.GetDbContext();
            CheckDbIsEmpty(entites);


            AuthenticationController authController = new AuthenticationController(appConfig, entites, protector);


            Dictionary<string, StringValues> formDictionary = new Dictionary<string, StringValues>();

            HttpRequest request = Mock.Of<HttpRequest>();
            HttpResponse response = Mock.Of<HttpResponse>();
            response.StatusCode = StatusCodes.Status204NoContent;

            httpContext = new Mock<HttpContext>();

            httpContext.Setup(c => c.Request)
                           .Returns(request);
            httpContext.Setup(c => c.Response)
                           .Returns(response);

            authController.ControllerContext.HttpContext = httpContext.Object;

            FormCollection form;
            IActionResult result;


            formDictionary.Clear();
            formDictionary.Add(nameof(User.Username), "iperic21@gmail.com");
            formDictionary.Add(nameof(User.Firstname), "Ivan");
            formDictionary.Add(nameof(User.Lastname) + "NoName", "Perić"); // Ovo ne postoji niti u bazi niti u HTTP POST formi. Ovo neka bude nadalje StatusCode = 401.
            formDictionary.Add("PasswordInitial", "test1234");
            formDictionary.Add("PasswordConfirmed", "test1234");
            form = new FormCollection(formDictionary);
            httpContext.Object.Request.Form = form;

            result = authController.RegisterUser();
            Assert.Equal(response.StatusCode, StatusCodes.Status500InternalServerError);

            CheckDbIsEmpty(entites);

            // Provjera ponovljenog passworda se mogla napraviti i na frontendu.
            formDictionary.Clear();
            formDictionary.Add(nameof(User.Username), "iperic21@gmail.com");
            formDictionary.Add(nameof(User.Firstname), "Ivan");
            formDictionary.Add(nameof(User.Lastname), "Perić"); // Sada je u redu u odnusu na gornji slučaj.
            formDictionary.Add("PasswordInitial", "test1234");
            formDictionary.Add("PasswordConfirmed", "test1236756"); // Ali je sada loš ponovljen unošeni password.
            form = new FormCollection(formDictionary);
            httpContext.Object.Request.Form = form;

            result = authController.RegisterUser();
            Assert.Equal(response.StatusCode, StatusCodes.Status400BadRequest);

            CheckDbIsEmpty(entites);

            formDictionary.Clear();
            formDictionary.Add(nameof(User.Username), "iperic21@gmail.com");
            formDictionary.Add(nameof(User.Firstname), "Ivan");
            formDictionary.Add(nameof(User.Lastname), "Perić");
            formDictionary.Add("PasswordInitial", "test1234");
            formDictionary.Add("PasswordConfirmed", "test1234");
            form = new FormCollection(formDictionary);
            httpContext.Object.Request.Form = form;

            // Sada mora biti sve u redu s registracijom.

            result = authController.RegisterUser();

            if (result is OkObjectResult)
            {
                Assert.Equal(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);
            }
 
            if (result is OkObjectResult && (result as OkObjectResult).Value is FormatToken)
            {
                FormatToken = (result as OkObjectResult).Value as FormatToken;
            }
            else
            {
                throw new Exception("Improper IActionResult cast in AuthenticationController");
            }

            IEnumerable<Claim> claims = SecurityHandler.GetClaimsFromJWT(FormatToken.Token);

            if(entites.Users.Count() == 1)
            {
                Assert.Equal(AuthenticationController.DefaultAdminRoleName, SecurityHandler.GetClaimByName(claims, nameof(User.Role)));
            }
            else if(entites.Users.Count() > 1)
            {
                Assert.Contains(SecurityHandler.GetClaimByName(claims, nameof(User.Role)), new[] { nameof(Actor), nameof(Director) });
            }

            new[] { nameof(User.Username), nameof(User.Firstname), nameof(User.Lastname) }.ToList()
            .ForEach(p => Assert.Equal(formDictionary[p].FirstOrDefault(), SecurityHandler.GetClaimByName(claims, p)));

            DataFactory.FillRequiredTables(entites);

            ClaimsPrincipal user = SecurityHandler.GetIdentity(claims);
            httpContext.Setup(c => c.User)
                          .Returns(user);
            httpContext.Setup(c =>c.User.Identity.IsAuthenticated).Returns(true);
            Dictionary<string, StringValues> authHeader = new Dictionary<string, StringValues>() {
                { "Authorization" , new StringValues(FormatToken.FullTokenFormat) }
            };

            IHeaderDictionary hd = new HeaderDictionary(authHeader);
            httpContext.Setup(c => c.Request.Headers).Returns(hd);

            entites.Dispose();
        }
    }
}
