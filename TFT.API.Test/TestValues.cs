using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TFT.API.Business.Model;

namespace TFT.API.Test
{
    public class TestValues
    {
        public Genre[] FeedGenre { get; } = new Genre[]
        {
            new Genre()
            {
                Name = "Action"
            },
            new Genre()
            {
                Name = "Science Fiction"
            },
            new Genre()
            {
                Name = "Romance"
            },
            new Genre()
            {
                Name = "Adventure"
            }
        };

        public Director[] FeedDirector { get; } = new Director[]
        {
            new Director()
            {
                DirectorID = "23252",
                Username = "glucas@gmail.com",
                Firstname = "George",
                Lastname = "Lucas"
            }
            ,
            new Director()
            {
                DirectorID = "13252",
                Username = "lleterrier@gmail.com",
                Firstname = "Louis",
                Lastname = "Leterrier"
            }
        };

        public Actor[] FeedActor { get; } = new Actor[]
        {
            new Actor()
            {
                ActorID = "24565452",
                Username = "mhamill@gmail.com",
                Firstname = "Mark",
                Lastname = "Hamill"
            },
            new Actor()
            {
                ActorID = "223532565452",
                Username = "hford@gmail.com",
                Firstname = "Harrison",
                Lastname = "Ford"
            }
            ,
            new Actor()
            {
                ActorID = "563532565452",
                Username = "cfisher@gmail.com",
                Firstname = "Carrie",
                Lastname = "Fisher"
            }
            ,
            new Actor()
            {
                ActorID = "783532565452",
                Username = "jwayne@gmail.com",
                Firstname = "John",
                Lastname = "Wayne"
            }
            ,
            new Actor()
            {
                ActorID = "997532565457",
                Username = "vdiesel@gmail.com",
                Firstname = "Vin",
                Lastname = "Diesel"
            }
            ,
             new Actor()
            {
                ActorID = "997532565457",
                Username = "ajolie@gmail.com",
                Firstname = "Angelina",
                Lastname = "Jolie"
            }
        };

        public List<int>[] FeedMovieGenre { get; } = new List<int>[] 
        { 
            new List<int>() { 0, 1 } // Ovaj element na index-u 0 je 0-ti index u movie array-u (vidjeti dolje: "Star Wars Episode 10") ,i ima Genre-ove na index-ima { 0, 1 }. Vidjeti gore: "Action" i "Science Fiction"
        };

        public int[] FeedMovieDirector { get; } = new int[]
        {
            0 // Ovaj element na index-u 0 je 0-ti index u movie array-u (vidjeti dolje: "Star Wars Episode 10") ,i ima Director index 0 vidjeti gore (znači G. Lucas)
        };


        public Movie[] FeedMovie { get; } = new Movie[]
        {
            new Movie
            {
                Title = "Star Wars Episode 10",
                Description = "None",
                Budget= 10000000,
                StartProduction = new DateTime(year:2023,month:4, day:1),
                EndProduction = new DateTime(year:2023,month:7, day:1),
                Duration = DateTimeOffset.Now
            }
            ,
             new Movie
            {
                Title = "The Transporter",
                Description = "None",
                Budget= 20000000,
                StartProduction = new DateTime(year:2023,month:5, day:1),
                EndProduction = new DateTime(year:2023,month:10, day:1),
                Duration = DateTimeOffset.Now
            }
        };
    }
}
