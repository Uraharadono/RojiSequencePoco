using Microsoft.EntityFrameworkCore.Migrations;

namespace Core5ApiBoilerplate.DbContext.Migrations
{
    public partial class SequenceOnBlogPkMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Oid",
                table: "Blogs",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR BlogSeq",
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Oid",
                table: "Blogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR BlogSeq");
        }
    }
}
