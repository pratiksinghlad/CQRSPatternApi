using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQRSPattern.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpDepartmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Department",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Name = table
                            .Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("Department_PK", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Employee",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        FirstName = table
                            .Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        LastName = table
                            .Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Gender = table
                            .Column<string>(type: "varchar(1)", maxLength: 1, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        HireDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("Employee_PK", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Employee");
            migrationBuilder.DropTable(name: "Department");
        }
    }
}
