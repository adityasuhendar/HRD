using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRD.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_pengguna",
                columns: table => new
                {
                    id_pengguna = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    peran = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    aktif = table.Column<bool>(type: "bit", nullable: false),
                    tgl_dibuat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_pengguna", x => x.id_pengguna);
                });

            migrationBuilder.CreateTable(
                name: "tb_karyawan",
                columns: table => new
                {
                    id_karyawan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pengguna = table.Column<int>(type: "int", nullable: false),
                    nik = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nama_lengkap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email_kantor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    no_telepon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    posisi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    divisi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tgl_bergabung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gaji_pokok = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    status_karyawan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    tgl_dibuat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_karyawan", x => x.id_karyawan);
                    table.ForeignKey(
                        name: "FK_tb_karyawan_tb_pengguna_id_pengguna",
                        column: x => x.id_pengguna,
                        principalTable: "tb_pengguna",
                        principalColumn: "id_pengguna",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_cuti",
                columns: table => new
                {
                    id_cuti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_karyawan = table.Column<int>(type: "int", nullable: false),
                    jenis_cuti = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    tgl_mulai = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tgl_selesai = table.Column<DateTime>(type: "datetime2", nullable: false),
                    jumlah_hari = table.Column<int>(type: "int", nullable: false),
                    alasan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status_persetujuan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    catatan_hrd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tgl_dibuat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_cuti", x => x.id_cuti);
                    table.ForeignKey(
                        name: "FK_tb_cuti_tb_karyawan_id_karyawan",
                        column: x => x.id_karyawan,
                        principalTable: "tb_karyawan",
                        principalColumn: "id_karyawan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_gaji",
                columns: table => new
                {
                    id_gaji = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_karyawan = table.Column<int>(type: "int", nullable: false),
                    bulan = table.Column<int>(type: "int", nullable: false),
                    tahun = table.Column<int>(type: "int", nullable: false),
                    gaji_pokok = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    tunjangan = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    potongan = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    total_jam_kerja = table.Column<decimal>(type: "decimal(6,2)", nullable: false, defaultValue: 0m),
                    total_gaji = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    status_bayar = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Belum Dibayar"),
                    tgl_dibuat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_gaji", x => x.id_gaji);
                    table.ForeignKey(
                        name: "FK_tb_gaji_tb_karyawan_id_karyawan",
                        column: x => x.id_karyawan,
                        principalTable: "tb_karyawan",
                        principalColumn: "id_karyawan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_presensi",
                columns: table => new
                {
                    id_presensi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_karyawan = table.Column<int>(type: "int", nullable: false),
                    tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    jam_masuk = table.Column<DateTime>(type: "datetime2", nullable: true),
                    jam_keluar = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_jam = table.Column<decimal>(type: "decimal(4,2)", nullable: false, defaultValue: 0m),
                    status_presensi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Hadir"),
                    metode_presensi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Web"),
                    device_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    catatan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    tgl_dibuat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_presensi", x => x.id_presensi);
                    table.ForeignKey(
                        name: "FK_tb_presensi_tb_karyawan_id_karyawan",
                        column: x => x.id_karyawan,
                        principalTable: "tb_karyawan",
                        principalColumn: "id_karyawan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tb_pengguna",
                columns: new[] { "id_pengguna", "aktif", "email", "password_hash", "peran", "tgl_dibuat", "username" },
                values: new object[,]
                {
                    { 1, true, "hrd@company.com", "$2a$11$rrrlegV/.r4Fs2jkuJ.QpOj3TcDGaDDqmhLyCe9qsu4vXbvkUg9YG", "HRD", new DateTime(2025, 7, 7, 3, 30, 53, 915, DateTimeKind.Local).AddTicks(8496), "admin" },
                    { 2, true, "john@company.com", "$2a$11$p0eZVi45QhH0oS9kik6SNeOrrLoNtVVFuPrcw6hmUNHvLi.BuRxJu", "Karyawan", new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(3978), "john.doe" }
                });

            migrationBuilder.InsertData(
                table: "tb_karyawan",
                columns: new[] { "id_karyawan", "divisi", "email_kantor", "gaji_pokok", "id_pengguna", "nik", "nama_lengkap", "no_telepon", "posisi", "status_karyawan", "tgl_bergabung", "tgl_dibuat" },
                values: new object[] { 1, "Development", "john@company.com", 8000000m, 2, "EMP001", "John Doe", "081234567890", "Software Developer", "Aktif", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(4751) });

            migrationBuilder.CreateIndex(
                name: "IX_tb_cuti_id_karyawan",
                table: "tb_cuti",
                column: "id_karyawan");

            migrationBuilder.CreateIndex(
                name: "IX_tb_cuti_status_persetujuan",
                table: "tb_cuti",
                column: "status_persetujuan");

            migrationBuilder.CreateIndex(
                name: "IX_tb_cuti_tgl_mulai",
                table: "tb_cuti",
                column: "tgl_mulai");

            migrationBuilder.CreateIndex(
                name: "IX_tb_gaji_bulan_tahun",
                table: "tb_gaji",
                columns: new[] { "bulan", "tahun" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_gaji_id_karyawan_bulan_tahun",
                table: "tb_gaji",
                columns: new[] { "id_karyawan", "bulan", "tahun" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_gaji_status_bayar",
                table: "tb_gaji",
                column: "status_bayar");

            migrationBuilder.CreateIndex(
                name: "IX_tb_karyawan_email_kantor",
                table: "tb_karyawan",
                column: "email_kantor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_karyawan_id_pengguna",
                table: "tb_karyawan",
                column: "id_pengguna",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_karyawan_nik",
                table: "tb_karyawan",
                column: "nik",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_pengguna_email",
                table: "tb_pengguna",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_pengguna_username",
                table: "tb_pengguna",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_presensi_id_karyawan_tanggal",
                table: "tb_presensi",
                columns: new[] { "id_karyawan", "tanggal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_presensi_metode_presensi",
                table: "tb_presensi",
                column: "metode_presensi");

            migrationBuilder.CreateIndex(
                name: "IX_tb_presensi_tanggal",
                table: "tb_presensi",
                column: "tanggal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_cuti");

            migrationBuilder.DropTable(
                name: "tb_gaji");

            migrationBuilder.DropTable(
                name: "tb_presensi");

            migrationBuilder.DropTable(
                name: "tb_karyawan");

            migrationBuilder.DropTable(
                name: "tb_pengguna");
        }
    }
}
