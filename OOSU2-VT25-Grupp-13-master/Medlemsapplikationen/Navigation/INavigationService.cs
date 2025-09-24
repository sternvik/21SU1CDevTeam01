using System.Windows;

namespace Medlemsapplikationen.Navigation
{
    public interface INavigationService
    {
        void CloseCurrentWindow();
        void NavigateTo<T>() where T : Window, new();
    }
}