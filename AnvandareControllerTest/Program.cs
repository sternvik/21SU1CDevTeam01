using AffärsLager.Controllers;
using DataLager;
using System;

Console.WriteLine("=== RestoNation AnvandareController Test ===\n");

var unitOfWork = new UnitOfWork();
var anvandareController = new AnvandareController(unitOfWork);

// Test 1: Autentisering med korrekt användare från seed data
Console.WriteLine("Test 1: Autentisering med servitor1/password123");
bool authSuccess = anvandareController.AutentiseraAnvandare("servitor1", "password123");
Console.WriteLine($"Resultat: {(authSuccess ? "LYCKAD" : "MISSLYCKAD")}\n");

// Test 2: Hämta inloggad användare
if (authSuccess)
{
    Console.WriteLine("Test 2: Hämta inloggad användare 'servitor1'");
    var anvandare = anvandareController.HamtaInloggadAnvandare("servitor1");
    if (anvandare != null)
    {
        Console.WriteLine($"✓ Användare: {anvandare.Namn}");
        Console.WriteLine($"✓ Roll: {anvandare.Roll}");
        Console.WriteLine($"✓ Hemmarestaurang ID: {anvandare.HemmarestaurangID}\n");
    }
}

// Test 3: Rollkontroll
Console.WriteLine("Test 3: Rollkontroll");
var servitor = anvandareController.HamtaInloggadAnvandare("servitor1");
if (servitor != null)
{
    bool ärServitor = anvandareController.HarRoll(servitor.AnvandarID, "Servitör");
    bool ärAdmin = anvandareController.HarRoll(servitor.AnvandarID, "Admin");
    Console.WriteLine($"✓ Har servitör-roll: {ärServitor}");
    Console.WriteLine($"✓ Har admin-roll: {ärAdmin}\n");
}

// Test 4: Testa admin-användare
Console.WriteLine("Test 4: Testa admin-användare (admin1/admin123)");
bool adminAuth = anvandareController.AutentiseraAnvandare("admin1", "admin123");
if (adminAuth)
{
    var admin = anvandareController.HamtaInloggadAnvandare("admin1");
    if (admin != null)
    {
        Console.WriteLine($"✓ Admin-användare: {admin.Namn}");
        Console.WriteLine($"✓ Roll: {admin.Roll}");
        bool ärAdmin = anvandareController.HarRoll(admin.AnvandarID, "Admin");
        Console.WriteLine($"✓ Har admin-roll: {ärAdmin}\n");
    }
}

// Test 5: Felaktig autentisering
Console.WriteLine("Test 5: Felaktig autentisering");
bool failAuth = anvandareController.AutentiseraAnvandare("felnamn", "fellösenord");
Console.WriteLine($"Resultat (ska vara false): {failAuth}\n");

// Test 6: Lista alla användare
Console.WriteLine("Test 6: Lista alla aktiva användare");
var allaAnvandare = anvandareController.HamtaAllaAktiviraAnvandare();
Console.WriteLine($"Antal aktiva användare: {allaAnvandare.Count}");
foreach (var user in allaAnvandare)
{
    Console.WriteLine($"  - {user.Anvandarnamn} ({user.Roll})");
}

Console.WriteLine("\n=== Test slutfört ===");
Console.WriteLine("Tryck på valfri tangent för att avsluta...");
Console.ReadKey();
