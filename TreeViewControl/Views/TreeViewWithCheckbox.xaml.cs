using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeViewWithCheckBoxes;

namespace SPGenerator.UserControls
{
    /// <summary>
    /// Interaction logic for TreeViewWithCheckBox.xaml
    /// </summary>
    public partial class TreeViewWithCheckBox : UserControl
    {
        public TreeViewWithCheckBox()
        {
            InitializeComponent();
        }

        public void BindCommand()
        {
            TreeViewNode root = this.tree.Items[0] as TreeViewNode;

            base.CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Undo,
                    (sender, e) => // Execute
                    {
                        e.Handled = true;
                        root.IsChecked = false;
                        this.tree.Focus();
                    },
                    (sender, e) => // CanExecute
                    {
                        e.Handled = true;
                        e.CanExecute = (root.IsChecked != false);
                    }));

            this.tree.Focus();

        }

        public TreeView GetTree()
        {
            return this.tree;
        }

    }
}
