using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKundSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Anvandare_AnvandarID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Kunder_KundID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Restauranger_RestaurangID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_BestallningsRader_Bestallningar_BestallningsID1",
                table: "BestallningsRader");

            migrationBuilder.DropForeignKey(
                name: "FK_BestallningsRader_Menyer_MenyID",
                table: "BestallningsRader");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Bord_BordID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Kunder_KundID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Restauranger_RestaurangID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bord_Restauranger_RestaurangID",
                table: "Bord");

            migrationBuilder.DropForeignKey(
                name: "FK_LojalitetsTransaktioner_Kunder_KundID",
                table: "LojalitetsTransaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurangMenyer_Menyer_MenyID",
                table: "RestaurangMenyer");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurangMenyer_Restauranger_RestaurangID",
                table: "RestaurangMenyer");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Anvandare_AnvandarID",
                table: "Transaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Bestallningar_BestallningsID",
                table: "Transaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Restauranger_RestaurangID",
                table: "Transaktioner");

            migrationBuilder.DropIndex(
                name: "IX_BestallningsRader_BestallningsID1",
                table: "BestallningsRader");

            migrationBuilder.DropColumn(
                name: "BestallningsID1",
                table: "BestallningsRader");

            migrationBuilder.AlterColumn<string>(
                name: "Telefon",
                table: "Kunder",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Kunder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Restauranger_RegionID",
                table: "Restauranger",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_BestallningsRader_BestallningsID",
                table: "BestallningsRader",
                column: "BestallningsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Anvandare_AnvandarID",
                table: "Bestallningar",
                column: "AnvandarID",
                principalTable: "Anvandare",
                principalColumn: "AnvandarID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Kunder_KundID",
                table: "Bestallningar",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Restauranger_RestaurangID",
                table: "Bestallningar",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID");

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningsRader_Bestallningar_BestallningsID",
                table: "BestallningsRader",
                column: "BestallningsID",
                principalTable: "Bestallningar",
                principalColumn: "BestallningsID");

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningsRader_Menyer_MenyID",
                table: "BestallningsRader",
                column: "MenyID",
                principalTable: "Menyer",
                principalColumn: "MenyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Bord_BordID",
                table: "Bokningar",
                column: "BordID",
                principalTable: "Bord",
                principalColumn: "BordID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Kunder_KundID",
                table: "Bokningar",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Restauranger_RestaurangID",
                table: "Bokningar",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bord_Restauranger_RestaurangID",
                table: "Bord",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID");

            migrationBuilder.AddForeignKey(
                name: "FK_LojalitetsTransaktioner_Kunder_KundID",
                table: "LojalitetsTransaktioner",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID");

            migrationBuilder.AddForeignKey(
                name: "FK_Restauranger_Regioner_RegionID",
                table: "Restauranger",
                column: "RegionID",
                principalTable: "Regioner",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurangMenyer_Menyer_MenyID",
                table: "RestaurangMenyer",
                column: "MenyID",
                principalTable: "Menyer",
                principalColumn: "MenyID");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurangMenyer_Restauranger_RestaurangID",
                table: "RestaurangMenyer",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Anvandare_AnvandarID",
                table: "Transaktioner",
                column: "AnvandarID",
                principalTable: "Anvandare",
                principalColumn: "AnvandarID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Bestallningar_BestallningsID",
                table: "Transaktioner",
                column: "BestallningsID",
                principalTable: "Bestallningar",
                principalColumn: "BestallningsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Restauranger_RestaurangID",
                table: "Transaktioner",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Anvandare_AnvandarID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Kunder_KundID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bestallningar_Restauranger_RestaurangID",
                table: "Bestallningar");

            migrationBuilder.DropForeignKey(
                name: "FK_BestallningsRader_Bestallningar_BestallningsID",
                table: "BestallningsRader");

            migrationBuilder.DropForeignKey(
                name: "FK_BestallningsRader_Menyer_MenyID",
                table: "BestallningsRader");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Bord_BordID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Kunder_KundID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Restauranger_RestaurangID",
                table: "Bokningar");

            migrationBuilder.DropForeignKey(
                name: "FK_Bord_Restauranger_RestaurangID",
                table: "Bord");

            migrationBuilder.DropForeignKey(
                name: "FK_LojalitetsTransaktioner_Kunder_KundID",
                table: "LojalitetsTransaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_Restauranger_Regioner_RegionID",
                table: "Restauranger");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurangMenyer_Menyer_MenyID",
                table: "RestaurangMenyer");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurangMenyer_Restauranger_RestaurangID",
                table: "RestaurangMenyer");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Anvandare_AnvandarID",
                table: "Transaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Bestallningar_BestallningsID",
                table: "Transaktioner");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaktioner_Restauranger_RestaurangID",
                table: "Transaktioner");

            migrationBuilder.DropIndex(
                name: "IX_Restauranger_RegionID",
                table: "Restauranger");

            migrationBuilder.DropIndex(
                name: "IX_BestallningsRader_BestallningsID",
                table: "BestallningsRader");

            migrationBuilder.AlterColumn<string>(
                name: "Telefon",
                table: "Kunder",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Kunder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BestallningsID1",
                table: "BestallningsRader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BestallningsRader_BestallningsID1",
                table: "BestallningsRader",
                column: "BestallningsID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Anvandare_AnvandarID",
                table: "Bestallningar",
                column: "AnvandarID",
                principalTable: "Anvandare",
                principalColumn: "AnvandarID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Kunder_KundID",
                table: "Bestallningar",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bestallningar_Restauranger_RestaurangID",
                table: "Bestallningar",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningsRader_Bestallningar_BestallningsID1",
                table: "BestallningsRader",
                column: "BestallningsID1",
                principalTable: "Bestallningar",
                principalColumn: "BestallningsID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BestallningsRader_Menyer_MenyID",
                table: "BestallningsRader",
                column: "MenyID",
                principalTable: "Menyer",
                principalColumn: "MenyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Bord_BordID",
                table: "Bokningar",
                column: "BordID",
                principalTable: "Bord",
                principalColumn: "BordID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Kunder_KundID",
                table: "Bokningar",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Restauranger_RestaurangID",
                table: "Bokningar",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bord_Restauranger_RestaurangID",
                table: "Bord",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LojalitetsTransaktioner_Kunder_KundID",
                table: "LojalitetsTransaktioner",
                column: "KundID",
                principalTable: "Kunder",
                principalColumn: "KundID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurangMenyer_Menyer_MenyID",
                table: "RestaurangMenyer",
                column: "MenyID",
                principalTable: "Menyer",
                principalColumn: "MenyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurangMenyer_Restauranger_RestaurangID",
                table: "RestaurangMenyer",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Anvandare_AnvandarID",
                table: "Transaktioner",
                column: "AnvandarID",
                principalTable: "Anvandare",
                principalColumn: "AnvandarID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Bestallningar_BestallningsID",
                table: "Transaktioner",
                column: "BestallningsID",
                principalTable: "Bestallningar",
                principalColumn: "BestallningsID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaktioner_Restauranger_RestaurangID",
                table: "Transaktioner",
                column: "RestaurangID",
                principalTable: "Restauranger",
                principalColumn: "RestaurangID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
