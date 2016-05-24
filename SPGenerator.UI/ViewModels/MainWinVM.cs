using SPGenerator;
using SPGenerator.Common;
using SPGenerator.DataModel;
using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreeViewWithCheckBoxes;


namespace SPGenerator.UI.ViewModels
{
    internal class MainWinVM : ViewModelBase
    {

        #region vairable
        TreeViewNode rootNode;
        MainWinModel model;
        //string conString = @"Server=.\sqlexpress;Database=TestDb;User ID=mahesh;Password=mypassword;";
        string defaultDisplayText = "Enter Connection String Here";
        #endregion

        public MainWinVM()
        {
            model = new MainWinModel();
            ConnectionString = defaultDisplayText;
            string[] args = Environment.GetCommandLineArgs();
            if(args.Length > 1)
            {
               ConnectionString = args[1];
               if(args.Length > 2)
                {
                    FolderPath = args[2];
                }
            }
        }

        #region Properties
        string sqlScript;
        public string SqlScript
        {
            get
            {
                return sqlScript;
            }
            set
            {
                sqlScript = value;
                NotifyPropertyChanged("SqlScript");
            }
        }

        string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                NotifyPropertyChanged("ConnectionString");
            }
        }
        string folderPath;
        public string FolderPath
        {
            get
            {
                return folderPath;
            }
            set
            {
                folderPath = value;
                NotifyPropertyChanged("FolderPath");
            }
        }

        bool isConnectedToServer;
        public bool IsConnectedToServer
        {
            get
            {
                return isConnectedToServer;
            }
            set
            {
                isConnectedToServer = value;
                NotifyPropertyChanged("IsConnectedToServer");
            }
        }

        #endregion

        #region Commands
        private RelayCommand connectServerCommand;
        public ICommand ConnectServerCommand
        {
            get
            {
                if (connectServerCommand == null) connectServerCommand = new RelayCommand(param => this.ConnectToServer(param));
                return connectServerCommand;
            }
        }
        private RelayCommand saveProcsToFile;
        public ICommand SaveProcsToFile
        {
            get
            {
                if (saveProcsToFile == null) saveProcsToFile = new RelayCommand(param => this.SaveGenerateSps());
                return saveProcsToFile;
            }
        }
        private void ConnectToServer(object param)
        {
            var currentCursor = Mouse.OverrideCursor;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var lstTabales = model.ConnectToServer(ConnectionString);
                PopulateTree(lstTabales, (SPGenerator.UserControls.TreeViewWithCheckBox)param);
                IsConnectedToServer = true;
            }

            finally
            {
                Mouse.OverrideCursor = currentCursor;
            }
        }

        private RelayCommand generateSPCommand;
        public ICommand GenerateSPCommand
        {
            get
            {
                if (generateSPCommand == null) generateSPCommand = new RelayCommand(param => this.GenerateSps());
                return generateSPCommand;
            }
        }

        private RelayCommand settingCommand;
        public ICommand SettingCommand
        {
            get
            {
                if (settingCommand == null) settingCommand = new RelayCommand(param => this.Settings());
                return settingCommand;
            }
        }
        private void Settings()
        {
            Views.Settings setting = new Views.Settings();
            setting.Show();
        }

        private RelayCommand copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null) copyCommand = new RelayCommand(param => this.Copy());
                return copyCommand;
            }
        }
        private void Copy()
        {
            if (!string.IsNullOrEmpty(SqlScript))
            {
                Clipboard.SetText(SqlScript);
            }
        }

        private RelayCommand gotFocusConnectionStringCommand;
        public ICommand GotFocusConnectionStringCommand
        {
            get
            {
                if (gotFocusConnectionStringCommand == null) gotFocusConnectionStringCommand = new RelayCommand(param => this.GotFocusConnectionString());
                return gotFocusConnectionStringCommand;
            }
        }
        private void GotFocusConnectionString()
        {
            if (ConnectionString.Trim() == defaultDisplayText)
                ConnectionString = "";
        }

        private RelayCommand lostFocusConnectionStringCommand;
        public ICommand LostFocusConnectionStringCommand
        {
            get
            {
                if (lostFocusConnectionStringCommand == null) lostFocusConnectionStringCommand = new RelayCommand(param => this.LostFocusConnectionString());
                return lostFocusConnectionStringCommand;
            }
        }
        private void LostFocusConnectionString()
        {
            if (ConnectionString.Trim() == "")
                ConnectionString = defaultDisplayText;
        }

        #endregion

        #region SP Generation

        private void GenerateSps()
        {
            StringBuilder sb = new StringBuilder(1000);
            model.RefreshSettings();
            foreach (TreeViewNode tblNode in rootNode.Children)
            {
                if (tblNode.IsChecked ?? true)
                {
                    GenerateSPForSingleTable(tblNode, sb);
                }
            }
            SqlScript = sb.ToString();
        }
        public void SaveGenerateSps()
        {
            model.RefreshSettings();
            foreach (TreeViewNode tblNode in rootNode.Children)
            {
                if (tblNode.IsChecked ?? true)
                {
                    SaveGenerateSPForSingleTable(tblNode);
                }
            }
        }

        private void GenerateSPForSingleTable(TreeViewNode tblNode, StringBuilder sb)
        {
            foreach (TreeViewNode childNode in tblNode.Children)
            {
                var selectedFields = GetSelectedFields(childNode);
                var whereClauseSelectedFields = GetWhereClauseFields(childNode);

                if (childNode.IsChecked ?? true)
                {
                    DBTableInfo tableData = (DBTableInfo)tblNode.Tag;
                    model.GenerateSp(tableData.TableName, tableData.SchemaName, childNode.Name, ref sb, selectedFields, whereClauseSelectedFields);
                }
            }
        }
        private void SaveGenerateSPForSingleTable(TreeViewNode tblNode)
        {
            foreach (TreeViewNode childNode in tblNode.Children)
            {

                var selectedFields = GetSelectedFields(childNode);
                var whereClauseSelectedFields = GetWhereClauseFields(childNode);

                if (childNode.IsChecked ?? true)
                {
                    DBTableInfo tableData = (DBTableInfo)tblNode.Tag;
                    model.SaveGenerateSp(tableData.TableName, tableData.SchemaName, childNode.Name, FolderPath, selectedFields, whereClauseSelectedFields);
                }
            }
        }

        private List<DBTableColumnInfo> GetSelectedFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            foreach (TreeViewNode fieldNode in tableChildNode.Children)
            {
                if ((fieldNode.IsChecked ?? true) && fieldNode.Name != Constants.whereConditionTreeNodeText)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        private List<DBTableColumnInfo> GetWhereClauseFields(TreeViewNode tableChildNode)
        {
            var selectedFields = new List<DBTableColumnInfo>();
            var whereClauseNode = GetChildNodeByText(tableChildNode, Constants.whereConditionTreeNodeText);
            if (whereClauseNode == null)
                return selectedFields;

            foreach (TreeViewNode fieldNode in whereClauseNode.Children)
            {
                if (fieldNode.IsChecked ?? true)
                {
                    selectedFields.Add((DBTableColumnInfo)fieldNode.Tag);
                }
            }

            return selectedFields;
        }

        private TreeViewNode GetChildNodeByText(TreeViewNode parentNode, string childNodeText)
        {
            var childNode = parentNode.Children.OfType<TreeViewNode>()
                          .FirstOrDefault(node => node.Name.Equals(childNodeText));

            return childNode;
        }

        #endregion

        #region PopulateTree
        private void PopulateTree(List<DBTableInfo> sqlTableList, SPGenerator.UserControls.TreeViewWithCheckBox treeView1)
        {
            TreeViewNode root = new TreeViewNode(Constants.rootTreeNodeText, null);
            foreach (var tbl in sqlTableList)
            {
                root.Children.Add(CreateTableNode(tbl, root));
            }
            root.IsNodeExpanded = true;
            treeView1.DataContext = new List<TreeViewNode> { root };
            rootNode = root;
        }

        private TreeViewNode CreateTableNode(DBTableInfo sqlTableInfo, TreeViewNode parent)
        {
            TreeViewNode tblNode = new TreeViewNode(sqlTableInfo.SchemaName + "." + sqlTableInfo.TableName, parent);
            tblNode.Tag = sqlTableInfo;

            TreeViewNode insertSp = new TreeViewNode(Constants.insertTreeNodeText, tblNode);
            TreeViewNode updateSp = new TreeViewNode(Constants.updateTreeNodeText, tblNode);
            TreeViewNode uspWhereCondition = new TreeViewNode(Constants.whereConditionTreeNodeText, updateSp);
            TreeViewNode insertUpdateSp = new TreeViewNode(Constants.insertUpdateTreeNodeText, tblNode);
            TreeViewNode iuspWhereCondition = new TreeViewNode(Constants.whereConditionTreeNodeText, insertUpdateSp);

            AddColumnNodes(insertSp, sqlTableInfo.Columns, true);
            AddColumnNodes(updateSp, sqlTableInfo.Columns, true);
            AddColumnNodes(uspWhereCondition, sqlTableInfo.Columns, false);
            AddColumnNodes(insertUpdateSp, sqlTableInfo.Columns, true);
            AddColumnNodes(iuspWhereCondition, sqlTableInfo.Columns, false);
            updateSp.Children.Add(uspWhereCondition);
            insertUpdateSp.Children.Add(iuspWhereCondition);


            tblNode.Children.Add(insertSp);
            tblNode.Children.Add(updateSp);
            tblNode.Children.Add(insertUpdateSp);
            return tblNode;
        }

        private TreeViewNode AddColumnNodes(TreeViewNode parentNode, List<DBTableColumnInfo> columns, bool disableExludeColumn)
        {
            foreach (var colInfo in columns)
            {
                TreeViewNode colNode = new TreeViewNode(GetColumnDispalyName(colInfo), parentNode);
                colNode.Tag = colInfo;
                if (disableExludeColumn)
                {
                    colNode.IsNodeEnabled = !colInfo.Exclude;
                }
                parentNode.Children.Add(colNode);
            }
            return parentNode;
        }

        private string GetColumnDispalyName(DBTableColumnInfo colInfo)
        {
            string diaplayName = colInfo.ColumnName;
            if (colInfo.Exclude)
            {
                if (colInfo.IsIdentity)
                    diaplayName += " (IDENTITY)";
                if (colInfo.DataType.ToUpperInvariant() == "TIMESTAMP")
                    diaplayName += " (TIMESTAMP)";

            }

            return diaplayName;
        }

        #endregion

    }
}
