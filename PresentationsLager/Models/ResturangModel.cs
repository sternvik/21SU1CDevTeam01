using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using PresentationsLager.Models.PresentationsLager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PresentationsLager.Models
{
    public partial class RestaurangModel : ObservableObject
    {
        [ObservableProperty] private int restaurangID;
        [ObservableProperty] private string restaurangnamn = string.Empty;
        [ObservableProperty] private int regionID;
        [ObservableProperty] private string? adress;
        [ObservableProperty] private string? telefon;
        [ObservableProperty] private string oppettider = "10:30-23:00"; // Default value

        [ObservableProperty] private Region? region;

        [ObservableProperty] private ObservableCollection<BordModel> bord = new();
        [ObservableProperty] private ObservableCollection<BokningModel> bokningar = new();
        [ObservableProperty] private ObservableCollection<BestallningModel> bestallningar = new();
        [ObservableProperty] private ObservableCollection<AnvandareModel> anvandare = new();
        [ObservableProperty] private ObservableCollection<RestaurangMenyModel> restaurangMenyer = new();

        public static RestaurangModel FromEntity(Restaurang entity) => new()
        {
            RestaurangID = entity.RestaurangID,
            Restaurangnamn = entity.Restaurangnamn,
            RegionID = entity.RegionID,
            Adress = entity.Adress,
            Telefon = entity.Telefon,
            Oppettider = entity.Oppettider,
            Region = entity.Region,
            Bord = new ObservableCollection<BordModel>(entity.Bord.Select(BordModel.FromEntity)),
            Bokningar = new ObservableCollection<BokningModel>(entity.Bokningar.Select(BokningModel.FromEntity)),
            Bestallningar = new ObservableCollection<BestallningModel>(entity.Bestallningar.Select(BestallningModel.FromEntity)),
            Anvandare = new ObservableCollection<AnvandareModel>(entity.Anvandare.Select(AnvandareModel.FromEntity)),
            RestaurangMenyer = new ObservableCollection<RestaurangMenyModel>(entity.RestaurangMenyer.Select(RestaurangMenyModel.FromEntity))
        };

        public Restaurang ToEntity() => new()
        {
            RestaurangID = RestaurangID,
            Restaurangnamn = Restaurangnamn,
            RegionID = RegionID,
            Adress = Adress,
            Telefon = Telefon,
            Oppettider = Oppettider,
            Region = Region,
            Bord = Bord.Select(b => b.ToEntity()).ToList(),
            Bokningar = Bokningar.Select(b => b.ToEntity()).ToList(),
            Bestallningar = Bestallningar.Select(b => b.ToEntity()).ToList(),
            Anvandare = Anvandare.Select(a => a.ToEntity()).ToList(),
            RestaurangMenyer = RestaurangMenyer.Select(rm => rm.ToEntity()).ToList()
        };
    }
}
