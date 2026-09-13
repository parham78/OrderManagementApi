using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemProductNameSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the column temporarily as nullable.
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            // 2. Copy the current product name into existing order items.
            migrationBuilder.Sql("""
        UPDATE oi
        SET oi.ProductName = p.Name
        FROM OrderItems oi
        INNER JOIN Products p
            ON oi.ProductId = p.Id;
        """);

            // 3. Now that every existing row has a value,
            // make the column required.
            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItems");
        }
    }
}
