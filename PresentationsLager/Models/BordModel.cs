using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using EntitetsLager;

    namespace PresentationsLager.Models
    {
        public partial class BordModel : ObservableObject
        {
            [ObservableProperty] private int bordID;
            [ObservableProperty] private int restaurangID;
            [ObservableProperty] private string bordkod = string.Empty;
            [ObservableProperty] private int antalPlatser;
            [ObservableProperty] private string status = "Ledigt";
            [ObservableProperty] private Restaurang? restaurang;

            public static BordModel FromEntity(Bord entity) => new()
            {
                BordID = entity.BordID,
                RestaurangID = entity.RestaurangID,
                Bordkod = entity.Bordkod,
                AntalPlatser = entity.AntalPlatser,
                Status = entity.Status,
                Restaurang = entity.Restaurang
            };

            public Bord ToEntity() => new()
            {
                BordID = BordID,
                RestaurangID = RestaurangID,
                Bordkod = Bordkod,
                AntalPlatser = AntalPlatser,
                Status = Status,
                Restaurang = Restaurang
            };
        }
    }

}
