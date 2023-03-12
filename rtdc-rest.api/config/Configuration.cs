using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rtdc_rest.api.config
{
    public class Configuration
    {
        public static string getApiUserName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ApiUserName"];
        }
        public static string getApiPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ApiPassword"];
        }
        public static string getRetailers()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ApiPassword"];
        }
        public static string getLogoConnection()
        {
            return System.Configuration.ConfigurationManager.AppSettings["logoConnection"];
        }
        public static string getApiUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
        }
    }
}
