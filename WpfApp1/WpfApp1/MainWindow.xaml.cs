using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace WpfApp1
{

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public static double opacity = 1.0;     


        public double opacityBind 
        {
            get { return opacity; } 
            set 
            { 
                opacity = value;
                OnPropertyChanged("opacityBind");  
            } 
        }
        private string secondRowHeight;
        public string secondRowHeightBind
        {
            get 
            {
                return secondRowHeight;
            }
            set
            {
                secondRowHeight = value;
                OnPropertyChanged("secondRowHeightBind");
            }
        }

        public void ShowMore()
        {
            secondRowHeight = "25*";
        }
        public void HideMore()
        {
            secondRowHeight = "0*";
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseLeftButtonDown;
            var main_window = this;
            var opacity_menu_vm = new MenuItemViewModel
                {
                    Header = "设置透明度",
                    Name = "SetOpacity"
                };
            opacity_menu_vm.MenuItems = new ObservableCollection<MenuItemViewModel>
                {
                    new OpacityMenuItemViewModel { Header = "不透明", Opacity = 1.0, opacity_parent_view_group = opacity_menu_vm, main_window = this },
                    new OpacityMenuItemViewModel { Header = "80%", Opacity = 0.8, opacity_parent_view_group = opacity_menu_vm, main_window = this },
                    new OpacityMenuItemViewModel { Header = "60%", Opacity = 0.6, opacity_parent_view_group = opacity_menu_vm, main_window = this },
                    new OpacityMenuItemViewModel { Header = "40%", Opacity = 0.4, opacity_parent_view_group = opacity_menu_vm, main_window = this }
                };
            MenuItems = new ObservableCollection<MenuItemViewModel>
                        {
                            new MenuItemViewModel { Header = "退出", Name = "Exit" },
                            opacity_menu_vm,
                            new MenuItemViewModel { Header = "显示更多", Name="ShowMore", main_window = main_window }
                        };


            DataContext = this;            
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }  
    }

    public class MenuItemViewModel : ViewModelBase
    {
        protected ICommand _command;

        public MenuItemViewModel()
        {
            _command = new CommandViewModel(Execute);
        }
        public MainWindow main_window { set; get; }
        public string Header { get; set; }
        public string Name { get; set; }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public ICommand Command
        {
            get
            {
                return _command;
            }
        }
        public bool IsCheckable
        {
            get
            {
                return false;
            }
        }
        protected bool _is_selected = false;
        private void Execute()
        {           
            HandleCommand(Name);
        }
        void HandleCommand(string menu_name)
        {
            if (menu_name == "Exit")
            {
                Application.Current.Shutdown();
            }
            else if (menu_name == "ShowMore")
            {
                if (_is_selected)
                {
                    main_window.ShowMore();
                }
                else
                {
                    main_window.HideMore();
                }
            }
        }
    }

    public class ToggleMenuItemViewModel : MenuItemViewModel
    {
        public new bool IsCheckable
        {
            get
            {
                return true;
            }
        }        
        public bool IsSelected
        {
            get
            {
                return _is_selected;
            }
            set
            {
                _is_selected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }

    public class OpacityMenuItemViewModel : ToggleMenuItemViewModel
    {    
        public double Opacity { set; get; }
        public MenuItemViewModel opacity_parent_view_group { set; get; }        
        private void Execute()
        {
            foreach (var item in opacity_parent_view_group.MenuItems)
            {
                var opacity = item as OpacityMenuItemViewModel;
                opacity.IsSelected = false;
            }
        }
        public OpacityMenuItemViewModel()
        {
            _command = new CommandViewModel(Execute);
        }

        public new bool IsSelected
        {
            get
            {
                return main_window.opacityBind == Opacity;
            }
            set
            {
                if (value)
                {
                    main_window.opacityBind = Opacity;
                }
                OnPropertyChanged("IsSelected");
            }           
        }
        //static void MenuItemChecked(object sender, RoutedEventArgs e)
        //{

        //}
    }

    public class CommandViewModel : ICommand
    {
        private readonly Action _action;

        public CommandViewModel(Action action)
        {
            _action = action;
        }

        public void Execute(object o)
        {
            _action();
        }

        public bool CanExecute(object o)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
