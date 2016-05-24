using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGenerator.Core
{
    class InsertUpdateSPGenerator : BaseSPGenerator
    {
        public override string GetSpName(string tableName)
        {
            return tableName + suffixInsertUpdateSp;
        }
        public override string GetSpName(string tableName, string schema)
        {
            return schema+ "." + tableName + suffixInsertUpdateSp;
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {

        }
        protected void GenerateUpdateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            var schema = "";

            if (!string.IsNullOrEmpty(selectedFields[0].Schema))
                schema = selectedFields[0].Schema + ".";

            StringBuilder sbFields = new StringBuilder();
            sb.Append(Environment.NewLine + "\t\tUPDATE " + schema + WrapIfKeyWord(tableName) + " SET ");
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.Exclude)
                    continue;
                sbFields.Append("\t\t" + WrapIfKeyWord(colInf.ColumnName) + "=" + prefixInputParameter + colInf.ColumnName + "," + Environment.NewLine);
            }
            sb.Append(sbFields.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(','));
            GenerateWhereStatement(whereConditionFields, sb);
        }
        protected void GenerateInsertStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            var schema = "";

            if (!string.IsNullOrEmpty(selectedFields[0].Schema))
                schema = selectedFields[0].Schema + ".";
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.Exclude)
                    continue;
                sbValues.Append("\t" + prefixInputParameter + colInf.ColumnName + "," + Environment.NewLine);
                sbFields.Append("\t" + WrapIfKeyWord(colInf.ColumnName) + "," + Environment.NewLine);
            }
            sb.Append(Environment.NewLine + "\tInsert into " + schema + WrapIfKeyWord(tableName) + " (" + Environment.NewLine + sbFields.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "\t)");
            sb.Append(Environment.NewLine + "\tvalues(" + Environment.NewLine + sbValues.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "\t)");
        }
        public override void GenerateSp(string tableName, string schema, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            string spName = GetSpName(tableName);
            GenerateDropScript(spName, sb);
            sb.Append(Environment.NewLine + "CREATE PROCEDURE " + schema + "." + spName);
            GenerateErrorNumberOutParameter(sb);
            GenerateInputParameters(selectedFields, sb);
            GenerateWhereParameters(whereConditionFields, sb);
            sb.Append(Environment.NewLine + "AS" + Environment.NewLine + "\tBEGIN");
            GenerateStartTryBlock(sb);
            GenerateUpdateStatement(tableName, sb, selectedFields, whereConditionFields);
            GenerateEndTryBlock(sb);
            GenerateCatchBlock(sb);
            sb.Append(Environment.NewLine + "IF (@@RowCount = 0)" + Environment.NewLine + "\tBEGIN");
            GenerateStartTryBlock(sb);
            GenerateInsertStatement(tableName, sb, selectedFields, whereConditionFields);
            GenerateEndTryBlock(sb);
            GenerateCatchBlock(sb);
            sb.Append(Environment.NewLine + "END");
            sb.Append(Environment.NewLine + "END");
            sb.Append(Environment.NewLine + "GO");
            sb.Append(Environment.NewLine);
        }
    }
}
