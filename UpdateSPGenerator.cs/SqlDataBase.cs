using SPGenerator.DataModel;
using SPGenerator.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.DAL
{
    public partial class SqlDataBase : IDataBase
    {
        public SqlDataBase(string connectionString)
        {
            SchemaName = "dbo";
            ConnectionString = connectionString;
        }
        public List<DBTableInfo> GetDataBaseTables()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string sql = "SELECT SCHEMA_NAME(schema_id) AS [schema], name FROM sys.tables";
                sqlTableList = new List<DBTableInfo>();

                DataTable dt = ExecuteDataTable(sql, connection);
                identityColumns = LoadIdentityColumns(connection);
                primaryColumns = LoadPrimaryColumns(connection);
                foreach (DataRow dr in dt.Rows)
                {
                    var tbinfo = GetTableInformation(dr["name"].ToString(), dr["schema"].ToString(), connection);
                    sqlTableList.Add(tbinfo);
                }
            }

            return sqlTableList;
        }
        private DBTableInfo GetTableInformation(string tableName, string schema, SqlConnection connection)
        {
            string sql = "Select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + tableName + "'";
            var dt = ExecuteDataTable(sql, connection);
            var sqlTableInfo = new DBTableInfo();
            List<DBTableColumnInfo> colList = new List<DBTableColumnInfo>();

            foreach (DataRow dr in dt.Rows)
            {
                var colinfo = new DBTableColumnInfo();
                colinfo.ColumnName = dr["COLUMN_NAME"].ToString();
                colinfo.DataType = dr["DATA_TYPE"].ToString();
                if (dr["CHARACTER_MAXIMUM_LENGTH"] != null && dr["CHARACTER_MAXIMUM_LENGTH"].ToString().Trim() != "")
                    colinfo.CharacterMaximumLength = int.Parse(dr["CHARACTER_MAXIMUM_LENGTH"].ToString());

                if (dr["NUMERIC_PRECISION"] != null && dr["NUMERIC_PRECISION"].ToString().Trim() != "")
                    colinfo.NumericPrecision = int.Parse(dr["NUMERIC_PRECISION"].ToString());

                if (dr["NUMERIC_PRECISION_RADIX"] != null && dr["NUMERIC_PRECISION_RADIX"].ToString().Trim() != "")
                    colinfo.NumericPrecisionRadix = int.Parse(dr["NUMERIC_PRECISION_RADIX"].ToString());

                if (dr["NUMERIC_SCALE"] != null && dr["NUMERIC_SCALE"].ToString().Trim() != "")
                    colinfo.NumericScale = int.Parse(dr["NUMERIC_SCALE"].ToString());

                if (dr["TABLE_SCHEMA"] != null && dr["TABLE_SCHEMA"].ToString().Trim() != "")
                    colinfo.Schema = dr["TABLE_SCHEMA"].ToString();


                colinfo.IsIdentity = IsDinityColumn(dr["COLUMN_NAME"].ToString(), tableName);
                colinfo.IsPrimaryKey = IsPrimaryColumn(dr["COLUMN_NAME"].ToString(), tableName);
                colinfo.Exclude = IsExcludeColumn(colinfo);

                colList.Add(colinfo);
            }
            sqlTableInfo.SchemaName = schema;
            sqlTableInfo.TableName = tableName;
            sqlTableInfo.Columns = colList;
            return sqlTableInfo;

        }
        public string ConnectionString { get; set; }
        public string SchemaName { get; set; }
    }


    public partial class SqlDataBase
    {
        Dictionary<string, bool> identityColumns = new Dictionary<string, bool>();
        Dictionary<string, bool> primaryColumns = new Dictionary<string, bool>();
        List<DBTableInfo> sqlTableList = null;

        private DataTable ExecuteDataTable(string sql, SqlConnection connection)
        {
            var dataTable = new DataTable();
            var command = new SqlCommand(sql);
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        private bool IsExcludeColumn(DBTableColumnInfo colInfo)
        {
            if (colInfo.IsIdentity)
                return true;
            if (colInfo.DataType.ToUpperInvariant() == "TIMESTAMP")
                return true;

            return false;
        }

        private bool IsDinityColumn(string columnName, string tableName)
        {
            string key = (tableName + columnName).ToUpperInvariant();
            return identityColumns.ContainsKey(key);
        }

        private bool IsPrimaryColumn(string columnName, string tableName)
        {
            string key = (tableName + columnName).ToUpperInvariant();
            return identityColumns.ContainsKey(key);
        }

        private Dictionary<string, bool> LoadIdentityColumns(SqlConnection connection)
        {
            string sql = "select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = '" + SchemaName + "' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 order by TABLE_NAME";
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            var dt = ExecuteDataTable(sql, connection);
            foreach (DataRow dr in dt.Rows)
            {
                dic[(dr["TABLE_NAME"].ToString() + dr["COLUMN_NAME"].ToString()).ToUpperInvariant()] = true;
            }
            return dic;
        }

        private Dictionary<string, bool> LoadPrimaryColumns(SqlConnection connection)
        {
            string sql = "select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = '" + SchemaName + "' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsPrimaryKey') = 1 order by TABLE_NAME";
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            var dt = ExecuteDataTable(sql, connection);
            foreach (DataRow dr in dt.Rows)
            {
                dic[(dr["TABLE_NAME"].ToString() + dr["COLUMN_NAME"].ToString()).ToUpperInvariant()] = true;
            }
            return dic;
        }
    }
}
