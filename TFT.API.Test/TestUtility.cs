using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFT.API.Business.Model;

namespace TFT.API.Test
{
    public class TestUtility
    {
        [Fact]
        void RemoveAll_Actor()
        {
            Entities entites = DataFactory.GetDbContext();
            entites.Actors.RemoveRange(entites.Actors);
            entites.SaveChanges();
        }


        [Fact]
        void RemoveAll_Director()
        {
            Entities entites = DataFactory.GetDbContext();
            entites.Directors.RemoveRange(entites.Directors);
            entites.SaveChanges();
        }

        [Fact]
        void RemoveAll_User_NonAdmin()
        {
            RemoveAll_Actor();
            RemoveAll_Director();
        }
    }
}
