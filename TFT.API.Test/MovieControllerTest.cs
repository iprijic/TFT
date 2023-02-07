using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFT.API.Business.Model;
using TFT.API.Controllers;
using TFT.API.Security;

namespace TFT.API.Test
{
    public class MovieControllerTest
    {
        public MovieControllerTest()
        {
            _testValues = new TestValues();
        }

        private TestValues _testValues;
        private Mock<HttpContext> httpContext;

        // Film bi trebao moći napraviti samo administrator.
        [Fact]
        public void CreateMovie_Test() 
        {
            AuthenticationControllerTest ac = new AuthenticationControllerTest();
            ac.Login_Test();
        
            IConfigurationRoot appConfig = ConfigurationFactory.GetAppConfig();
            Assert.NotNull(appConfig);

            IDataProtector protector = DataFactory.GetDataProtector();
            Assert.NotNull(protector);

            Entities entites = DataFactory.GetDbContext();
            Assert.NotEqual(0, entites.Users.Count());

            FormCollection form;
            IActionResult result;

            Dictionary<string, StringValues> formDictionary = new Dictionary<string, StringValues>();
            MovieController movieController = new MovieController(appConfig, entites, protector);

            HttpRequest request = Mock.Of<HttpRequest>();
            IHeaderDictionary header = Mock.Of<IHeaderDictionary>();
   
            HttpResponse response = Mock.Of<HttpResponse>();
            response.StatusCode = StatusCodes.Status204NoContent;
            httpContext = new Mock<HttpContext>();

            httpContext.Setup(c => c.Request)
                           .Returns(request);          

            httpContext.Setup(c => c.Request.Headers)
                           .Returns(ac.HttpMockContext.Object.Request.Headers);

            httpContext.Setup(c => c.Response)
                           .Returns(response);

            movieController.ControllerContext.HttpContext = httpContext.Object;

            List<Movie> movies = _testValues.FeedMovie.ToList();

            movies.ToList().ForEach(m => 
            {
                formDictionary.Clear();

                formDictionary.Add(nameof(Movie.Title), m.Title);
                formDictionary.Add(nameof(Movie.Budget), m.Budget.ToString());
                formDictionary.Add(nameof(Movie.Description), m.Description);
                formDictionary.Add(nameof(Movie.Duration), m.Duration.ToString());
                formDictionary.Add(nameof(Movie.StartProduction), m.StartProduction.ToString());
                formDictionary.Add(nameof(Movie.EndProduction), m.EndProduction.ToString());

                int i = movies.IndexOf(m);
                int index = _testValues.FeedMovieDirector[i];
                Director director =_testValues.FeedDirector[index];

                String[] listGenre =_testValues.FeedGenre
                .Join(_testValues.FeedMovieGenre[i],
                (outer) => _testValues.FeedGenre.ToList().IndexOf(outer),
                (inner) => inner,
                (outer,inner) => outer.Name).ToArray();

                formDictionary.Add(nameof(Director), director.Username);
                formDictionary.Add(nameof(GenreMovie), new StringValues(listGenre));


                form = new FormCollection(formDictionary);
                httpContext.Object.Request.Form = form;
                result = movieController.CreateMovie();
            });
        }
    }
}
