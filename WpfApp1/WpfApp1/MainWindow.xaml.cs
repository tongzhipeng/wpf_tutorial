using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

    [Serializable]
    public class WindowStat
    {
        public double opacity;
        public string secondColumnWidth;
        public double Left;
        public double Top;
    }
    public static class DebounceClass
    {
        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 2000)
        {
            var last = 0;
            return arg =>
            {
                var current = Interlocked.Increment(ref last);
                Task.Delay(milliseconds).ContinueWith(task =>
                {
                    if (current == last) func(arg);
                    task.Dispose();
                });
            };
        }
    }

    public struct ItemValue
    {
        double current_value;
        double warning_value;
    }
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static Dictionary<string, double> perf_values;//性能数据
        public static Dictionary<string, double> perf_thredhold_values;//报警阈值
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
            if (name == "opacityBind" || name == "secondColumnWidthBind")
            {
                SaveUIConfig();
            }            
        }

        public static void UpdatePerfValue(string name, double value)
        {
            perf_values[name] = value;
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
        private string secondColumnWidth = "0*";
        public string secondColumnWidthBind
        {
            get 
            {
                return secondColumnWidth;
            }
            set
            {
                secondColumnWidth = value;
                OnPropertyChanged("secondColumnWidthBind");
            }
        }

        void SaveUIConfig()
        {
            using (FileStream fs = new FileStream("UI.dat", FileMode.Create))
            {
                WindowStat stat = new WindowStat { opacity = opacity, secondColumnWidth = secondColumnWidth, Left = Left, Top = Top };
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, stat);                                
            }
        }
        void LoadUIConfig()
        {
            try
            {
                using (FileStream fs = new FileStream("UI.dat", FileMode.Open))
                {                    
                    BinaryFormatter bf = new BinaryFormatter();
                    WindowStat stat = bf.Deserialize(fs) as WindowStat;
                    opacity = stat.opacity;
                    secondColumnWidth = stat.secondColumnWidth;
                    WindowStartupLocation = WindowStartupLocation.Manual;
                    Left = stat.Left;
                    Top = stat.Top;
                }
            }
            catch (FileNotFoundException e)
            {

            }
            catch (SerializationException e)
            {

            }

        }

        public void ShowMore()
        {
            secondColumnWidthBind = "25*";
        }
        public void HideMore()
        {
            secondColumnWidthBind = "0*";
        }
        public bool IsShowMore()
        {
            return secondColumnWidthBind != "0*";
        }
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
        public MainWindow()
        {
            LoadUIConfig();
            InitializeComponent();
            MouseDown += Window_MouseLeftButtonDown;
            LocationChanged += MainWindow_LocationChanged;
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
                            new ToggleMenuItemViewModel { Header = "显示更多", Name="ShowMore", main_window = main_window, IsSelected = IsShowMore() }
                        };


            DataContext = this;        
        }
        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            
            Action<int> a = (arg)=>
            {
                this.Dispatcher.Invoke(SaveUIConfig);                
            };
            var debouncedWrapper = DebounceClass.Debounce<int>(a);
            debouncedWrapper(0);

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
        public ToggleMenuItemViewModel()
        {

        }
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
