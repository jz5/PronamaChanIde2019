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

namespace PronamaChanIde2019.ShellChecker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Success_Button_Click(object sender, RoutedEventArgs e)
        {
            MyShell.ExpressEmotion(true);
        }

        private void Fail_Button_Click(object sender, RoutedEventArgs e)
        {
            MyShell.ExpressEmotion(false);
        }

        private async void Blink_Button_Click(object sender, RoutedEventArgs e)
        {
            await MyShell.BlinkAsync();
        }

        private async void ExpressEmotion_Button_Click(object sender, RoutedEventArgs e)
        {
            await MyShell.ExpressEmotionAsync();
        }
    }
}
