﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TFT.Repository
{
    //public interface IRepositoryBuilder
    //{

    //}
    public static class RepositoryBuilder //: IRepositoryBuilder
    {
        //public static void GetConfigFromSettings()
        //{
        //    IConfigurationRoot MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //    string AppName = MyConfig.GetValue<string>("AppSettings:Name");
        //}

        public static void Build(WebApplicationBuilder builder)
        {
            throw new NotImplementedException();
        }

        public static void Build(String contentRootPath, String subFolder = "Production")
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String libName = assembly.GetName().Name ?? "";
            
            String fullPath = Path.Combine(String.Join(@"\",
                contentRootPath.Split(@"\").Where(s => String.IsNullOrEmpty(s) == false).Reverse().Skip(1).Reverse()),
            libName, "DataSource");

            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(fullPath, subFolder));
        }
    }
}