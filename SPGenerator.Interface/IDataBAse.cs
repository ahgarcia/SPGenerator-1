using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Interface
{
    public interface IDataBase
    {
        string ConnectionString { get; set; }
        string SchemaName { get; set; }
        List<DBTableInfo> GetDataBaseTables();
    }

}
