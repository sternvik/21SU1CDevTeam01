using CommunityToolkit.Mvvm.ComponentModel;
using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationsLager.Models
{
    public partial class MenyModel : ObservableObject
    {
        [ObservableProperty] private int menyID;
        [ObservableProperty] private string rattnamn = string.Empty;
        [ObservableProperty] private string? beskrivning;
        [ObservableProperty] private decimal pris;
        [ObservableProperty] private string kategori = string.Empty;
        [ObservableProperty] private bool arGrundmeny = true;
        [ObservableProperty] private bool aktiv = true;

        public static MenyModel FromEntity(Meny entity)
        {
            return new MenyModel
            {
                MenyID = entity.MenyID,
                Rattnamn = entity.Rattnamn,
                Beskrivning = entity.Beskrivning,
                Pris = entity.Pris,
                Kategori = entity.Kategori,
                ArGrundmeny = entity.ArGrundmeny,
                Aktiv = entity.Aktiv
            };
        }

        public Meny ToEntity()
        {
            return new Meny
            {
                MenyID = this.MenyID,
                Rattnamn = this.Rattnamn,
                Beskrivning = this.Beskrivning,
                Pris = this.Pris,
                Kategori = this.Kategori,
                ArGrundmeny = this.ArGrundmeny,
                Aktiv = this.Aktiv
            };
        }
    }
}
