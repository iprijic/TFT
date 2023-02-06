using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFT.API.Business.Model;
using TFT.Repository;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Hosting.Server;
using System.Windows.Input;

namespace TFT.API.Test
{
    public static class DataFactory
    {
        static private String connString => ConfigurationFactory.GetAppConfig().GetValue<String>("Entities:targettest");

        static public void DropAndCreateDb()
        {       
            String script = File.ReadAllText(@".\..\..\..\Script\TFTModel.edmx.sql");
            String[] ScriptSplitter = script.Split(new string[] { "GO" }, StringSplitOptions.None);

            using (SqlConnection conn = new SqlConnection(connString))
            {  
                conn.Open();

                foreach (String str in ScriptSplitter)
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = str;
                        command.ExecuteNonQuery();
                    }
                }


                //using (SqlCommand command = new SqlCommand(script, conn))
                //{
                //    command.ExecuteNonQuery();
                //}
            }
        }

        static DataFactory()
        {
            string path = String.Join("\\", AppDomain.CurrentDomain.BaseDirectory.Split("\\").Where(s => String.IsNullOrEmpty(s) == false).Reverse().Skip(3).Reverse());
            RepositoryBuilder.Build(path, "UnitTest");
        }

        static public Entities GetDbContext()
        {
            DbContextOptions<Entities> options = new DbContextOptionsBuilder<Entities>().UseSqlServer(connString).Options;
            Entities entities = new Entities(options);
            entities.Database.EnsureCreated();
            return entities;
        }

        //static public void DbContextConnTest(DbContextOptions<Entities> options)
        //{
        //    using (Entities entities = new Entities(options))
        //    {
        //        List<Genre> genres = entities.Genres.ToList();
        //    }
        //}

        // builder.Environment.ContentRootPath
        // C:\Programming Projects\qualification process\TFT Falcon\TFT\TFT.API\

        static public IDataProtector GetDataProtector()
        {
            String contentRootPath = String.Join("\\", AppDomain.CurrentDomain.BaseDirectory.Split("\\").Where(s => String.IsNullOrEmpty(s) == false).Reverse().Skip(3).Reverse());

            IDataProtector protector = DataProtectionProvider.Create(new System.IO.DirectoryInfo($@"{contentRootPath}{ConfigurationFactory.GetAppConfig().GetValue<String>("Protection:Keys")}"), options =>
            {
                options.SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 20));
                options.UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
            }).CreateProtector(ConfigurationFactory.GetAppConfig().GetValue<String>("Protection:Cipher"));

            return protector;
        }
    }
}
