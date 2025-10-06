-- Kolla alla bokningar för bord N1001 och N1018
USE suht2501;
GO

-- Hitta bord-ID för N1001 och N1018
SELECT BordID, Bordkod, RestaurangID
FROM Bord
WHERE Bordkod IN ('N1001', 'N1018')
ORDER BY Bordkod;

-- Visa alla bokningar för dessa bord (inklusive avslutade)
SELECT
    b.BokningsID,
    bord.Bordkod,
    b.Datum,
    b.Tid,
    b.Status,
    b.AntalGaster,
    k.Namn as KundNamn,
    b.SkapadDatum
FROM Bokningar b
INNER JOIN Bord bord ON b.BordID = bord.BordID
INNER JOIN Kunder k ON b.KundID = k.KundID
WHERE bord.Bordkod IN ('N1001', 'N1003', 'N1018')
ORDER BY bord.Bordkod, b.Datum DESC, b.Tid DESC;

-- Räkna antalet bokningar per status
SELECT
    bord.Bordkod,
    b.Status,
    COUNT(*) as Antal
FROM Bokningar b
INNER JOIN Bord bord ON b.BordID = bord.BordID
WHERE bord.Bordkod IN ('N1001', 'N1003', 'N1018')
GROUP BY bord.Bordkod, b.Status
ORDER BY bord.Bordkod, b.Status;
