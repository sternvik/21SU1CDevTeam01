using System;

namespace AffärsLager.DTOs
{
    /// <summary>
    /// DTO för personalstatistik
    /// </summary>
    public class PersonalStatistikDto
    {
        public int AnvandarID { get; set; }
        public string PersonalNamn { get; set; } = string.Empty;
        public int RestaurangID { get; set; }
        public string RestaurangNamn { get; set; } = string.Empty;
        public int AntalBokningar { get; set; }
        public int AntalBordHanterade { get; set; }
        public int TotaltAntalGaster { get; set; }
        public decimal TotalForsaljning { get; set; }
        public DateTime Period { get; set; }
    }

    /// <summary>
    /// DTO för menystatistik
    /// </summary>
    public class MenyStatistikDto
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int AntalSalda { get; set; }
        public decimal TotalForsaljning { get; set; }
        public decimal GenomsnittsPris { get; set; }
        public bool ArGrundmeny { get; set; }
        public int RestaurangID { get; set; }
        public string? RestaurangNamn { get; set; }
    }

    /// <summary>
    /// DTO för försäljningssammanfattning
    /// </summary>
    public class ForsaljningsSummaryDto
    {
        public int RestaurangID { get; set; }
        public string RestaurangNamn { get; set; } = string.Empty;
        public DateTime FranDatum { get; set; }
        public DateTime TillDatum { get; set; }
        
        // Totaler
        public decimal TotalForsaljning { get; set; }
        public int TotaltAntalBestallningar { get; set; }
        public int TotaltAntalBokningar { get; set; }
        public int TotaltAntalGaster { get; set; }
        
        // Mat vs Alkohol
        public decimal MatForsaljning { get; set; }
        public decimal DryckForsaljning { get; set; }
        
        // Populära rätter
        public List<MenyStatistikDto> MestSaldaRatter { get; set; } = new();
        public List<MenyStatistikDto> MinstSaldaRatter { get; set; } = new();
        
        // Personal
        public List<PersonalStatistikDto> PersonalStatistik { get; set; } = new();
    }

    /// <summary>
    /// DTO för bokföringsdata
    /// </summary>
    public class BokforingDto
    {
        public DateTime Datum { get; set; }
        public int RestaurangID { get; set; }
        public string RestaurangNamn { get; set; } = string.Empty;
        public decimal Dagssumma { get; set; }
        public int AntalTransaktioner { get; set; }
        public decimal Dricks { get; set; }
        public decimal KontantBetalningar { get; set; }
        public decimal KortBetalningar { get; set; }
        public decimal LojalitetsPoangAnvanda { get; set; }
    }

    /// <summary>
    /// DTO för grundmeny statistik (alla restauranger)
    /// </summary>
    public class GrundmenyStatistikDto
    {
        public int MenyID { get; set; }
        public string Rattnamn { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int TotaltAntalSalda { get; set; }
        public decimal TotalForsaljning { get; set; }
        public Dictionary<string, int> ForsaljningPerRestaurang { get; set; } = new();
    }
}
