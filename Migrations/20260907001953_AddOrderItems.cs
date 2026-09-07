using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create OrderItems BEFORE deleting old Order columns
            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(
                            type: "int",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    OrderId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    ProductId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Quantity = table.Column<int>(
                        type: "int",
                        nullable: false),

                    UnitPrice = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_OrderItems",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // 2. Copy existing order product/quantity data
            // into the new OrderItems table.
            //
            // UnitPrice is reconstructed from:
            //
            // TotalPrice / Quantity
            //
            // instead of using the current Products.Price.
            migrationBuilder.Sql(
                """
                INSERT INTO OrderItems
                    (OrderId, ProductId, Quantity, UnitPrice)
                SELECT
                    Id,
                    ProductId,
                    Quantity,
                    CASE
                        WHEN Quantity > 0
                        THEN CAST(
                            TotalPrice / Quantity
                            AS decimal(18,2)
                        )
                        ELSE 0
                    END
                FROM Orders;
                """);

            // 3. Create indexes for the new foreign keys
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            // 4. Remove old Order -> Product foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            // 5. Now that the data was copied,
            // remove the obsolete columns from Orders
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The old database design supports exactly
            // ONE product per Order.
            //
            // The new design supports many OrderItems.
            //
            // Therefore rollback is only safe when
            // every Order currently has exactly one OrderItem.
            migrationBuilder.Sql(
                """
                IF EXISTS
                (
                    SELECT o.Id
                    FROM Orders o
                    LEFT JOIN OrderItems oi
                        ON oi.OrderId = o.Id
                    GROUP BY o.Id
                    HAVING COUNT(oi.Id) <> 1
                )
                BEGIN
                    THROW 50000,
                        'Cannot rollback AddOrderItems because some orders do not contain exactly one OrderItem.',
                        1;
                END
                """);

            // 1. Temporarily restore the old columns as nullable.
            // They must be nullable initially because existing
            // Orders do not yet have values in these columns.
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: true);

            // 2. Copy data from OrderItems back into Orders
            migrationBuilder.Sql(
                """
                UPDATE o
                SET
                    o.ProductId = oi.ProductId,
                    o.Quantity = oi.Quantity
                FROM Orders o
                INNER JOIN OrderItems oi
                    ON oi.OrderId = o.Id;
                """);

            // 3. Remove OrderItems
            migrationBuilder.DropTable(
                name: "OrderItems");

            // 4. Return ProductId and Quantity
            // to their old NOT NULL definitions
            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 5. Restore the old ProductId index
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            // 6. Restore the old Order -> Product FK
            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}