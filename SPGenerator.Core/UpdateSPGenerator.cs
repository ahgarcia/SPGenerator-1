using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    class UpdateSPGenerator : BaseSPGenerator
    {
        public override string GetSpName(string tableName)
        {
            return prefixUpdateSp + tableName;
        }
        public override string GetSpName(string tableName, string schema)
        {
            return schema + "." + tableName + suffixUpdateSp;
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            var schema = "";

            if (!string.IsNullOrEmpty(selectedFields[0].Schema))
                schema = "[" + selectedFields[0].Schema + "].";

            sb.Append(Environment.NewLine + "\tUPDATE " + schema + WrapIfKeyWord(tableName) + " SET ");

            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.Exclude)
                    continue;
                sb.Append(WrapIfKeyWord(colInf.ColumnName) + "=" + prefixInputParameter + colInf.ColumnName);
                sb.Append(",");
            }
            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
            GenerateWhereStatement(whereConditionFields, sb);
        }
    }
}
