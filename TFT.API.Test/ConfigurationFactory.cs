using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TFT.API.Test
{
    public static class ConfigurationFactory
    {
        public static IConfigurationRoot GetAppConfig() => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
}
