using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication14
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel { Header = "Alpha" },
                new MenuItemViewModel { Header = "Beta",
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                        {
                            new MenuItemViewModel { Header = "Beta1", Name = "_beta1" },
                            new MenuItemViewModel { Header = "Beta2", Name="_beta2",
                                MenuItems = new ObservableCollection<MenuItemViewModel>
                                {
                                    new MenuItemViewModel { Header = "Beta1a", Name = "_beta1a" },
                                    new MenuItemViewModel { Header = "Beta1b", Name = "_beta1b" },
                                    new MenuItemViewModel { Header = "Beta1c", Name = "_beta1c" }
                                }
                            },
                            new MenuItemViewModel { Header = "Beta3", Name="_beta3" }
                        }
                },
                new MenuItemViewModel { Header = "Gamma", Name="_gamma" }
            };

            DataContext = this;
        }
    }

    public class MenuItemViewModel
    {
        private readonly ICommand _command;

        public MenuItemViewModel()
        {
            _command = new CommandViewModel(Execute);
        }

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

        private void Execute()
        {
            // (NOTE: In a view model, you normally should not use MessageBox.Show()).
            MessageBox.Show("Clicked at " + Name);
        }
        static void HandleCommand(string menu_name)
        {
            
        }
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