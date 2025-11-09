using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePurchasess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseForms_PurchaseFormId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseKinds_PurchaseKindId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseTypes_PurchaseTypeId",
                table: "Purchases");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseTypeId",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseKindId",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseFormId",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FirmId",
                table: "Purchases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseForms_PurchaseFormId",
                table: "Purchases",
                column: "PurchaseFormId",
                principalTable: "PurchaseForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseKinds_PurchaseKindId",
                table: "Purchases",
                column: "PurchaseKindId",
                principalTable: "PurchaseKinds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseTypes_PurchaseTypeId",
                table: "Purchases",
                column: "PurchaseTypeId",
                principalTable: "PurchaseTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseForms_PurchaseFormId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseKinds_PurchaseKindId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_PurchaseTypes_PurchaseTypeId",
                table: "Purchases");

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseTypeId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseKindId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PurchaseFormId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FirmId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseForms_PurchaseFormId",
                table: "Purchases",
                column: "PurchaseFormId",
                principalTable: "PurchaseForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseKinds_PurchaseKindId",
                table: "Purchases",
                column: "PurchaseKindId",
                principalTable: "PurchaseKinds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_PurchaseTypes_PurchaseTypeId",
                table: "Purchases",
                column: "PurchaseTypeId",
                principalTable: "PurchaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
