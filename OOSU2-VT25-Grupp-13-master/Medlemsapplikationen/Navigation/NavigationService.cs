using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows;

namespace Medlemsapplikationen.Navigation
{
    public class NavigationService : INavigationService
    {
        public void NavigateTo<T>() where T : Window, new()
        {
            new T().Show();
            CloseCurrentWindow();
        }

        public void CloseCurrentWindow()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();
        }
    }
}
