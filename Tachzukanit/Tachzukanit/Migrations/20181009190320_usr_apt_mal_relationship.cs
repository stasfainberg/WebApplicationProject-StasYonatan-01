using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tachzukanit.Migrations
{
    public partial class usr_apt_mal_relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Malfunction_Apartment_CurrentApartmentApartmentId",
                table: "Malfunction");

            migrationBuilder.DropForeignKey(
                name: "FK_Malfunction_User_RequestedByUserId",
                table: "Malfunction");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedByUserId",
                table: "Malfunction",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentApartmentApartmentId",
                table: "Malfunction",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Malfunction_Apartment_CurrentApartmentApartmentId",
                table: "Malfunction",
                column: "CurrentApartmentApartmentId",
                principalTable: "Apartment",
                principalColumn: "ApartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Malfunction_User_RequestedByUserId",
                table: "Malfunction",
                column: "RequestedByUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Malfunction_Apartment_CurrentApartmentApartmentId",
                table: "Malfunction");

            migrationBuilder.DropForeignKey(
                name: "FK_Malfunction_User_RequestedByUserId",
                table: "Malfunction");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedByUserId",
                table: "Malfunction",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CurrentApartmentApartmentId",
                table: "Malfunction",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Malfunction_Apartment_CurrentApartmentApartmentId",
                table: "Malfunction",
                column: "CurrentApartmentApartmentId",
                principalTable: "Apartment",
                principalColumn: "ApartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Malfunction_User_RequestedByUserId",
                table: "Malfunction",
                column: "RequestedByUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
