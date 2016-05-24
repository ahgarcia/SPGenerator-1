using SPGenerator.UI.ViewModels;
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
using System.Windows.Shapes;
using TreeViewWithCheckBoxes;


namespace SPGenerator.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        MainWinVM vm;
        public MainWin()
        {
            InitializeComponent();
            vm = new MainWinVM();
            this.DataContext = vm;
        }
        private void SaveGenerateSps_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveGenerateSps();
        }
    }
}
