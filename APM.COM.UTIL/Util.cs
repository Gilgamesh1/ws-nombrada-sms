using System.Configuration;
using System.Data;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace APM.COM.UTIL
{
	public static class Util
    {
        
    #region Config

        public static string GetConnectionStrings(string strName)
        {
            ConnectionStringsSection configurationSection = ConfigurationManager.GetSection("connectionStrings") as ConnectionStringsSection;
            return configurationSection.ConnectionStrings[strName].ConnectionString;
        }

        public static string GetAppSettings(string strName)
        {
            return ConfigurationManager.AppSettings[strName];
        }

    #endregion


    #region XML

        public static string getXMLData(DataTable tabla)
        {
            MemoryStream ms = new MemoryStream();
            XmlDocument xDocument = new XmlDocument();

            tabla.TableName = "TableName";
            tabla.WriteXml(ms, XmlWriteMode.IgnoreSchema, false);
            xDocument.LoadXml(System.Text.Encoding.UTF8.GetString(ms.GetBuffer()));

            return xDocument.InnerXml;
        }

    #endregion

        #region JSON
        public static string Stringify(object source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static T Parse<T>(string source)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling=NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };
            return JsonConvert.DeserializeObject<T>(source, settings);
        }
        #endregion
    }
}
