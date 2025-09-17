using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menyer",
                columns: table => new
                {
                    MenyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rattnamn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Beskrivning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Pris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArGrundmeny = table.Column<bool>(type: "bit", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menyer", x => x.MenyID);
                });

            migrationBuilder.CreateTable(
                name: "Regioner",
                columns: table => new
                {
                    RegionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Regionnamn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AntalRestauranger = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regioner", x => x.RegionID);
                });

            migrationBuilder.CreateTable(
                name: "Restauranger",
                columns: table => new
                {
                    RestaurangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Restaurangnamn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Oppettider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restauranger", x => x.RestaurangID);
                    table.ForeignKey(
                        name: "FK_Restauranger_Regioner_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regioner",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anvandare",
                columns: table => new
                {
                    AnvandarID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Anvandarnamn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Losenord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Namn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HemmarestaurangID = table.Column<int>(type: "int", nullable: true),
                    Roll = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anvandare", x => x.AnvandarID);
                    table.ForeignKey(
                        name: "FK_Anvandare_Restauranger_HemmarestaurangID",
                        column: x => x.HemmarestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID");
                });

            migrationBuilder.CreateTable(
                name: "Bord",
                columns: table => new
                {
                    BordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurangID = table.Column<int>(type: "int", nullable: false),
                    Bordkod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AntalPlatser = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bord", x => x.BordID);
                    table.ForeignKey(
                        name: "FK_Bord_Restauranger_RestaurangID",
                        column: x => x.RestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kunder",
                columns: table => new
                {
                    KundID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegionID = table.Column<int>(type: "int", nullable: true),
                    HemmarestaurangID = table.Column<int>(type: "int", nullable: true),
                    LojalitetsPoang = table.Column<int>(type: "int", nullable: false),
                    LojalitetsNiva = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SkapadDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunder", x => x.KundID);
                    table.ForeignKey(
                        name: "FK_Kunder_Regioner_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Regioner",
                        principalColumn: "RegionID");
                    table.ForeignKey(
                        name: "FK_Kunder_Restauranger_HemmarestaurangID",
                        column: x => x.HemmarestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID");
                });

            migrationBuilder.CreateTable(
                name: "RestaurangMenyer",
                columns: table => new
                {
                    RestaurangMenyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurangID = table.Column<int>(type: "int", nullable: false),
                    MenyID = table.Column<int>(type: "int", nullable: false),
                    LokalPris = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Tillganglig = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurangMenyer", x => x.RestaurangMenyID);
                    table.ForeignKey(
                        name: "FK_RestaurangMenyer_Menyer_MenyID",
                        column: x => x.MenyID,
                        principalTable: "Menyer",
                        principalColumn: "MenyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurangMenyer_Restauranger_RestaurangID",
                        column: x => x.RestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Systemloggar",
                columns: table => new
                {
                    LoggID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnvandarID = table.Column<int>(type: "int", nullable: true),
                    Modul = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Handelse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tid = table.Column<TimeSpan>(type: "time", nullable: false),
                    IPAdress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systemloggar", x => x.LoggID);
                    table.ForeignKey(
                        name: "FK_Systemloggar_Anvandare_AnvandarID",
                        column: x => x.AnvandarID,
                        principalTable: "Anvandare",
                        principalColumn: "AnvandarID");
                });

            migrationBuilder.CreateTable(
                name: "Bokningar",
                columns: table => new
                {
                    BokningsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KundID = table.Column<int>(type: "int", nullable: false),
                    BordID = table.Column<int>(type: "int", nullable: false),
                    RestaurangID = table.Column<int>(type: "int", nullable: false),
                    AnvandarID = table.Column<int>(type: "int", nullable: true),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tid = table.Column<TimeSpan>(type: "time", nullable: false),
                    AntalGaster = table.Column<int>(type: "int", nullable: false),
                    Specialinformation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BokningsTyp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SkapadDatum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bokningar", x => x.BokningsID);
                    table.ForeignKey(
                        name: "FK_Bokningar_Anvandare_AnvandarID",
                        column: x => x.AnvandarID,
                        principalTable: "Anvandare",
                        principalColumn: "AnvandarID");
                    table.ForeignKey(
                        name: "FK_Bokningar_Bord_BordID",
                        column: x => x.BordID,
                        principalTable: "Bord",
                        principalColumn: "BordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bokningar_Kunder_KundID",
                        column: x => x.KundID,
                        principalTable: "Kunder",
                        principalColumn: "KundID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bokningar_Restauranger_RestaurangID",
                        column: x => x.RestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bestallningar",
                columns: table => new
                {
                    BestallningsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BokningsID = table.Column<int>(type: "int", nullable: true),
                    KundID = table.Column<int>(type: "int", nullable: false),
                    RestaurangID = table.Column<int>(type: "int", nullable: false),
                    AnvandarID = table.Column<int>(type: "int", nullable: false),
                    BestallningsTyp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Utkorare = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalSumma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Betald = table.Column<bool>(type: "bit", nullable: false),
                    PoangTilldelas = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tid = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bestallningar", x => x.BestallningsID);
                    table.ForeignKey(
                        name: "FK_Bestallningar_Anvandare_AnvandarID",
                        column: x => x.AnvandarID,
                        principalTable: "Anvandare",
                        principalColumn: "AnvandarID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bestallningar_Bokningar_BokningsID",
                        column: x => x.BokningsID,
                        principalTable: "Bokningar",
                        principalColumn: "BokningsID");
                    table.ForeignKey(
                        name: "FK_Bestallningar_Kunder_KundID",
                        column: x => x.KundID,
                        principalTable: "Kunder",
                        principalColumn: "KundID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bestallningar_Restauranger_RestaurangID",
                        column: x => x.RestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BestallningsRader",
                columns: table => new
                {
                    BestallningsRadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestallningsID = table.Column<int>(type: "int", nullable: false),
                    MenyID = table.Column<int>(type: "int", nullable: false),
                    Antal = table.Column<int>(type: "int", nullable: false),
                    Pris = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Summa = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestallningsRader", x => x.BestallningsRadID);
                    table.ForeignKey(
                        name: "FK_BestallningsRader_Bestallningar_BestallningsID",
                        column: x => x.BestallningsID,
                        principalTable: "Bestallningar",
                        principalColumn: "BestallningsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BestallningsRader_Menyer_MenyID",
                        column: x => x.MenyID,
                        principalTable: "Menyer",
                        principalColumn: "MenyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LojalitetsTransaktioner",
                columns: table => new
                {
                    LojalitetsTransaktionsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KundID = table.Column<int>(type: "int", nullable: false),
                    BestallningsID = table.Column<int>(type: "int", nullable: true),
                    PoangTillagda = table.Column<int>(type: "int", nullable: false),
                    PoangAnvanda = table.Column<int>(type: "int", nullable: false),
                    PoangSaldo = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LojalitetsTransaktioner", x => x.LojalitetsTransaktionsID);
                    table.ForeignKey(
                        name: "FK_LojalitetsTransaktioner_Bestallningar_BestallningsID",
                        column: x => x.BestallningsID,
                        principalTable: "Bestallningar",
                        principalColumn: "BestallningsID");
                    table.ForeignKey(
                        name: "FK_LojalitetsTransaktioner_Kunder_KundID",
                        column: x => x.KundID,
                        principalTable: "Kunder",
                        principalColumn: "KundID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaktioner",
                columns: table => new
                {
                    TransaktionsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestallningsID = table.Column<int>(type: "int", nullable: false),
                    RestaurangID = table.Column<int>(type: "int", nullable: false),
                    AnvandarID = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatSumma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AlkoholSumma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moms = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSumma = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaktioner", x => x.TransaktionsID);
                    table.ForeignKey(
                        name: "FK_Transaktioner_Anvandare_AnvandarID",
                        column: x => x.AnvandarID,
                        principalTable: "Anvandare",
                        principalColumn: "AnvandarID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaktioner_Bestallningar_BestallningsID",
                        column: x => x.BestallningsID,
                        principalTable: "Bestallningar",
                        principalColumn: "BestallningsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaktioner_Restauranger_RestaurangID",
                        column: x => x.RestaurangID,
                        principalTable: "Restauranger",
                        principalColumn: "RestaurangID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Anvandare",
                columns: new[] { "AnvandarID", "Aktiv", "Anvandarnamn", "HemmarestaurangID", "Losenord", "Namn", "Roll" },
                values: new object[] { 4, true, "vd", null, "vd123", "Sten Hård", "VD" });

            migrationBuilder.InsertData(
                table: "Menyer",
                columns: new[] { "MenyID", "Aktiv", "ArGrundmeny", "Beskrivning", "Kategori", "Pris", "Rattnamn" },
                values: new object[,]
                {
                    { 1, true, true, "Traditionella svenska köttbullar med potatismos", "Mat", 165m, "Klassisk köttbullar" },
                    { 2, true, true, "Husmarinerad gravlax med hovmästarsås", "Mat", 185m, "Gravlax" },
                    { 3, true, true, "Varierar dagligen", "Mat", 125m, "Dagens lunch" },
                    { 4, true, true, "50cl", "Alkoholhaltig dryck", 75m, "Öl (stor stark)" },
                    { 5, true, true, "Bryggkaffe", "Alkoholfri dryck", 35m, "Kaffe" },
                    { 6, true, true, "Coca-Cola, Fanta eller Sprite", "Alkoholfri dryck", 45m, "Läsk" }
                });

            migrationBuilder.InsertData(
                table: "Regioner",
                columns: new[] { "RegionID", "AntalRestauranger", "Regionnamn" },
                values: new object[,]
                {
                    { 1, 2, "Norr" },
                    { 2, 7, "Öst" },
                    { 3, 5, "Väst" },
                    { 4, 4, "Syd" }
                });

            migrationBuilder.InsertData(
                table: "Restauranger",
                columns: new[] { "RestaurangID", "Adress", "Oppettider", "RegionID", "Restaurangnamn", "Telefon" },
                values: new object[,]
                {
                    { 1, "Kungsgatan 1, Stockholm", "10:30-23:00", 2, "RestoNation Stockholm City", "08-123456" },
                    { 2, "Avenyn 10, Göteborg", "10:30-23:00", 3, "RestoNation Göteborg", "031-789012" },
                    { 3, "Stortorget 5, Malmö", "10:30-23:00", 4, "RestoNation Malmö", "040-345678" }
                });

            migrationBuilder.InsertData(
                table: "Anvandare",
                columns: new[] { "AnvandarID", "Aktiv", "Anvandarnamn", "HemmarestaurangID", "Losenord", "Namn", "Roll" },
                values: new object[,]
                {
                    { 1, true, "servitor1", 1, "password123", "Anna Servitör", "Servitör" },
                    { 2, true, "admin1", 1, "admin123", "Erik Admin", "Admin" },
                    { 3, true, "rchef1", 1, "chef123", "Maria Restaurangchef", "Restaurangchef" }
                });

            migrationBuilder.InsertData(
                table: "Bord",
                columns: new[] { "BordID", "AntalPlatser", "Bordkod", "RestaurangID", "Status" },
                values: new object[,]
                {
                    { 1, 2, "STH001", 1, "Ledigt" },
                    { 2, 4, "STH002", 1, "Ledigt" },
                    { 3, 6, "STH003", 1, "Ledigt" },
                    { 4, 2, "GBG001", 2, "Ledigt" },
                    { 5, 4, "GBG002", 2, "Ledigt" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anvandare_HemmarestaurangID",
                table: "Anvandare",
                column: "HemmarestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_Bestallningar_AnvandarID",
                table: "Bestallningar",
                column: "AnvandarID");

            migrationBuilder.CreateIndex(
                name: "IX_Bestallningar_BokningsID",
                table: "Bestallningar",
                column: "BokningsID");

            migrationBuilder.CreateIndex(
                name: "IX_Bestallningar_KundID",
                table: "Bestallningar",
                column: "KundID");

            migrationBuilder.CreateIndex(
                name: "IX_Bestallningar_RestaurangID",
                table: "Bestallningar",
                column: "RestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_BestallningsRader_BestallningsID",
                table: "BestallningsRader",
                column: "BestallningsID");

            migrationBuilder.CreateIndex(
                name: "IX_BestallningsRader_MenyID",
                table: "BestallningsRader",
                column: "MenyID");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_AnvandarID",
                table: "Bokningar",
                column: "AnvandarID");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_BordID",
                table: "Bokningar",
                column: "BordID");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_KundID",
                table: "Bokningar",
                column: "KundID");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_RestaurangID",
                table: "Bokningar",
                column: "RestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_Bord_RestaurangID",
                table: "Bord",
                column: "RestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_Kunder_HemmarestaurangID",
                table: "Kunder",
                column: "HemmarestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_Kunder_RegionID",
                table: "Kunder",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_LojalitetsTransaktioner_BestallningsID",
                table: "LojalitetsTransaktioner",
                column: "BestallningsID");

            migrationBuilder.CreateIndex(
                name: "IX_LojalitetsTransaktioner_KundID",
                table: "LojalitetsTransaktioner",
                column: "KundID");

            migrationBuilder.CreateIndex(
                name: "IX_Restauranger_RegionID",
                table: "Restauranger",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurangMenyer_MenyID",
                table: "RestaurangMenyer",
                column: "MenyID");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurangMenyer_RestaurangID",
                table: "RestaurangMenyer",
                column: "RestaurangID");

            migrationBuilder.CreateIndex(
                name: "IX_Systemloggar_AnvandarID",
                table: "Systemloggar",
                column: "AnvandarID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaktioner_AnvandarID",
                table: "Transaktioner",
                column: "AnvandarID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaktioner_BestallningsID",
                table: "Transaktioner",
                column: "BestallningsID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaktioner_RestaurangID",
                table: "Transaktioner",
                column: "RestaurangID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BestallningsRader");

            migrationBuilder.DropTable(
                name: "LojalitetsTransaktioner");

            migrationBuilder.DropTable(
                name: "RestaurangMenyer");

            migrationBuilder.DropTable(
                name: "Systemloggar");

            migrationBuilder.DropTable(
                name: "Transaktioner");

            migrationBuilder.DropTable(
                name: "Menyer");

            migrationBuilder.DropTable(
                name: "Bestallningar");

            migrationBuilder.DropTable(
                name: "Bokningar");

            migrationBuilder.DropTable(
                name: "Anvandare");

            migrationBuilder.DropTable(
                name: "Bord");

            migrationBuilder.DropTable(
                name: "Kunder");

            migrationBuilder.DropTable(
                name: "Restauranger");

            migrationBuilder.DropTable(
                name: "Regioner");
        }
    }
}
