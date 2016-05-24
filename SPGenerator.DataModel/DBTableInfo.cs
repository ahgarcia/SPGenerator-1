using System.Collections.Generic;

namespace SPGenerator.DataModel
{
    public class DBTableInfo
    {
        public string SchemaName;
        public string TableName;
        public List<DBTableColumnInfo> Columns;
    }
}
