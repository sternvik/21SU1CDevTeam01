# RestoNation - Restauranghanteringssystem

## Om Projektet

RestoNation är ett restauranghanteringssystem utvecklat för att hantera en koncern med flera restauranger. Systemet hanterar bokningar, beställningar, kunder, lojalitetsprogram och bokföring.

---

## Systemöversikt

### Arkitektur
Projektet använder en **fyra-lagers arkitektur** för att separera ansvar:

1. **PresentationsLager** - WPF användargränssnitt med MVVM-mönster
2. **AffärsLager** - Affärslogik med controllers och services
3. **DataLager** - Databasåtkomst med Repository pattern
4. **EntitetsLager** - Datamodeller och entiteter

### Huvudfunktioner

- **Bokningar** - Boka bord för middag (kl 16:00-21:00)
- **Beställningar** - Hantera mat/dryck för middag, lunch, avhämtning, utkörning
- **Kunder** - Centraliserad kunddatabas med sökning
- **Lojalitetsprogram** - Tre nivåer (Brons/Silver/Guld) med poäng och rabatter
- **Meny** - Grundmeny + restaurangspecifika rätter
- **Bokföring** - Daglig export för ekonomi (Mat/Alkohol uppdelat)
- **Statistik** - Försäljning, rättspopularitet, personalprestation
- **Systemlogg** - Audit trail för alla användaraktiviteter

---

## Användarroller

### 1. VD
- Se koncernövergripande statistik
- Generera bokföring för alla restauranger
- Månadsrapporter i PDF-format
- Jämföra regioner och restauranger

### 2. Restaurangchef
- Hantera EN specifik restaurang
- Generera bokföring för sin restaurang
- Se statistik för personal
- Hantera lokala menyrätter

### 3. Personal/Servitör
- Skapa bokningar och beställningar
- Registrera kundankomst
- Ta betalningar och dricks
- Söka och lägga till kunder
- Tilldela lojalitetspoäng

### 4. Admin
- Skapa och hantera användare
- Hantera grundmenyn
- Hantera kunder
- Systemkonfiguration

---


### Inloggningsuppgifter (Test)

   ```
   
   VD:              vd / vd
   Restaurangchef:  chef / chef (Hemma 
   Servitör:        s / s
   Admin:           a / a

   ```

---

## Användardokumentation

### För Servitörer

#### Skapa Bokning
1. Klicka "Ny Bokning"
2. Sök kund (telefon/namn) eller lägg till ny
3. Välj datum, tid och antal gäster
4. Välj ledigt bord
5. Spara bokning

#### Registrera Ankomst
1. Hitta bokningen
2. Klicka "Markera Bekräftad"
3. Bordet blir "Aktivt"

#### Skapa Beställning
1. Öppna bokningsdetaljer
2. Lägg till maträtter från menyn
3. Granska summa (rabatt tillämpas automatiskt)
4. Spara beställning

#### Registrera Betalning
1. Öppna beställningsdetaljer
2. Ange dricks (om tillämpligt)
3. Klicka "Markera som Betald"
4. Systemet tilldelar automatiskt lojalitetspoäng

### För Restaurangchef

#### Generera Bokföring
1. Välj "Bokföring" i menyn
2. Välj datum
3. Klicka "Generera Bokföring"
4. Filen sparas i `Logg`-mappen
5. Skicka till ekonomi@restonation.se

#### Se Statistik
1. Välj "Statistik"
2. Välj period (dag/vecka/månad)
3. Granska försäljning, personal, rätter

### För VD

#### Koncernbokföring
1. Klicka "Generera Koncernbokföring"
2. Välj datum
3. Filen `Bokföring_KONCERN_[datum].txt` skapas
4. Innehåller alla 18 restaurangers data

---

## Lojalitetsprogram

### Nivåer

| Nivå   | Poäng      | Rabatt |
|--------|------------|--------|
| Brons  | 0-39       | 5%     |
| Silver | 40-74      | 10%    |
| Guld   | 75+        | 15%    |

### Poängtilldelning

- Middag: 15 poäng
- Lunch: 10 poäng
- Avhämtning: 10 poäng

### Belöningar (100 poäng)

- Gratis lunch, ELLER
- 50% rabatt på à la carte

---

## Databasstruktur

### Huvudentiteter

- **Anvandare** - Systemanvändare med roller
- **Kund** - Kunder med lojalitetspoäng
- **Restaurang** - 18 restauranger i 4 regioner
- **Bord** - Bord med status (Ledigt/Bokat/Aktivt/Betalt)
- **Bokning** - Bordsreservationer
- **Bestallning** - Mat/dryckesbeställningar
- **BestallningsRad** - Individuella rätter i beställning
- **Meny** - Menyrätter (Mat, Alkohol, Alkoholfritt)
- **Transaktion** - Betalningar (Mat/Alkohol uppdelat)
- **LojalitetsTransaktion** - Poänghistorik

---

## Teknisk Stack

### Frontend
- WPF (Windows Presentation Foundation)
- XAML
- MVVM (CommunityToolkit.Mvvm)
- LiveCharts.Wpf (grafer)

### Backend
- C# .NET 8.0
- Entity Framework Core 9.0.10
- SQL Server

### Services
- QuestPDF (PDF-rapporter)
- MailKit (email)

### Design Patterns
- Repository Pattern
- Unit of Work Pattern
- MVVM Pattern

---

## Viktiga Filer

### Services (AffärsLager/Services/)

- **BokföringsService.cs** - Genererar daglig bokföring
- **LojalitetsService.cs** - Hanterar lojalitetsprogram
- **StatistikService.cs** - Beräknar statistik
- **LoggService.cs** - Systemloggning
- **PDFService.cs** - PDF-rapporter

### Controllers (AffärsLager/Controllers/)

- **BokningsController.cs** - Bokningslogik
- **BestallningsController.cs** - Beställningslogik
- **KundController.cs** - Kundhantering
- **AnvandareController.cs** - Användarhantering

### Data (DataLager/)

- **ApplikationDbContext.cs** - EF Core context
- **UnitOfWork.cs** - Transaktionshantering och seeding
- **Repository.cs** - Generisk dataåtkomst

---
