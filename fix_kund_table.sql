-- Uppdatera Kunder tabellen för att tillåta NULL värden för Email och Telefon
-- Detta löser problemet med att skapa kunder utan email

ALTER TABLE [Kunder] ALTER COLUMN [Email] nvarchar(100) NULL;
ALTER TABLE [Kunder] ALTER COLUMN [Telefon] nvarchar(20) NULL;

-- Kontrollera ändringarna
SELECT TOP 5 KundID, Namn, Email, Telefon FROM [Kunder];