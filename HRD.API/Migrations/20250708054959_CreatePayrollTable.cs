using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRD.API.Migrations
{
    /// <inheritdoc />
    public partial class CreatePayrollTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_gaji_tb_karyawan_id_karyawan",
                table: "tb_gaji");

            migrationBuilder.RenameIndex(
                name: "IX_tb_gaji_id_karyawan_bulan_tahun",
                table: "tb_gaji",
                newName: "IX_Gaji_Karyawan_Bulan_Tahun");

            migrationBuilder.AlterColumn<decimal>(
                name: "tunjangan",
                table: "tb_gaji",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "total_jam_kerja",
                table: "tb_gaji",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_gaji",
                table: "tb_gaji",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "potongan",
                table: "tb_gaji",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "gaji_pokok",
                table: "tb_gaji",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AddColumn<int>(
                name: "KaryawanIdKaryawan",
                table: "tb_gaji",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "keterangan",
                table: "tb_gaji",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "tb_karyawan",
                keyColumn: "id_karyawan",
                keyValue: 1,
                column: "tgl_dibuat",
                value: new DateTime(2025, 7, 8, 12, 49, 58, 768, DateTimeKind.Local).AddTicks(664));

            migrationBuilder.UpdateData(
                table: "tb_pengguna",
                keyColumn: "id_pengguna",
                keyValue: 1,
                columns: new[] { "password_hash", "tgl_dibuat" },
                values: new object[] { "$2a$11$HXzJmitOQMik5obAyQrQVuZxTBcevpyk6QZpoE74fCgWEtgvisyZ2", new DateTime(2025, 7, 8, 12, 49, 58, 660, DateTimeKind.Local).AddTicks(6523) });

            migrationBuilder.UpdateData(
                table: "tb_pengguna",
                keyColumn: "id_pengguna",
                keyValue: 2,
                columns: new[] { "password_hash", "tgl_dibuat" },
                values: new object[] { "$2a$11$kPSqUHrwma0VSTiYKiv8ROZzYhJywCcNtTcKTwxQnrFL7s5hp5pyq", new DateTime(2025, 7, 8, 12, 49, 58, 767, DateTimeKind.Local).AddTicks(9921) });

            migrationBuilder.CreateIndex(
                name: "IX_tb_gaji_KaryawanIdKaryawan",
                table: "tb_gaji",
                column: "KaryawanIdKaryawan");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_gaji_tb_karyawan_KaryawanIdKaryawan",
                table: "tb_gaji",
                column: "KaryawanIdKaryawan",
                principalTable: "tb_karyawan",
                principalColumn: "id_karyawan");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_gaji_tb_karyawan_id_karyawan",
                table: "tb_gaji",
                column: "id_karyawan",
                principalTable: "tb_karyawan",
                principalColumn: "id_karyawan",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_gaji_tb_karyawan_KaryawanIdKaryawan",
                table: "tb_gaji");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_gaji_tb_karyawan_id_karyawan",
                table: "tb_gaji");

            migrationBuilder.DropIndex(
                name: "IX_tb_gaji_KaryawanIdKaryawan",
                table: "tb_gaji");

            migrationBuilder.DropColumn(
                name: "KaryawanIdKaryawan",
                table: "tb_gaji");

            migrationBuilder.DropColumn(
                name: "keterangan",
                table: "tb_gaji");

            migrationBuilder.RenameIndex(
                name: "IX_Gaji_Karyawan_Bulan_Tahun",
                table: "tb_gaji",
                newName: "IX_tb_gaji_id_karyawan_bulan_tahun");

            migrationBuilder.AlterColumn<decimal>(
                name: "tunjangan",
                table: "tb_gaji",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_jam_kerja",
                table: "tb_gaji",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_gaji",
                table: "tb_gaji",
                type: "decimal(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "potongan",
                table: "tb_gaji",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "gaji_pokok",
                table: "tb_gaji",
                type: "decimal(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "tb_karyawan",
                keyColumn: "id_karyawan",
                keyValue: 1,
                column: "tgl_dibuat",
                value: new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(4751));

            migrationBuilder.UpdateData(
                table: "tb_pengguna",
                keyColumn: "id_pengguna",
                keyValue: 1,
                columns: new[] { "password_hash", "tgl_dibuat" },
                values: new object[] { "$2a$11$rrrlegV/.r4Fs2jkuJ.QpOj3TcDGaDDqmhLyCe9qsu4vXbvkUg9YG", new DateTime(2025, 7, 7, 3, 30, 53, 915, DateTimeKind.Local).AddTicks(8496) });

            migrationBuilder.UpdateData(
                table: "tb_pengguna",
                keyColumn: "id_pengguna",
                keyValue: 2,
                columns: new[] { "password_hash", "tgl_dibuat" },
                values: new object[] { "$2a$11$p0eZVi45QhH0oS9kik6SNeOrrLoNtVVFuPrcw6hmUNHvLi.BuRxJu", new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(3978) });

            migrationBuilder.AddForeignKey(
                name: "FK_tb_gaji_tb_karyawan_id_karyawan",
                table: "tb_gaji",
                column: "id_karyawan",
                principalTable: "tb_karyawan",
                principalColumn: "id_karyawan",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
