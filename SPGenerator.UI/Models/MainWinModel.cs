using SPGenerator;
using SPGenerator.Core;
using SPGenerator.DAL;
using SPGenerator.DataModel;
using SPGenerator.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.UI.Models
{
    internal class MainWinModel
    {
        public List<DBTableInfo> ConnectToServer(string connectionString)
        {
            IDataBase dataBase = GetDataBaseObject(connectionString);
            return dataBase.GetDataBaseTables();
        }
        internal void RefreshSettings()
        {
            BaseSPGenerator.SetSettings(Comman.Settings.GetSettings());
        }
        public void GenerateSp(string tableName, string schema, string nodeName, ref StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            BaseSPGenerator spGenerator = SPFactory.GetSpGeneratorObject(nodeName);
            spGenerator.GenerateSp(tableName, schema, sb, selectedFields, whereConditionFields);
        }
        public void SaveGenerateSp(string tableName, string schema, string nodeName, string folderPath, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            StringBuilder sb = new StringBuilder();
            BaseSPGenerator spGenerator = SPFactory.GetSpGeneratorObject(nodeName);
            spGenerator.GenerateSp(tableName, schema, sb, selectedFields, whereConditionFields);
            File.WriteAllText(folderPath +"\\" + spGenerator.GetSpName(tableName, schema) +".sql" , sb.ToString());
        }
        private IDataBase GetDataBaseObject(string connectionString)
        {
            return new SqlDataBase(connectionString);
        }
    }
}
