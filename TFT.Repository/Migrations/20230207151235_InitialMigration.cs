using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT.Repository.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DirectorID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StartProduction = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndProduction = table.Column<DateTime>(type: "datetime", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    DirectorID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MovieDirector",
                        column: x => x.DirectorID,
                        principalTable: "Directors",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ActorAgreements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsInvited = table.Column<bool>(type: "bit", nullable: false),
                    IsAccepted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Honorarium = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    MovieID = table.Column<long>(type: "bigint", nullable: false),
                    ActorID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorAgreements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ActorActorAgreement",
                        column: x => x.ActorID,
                        principalTable: "Actors",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ActorAgreementMovie",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GenreMovie",
                columns: table => new
                {
                    Genres_ID = table.Column<long>(type: "bigint", nullable: false),
                    Movies_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMovie", x => new { x.Genres_ID, x.Movies_ID });
                    table.ForeignKey(
                        name: "FK_GenreMovie_Genre",
                        column: x => x.Genres_ID,
                        principalTable: "Genres",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GenreMovie_Movie",
                        column: x => x.Movies_ID,
                        principalTable: "Movies",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_ActorActorAgreement",
                table: "ActorAgreements",
                column: "ActorID");

            migrationBuilder.CreateIndex(
                name: "IX_FK_ActorAgreementMovie",
                table: "ActorAgreements",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_FK_GenreMovie_Movie",
                table: "GenreMovie",
                column: "Movies_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FK_MovieDirector",
                table: "Movies",
                column: "DirectorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorAgreements");

            migrationBuilder.DropTable(
                name: "GenreMovie");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Directors");
        }
    }
}
