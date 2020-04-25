using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfXamlDemoContainer
{


    [ValueConversion(typeof(object), typeof(string))]
    public class StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private double testDouble = 100;
        public double testDoubleBind { get { return testDouble; } set { testDouble = value; } }
        //public double testDoubleBind
        //{
        //    get { return (double)GetValue(testDoubleProperty); }
        //    set { SetValue(testDoubleProperty, value); }
        //}
        //public static readonly DependencyProperty testDoubleProperty =
        //    DependencyProperty.Register("testDoubleBind", typeof(double), typeof(Window), new UIPropertyMetadata(1d));


        public MainWindow()
        {
            testDouble = 200;
            InitializeComponent();
            
            //Binding bind = new Binding("testDoubleBind");
            //bind.Source = this;
            //bind.Mode = BindingMode.TwoWay;
            //bind.Converter = UniversalValueConverter;
            //tb1.SetBinding(TextBox.TextProperty, bind);
            //testDoubleBind = 100;
        }
    }
}
