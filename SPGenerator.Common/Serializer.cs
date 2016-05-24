using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.Common
{
    public class Serializer
    {
        public static void Serialize<T>(T obj, string filePath)
        {
            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(obj);
            File.WriteAllText(filePath, json);
        }

        public static T Deserialize<T>(T obj, string filePath) where T : class
        {
            //SPGenerator.DataModel.Settings settings = new DataModel.Settings();
            T objDes = null;
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                objDes = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<T>(json);
            }
            return objDes;
        }
    }
}
