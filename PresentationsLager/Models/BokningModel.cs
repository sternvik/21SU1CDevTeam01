using EntitetsLager;
using System;

namespace PresentationsLager.Models
{
    public class BokningModel
    {
        public int BokningsID { get; set; }
        public int KundID { get; set; }
        public int BordID { get; set; }
        public int RestaurangID { get; set; }
        public int? AnvandarID { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan Tid { get; set; }
        public int AntalGaster { get; set; }
        public string? Specialinformation { get; set; }
        public string BokningsTyp { get; set; } = string.Empty;
        public string Status { get; set; } = "Bokad";
        public DateTime SkapadDatum { get; set; } = DateTime.Now;

        public KundModel? Kund { get; set; }
        public BordModel? Bord { get; set; }
        public RestaurangModel? Restaurang { get; set; }
        public AnvandareModel? Anvandare { get; set; }

        public static BokningModel FromEntity(Bokning entity) => new()
        {
            BokningsID = entity.BokningsID,
            KundID = entity.KundID,
            BordID = entity.BordID,
            RestaurangID = entity.RestaurangID,
            AnvandarID = entity.AnvandarID,
            Datum = entity.Datum,
            Tid = entity.Tid,
            AntalGaster = entity.AntalGaster,
            Specialinformation = entity.Specialinformation,
            BokningsTyp = entity.BokningsTyp,
            Status = entity.Status,
            SkapadDatum = entity.SkapadDatum,
            Kund = entity.Kund != null ? KundModel.FromEntity(entity.Kund) : null,
            Bord = entity.Bord != null ? BordModel.FromEntity(entity.Bord) : null,
            Restaurang = entity.Restaurang != null ? RestaurangModel.FromEntity(entity.Restaurang) : null,
            Anvandare = entity.Anvandare != null ? AnvandareModel.FromEntity(entity.Anvandare) : null
        };

        public Bokning ToEntity() => new()
        {
            BokningsID = BokningsID,
            KundID = KundID,
            BordID = BordID,
            RestaurangID = RestaurangID,
            AnvandarID = AnvandarID,
            Datum = Datum,
            Tid = Tid,
            AntalGaster = AntalGaster,
            Specialinformation = Specialinformation,
            BokningsTyp = BokningsTyp,
            Status = Status,
            SkapadDatum = SkapadDatum
        };
    }
}
