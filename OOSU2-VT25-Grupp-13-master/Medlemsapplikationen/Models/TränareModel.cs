using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace Medlemsapplikationen.Models
{
    public partial class TränareModel : ObservableObject
    {
       [ObservableProperty]
       private int tränareID;

       [ObservableProperty]
       private string namn;

       [ObservableProperty]
       private string specialisering;

       [ObservableProperty]
       private ObservableCollection<TräningspassModel> träningspass = new();
    }
}
