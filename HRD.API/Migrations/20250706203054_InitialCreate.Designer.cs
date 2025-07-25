﻿// <auto-generated />
using System;
using HRD.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HRD.API.Migrations
{
    [DbContext(typeof(HRDDbContext))]
    [Migration("20250706203054_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HRD.API.Models.Entities.Cuti", b =>
                {
                    b.Property<int>("IdCuti")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_cuti");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCuti"));

                    b.Property<string>("Alasan")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("alasan");

                    b.Property<string>("CatatanHRD")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("catatan_hrd");

                    b.Property<int>("IdKaryawan")
                        .HasColumnType("int")
                        .HasColumnName("id_karyawan");

                    b.Property<string>("JenisCuti")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)")
                        .HasColumnName("jenis_cuti");

                    b.Property<int>("JumlahHari")
                        .HasColumnType("int")
                        .HasColumnName("jumlah_hari");

                    b.Property<string>("StatusPersetujuan")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("status_persetujuan");

                    b.Property<DateTime>("TglDibuat")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_dibuat");

                    b.Property<DateTime>("TglMulai")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_mulai");

                    b.Property<DateTime>("TglSelesai")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_selesai");

                    b.HasKey("IdCuti");

                    b.HasIndex("IdKaryawan");

                    b.HasIndex("StatusPersetujuan");

                    b.HasIndex("TglMulai");

                    b.ToTable("tb_cuti");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Gaji", b =>
                {
                    b.Property<int>("IdGaji")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_gaji");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdGaji"));

                    b.Property<int>("Bulan")
                        .HasColumnType("int")
                        .HasColumnName("bulan");

                    b.Property<decimal>("GajiPokok")
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("gaji_pokok");

                    b.Property<int>("IdKaryawan")
                        .HasColumnType("int")
                        .HasColumnName("id_karyawan");

                    b.Property<decimal>("Potongan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("potongan");

                    b.Property<string>("StatusBayar")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Belum Dibayar")
                        .HasColumnName("status_bayar");

                    b.Property<int>("Tahun")
                        .HasColumnType("int")
                        .HasColumnName("tahun");

                    b.Property<DateTime>("TglDibuat")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_dibuat");

                    b.Property<decimal>("TotalGaji")
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("total_gaji");

                    b.Property<decimal>("TotalJamKerja")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(6,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("total_jam_kerja");

                    b.Property<decimal>("Tunjangan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("tunjangan");

                    b.HasKey("IdGaji");

                    b.HasIndex("StatusBayar");

                    b.HasIndex("Bulan", "Tahun");

                    b.HasIndex("IdKaryawan", "Bulan", "Tahun")
                        .IsUnique();

                    b.ToTable("tb_gaji");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Karyawan", b =>
                {
                    b.Property<int>("IdKaryawan")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_karyawan");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdKaryawan"));

                    b.Property<string>("Divisi")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("divisi");

                    b.Property<string>("EmailKantor")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("email_kantor");

                    b.Property<decimal>("GajiPokok")
                        .HasColumnType("decimal(12,2)")
                        .HasColumnName("gaji_pokok");

                    b.Property<int>("IdPengguna")
                        .HasColumnType("int")
                        .HasColumnName("id_pengguna");

                    b.Property<string>("NIK")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("nik");

                    b.Property<string>("NamaLengkap")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("nama_lengkap");

                    b.Property<string>("NoTelepon")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("no_telepon");

                    b.Property<string>("Posisi")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("posisi");

                    b.Property<string>("StatusKaryawan")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("status_karyawan");

                    b.Property<DateTime>("TglBergabung")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_bergabung");

                    b.Property<DateTime>("TglDibuat")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_dibuat");

                    b.HasKey("IdKaryawan");

                    b.HasIndex("EmailKantor")
                        .IsUnique();

                    b.HasIndex("IdPengguna")
                        .IsUnique();

                    b.HasIndex("NIK")
                        .IsUnique();

                    b.ToTable("tb_karyawan");

                    b.HasData(
                        new
                        {
                            IdKaryawan = 1,
                            Divisi = "Development",
                            EmailKantor = "john@company.com",
                            GajiPokok = 8000000m,
                            IdPengguna = 2,
                            NIK = "EMP001",
                            NamaLengkap = "John Doe",
                            NoTelepon = "081234567890",
                            Posisi = "Software Developer",
                            StatusKaryawan = "Aktif",
                            TglBergabung = new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TglDibuat = new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(4751)
                        });
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Pengguna", b =>
                {
                    b.Property<int>("IdPengguna")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_pengguna");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPengguna"));

                    b.Property<bool>("Aktif")
                        .HasColumnType("bit")
                        .HasColumnName("aktif");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("Peran")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("peran");

                    b.Property<DateTime>("TglDibuat")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_dibuat");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("username");

                    b.HasKey("IdPengguna");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("tb_pengguna");

                    b.HasData(
                        new
                        {
                            IdPengguna = 1,
                            Aktif = true,
                            Email = "hrd@company.com",
                            PasswordHash = "$2a$11$rrrlegV/.r4Fs2jkuJ.QpOj3TcDGaDDqmhLyCe9qsu4vXbvkUg9YG",
                            Peran = "HRD",
                            TglDibuat = new DateTime(2025, 7, 7, 3, 30, 53, 915, DateTimeKind.Local).AddTicks(8496),
                            Username = "admin"
                        },
                        new
                        {
                            IdPengguna = 2,
                            Aktif = true,
                            Email = "john@company.com",
                            PasswordHash = "$2a$11$p0eZVi45QhH0oS9kik6SNeOrrLoNtVVFuPrcw6hmUNHvLi.BuRxJu",
                            Peran = "Karyawan",
                            TglDibuat = new DateTime(2025, 7, 7, 3, 30, 54, 24, DateTimeKind.Local).AddTicks(3978),
                            Username = "john.doe"
                        });
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Presensi", b =>
                {
                    b.Property<int>("IdPresensi")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_presensi");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPresensi"));

                    b.Property<string>("Catatan")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("catatan");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("device_id");

                    b.Property<int>("IdKaryawan")
                        .HasColumnType("int")
                        .HasColumnName("id_karyawan");

                    b.Property<DateTime?>("JamKeluar")
                        .HasColumnType("datetime2")
                        .HasColumnName("jam_keluar");

                    b.Property<DateTime?>("JamMasuk")
                        .HasColumnType("datetime2")
                        .HasColumnName("jam_masuk");

                    b.Property<string>("MetodePresensi")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Web")
                        .HasColumnName("metode_presensi");

                    b.Property<string>("StatusPresensi")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("Hadir")
                        .HasColumnName("status_presensi");

                    b.Property<DateTime>("Tanggal")
                        .HasColumnType("datetime2")
                        .HasColumnName("tanggal");

                    b.Property<DateTime>("TglDibuat")
                        .HasColumnType("datetime2")
                        .HasColumnName("tgl_dibuat");

                    b.Property<decimal>("TotalJam")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(4,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("total_jam");

                    b.HasKey("IdPresensi");

                    b.HasIndex("MetodePresensi");

                    b.HasIndex("Tanggal");

                    b.HasIndex("IdKaryawan", "Tanggal")
                        .IsUnique();

                    b.ToTable("tb_presensi");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Cuti", b =>
                {
                    b.HasOne("HRD.API.Models.Entities.Karyawan", "Karyawan")
                        .WithMany("DaftarCuti")
                        .HasForeignKey("IdKaryawan")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Karyawan");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Gaji", b =>
                {
                    b.HasOne("HRD.API.Models.Entities.Karyawan", "Karyawan")
                        .WithMany("DaftarGaji")
                        .HasForeignKey("IdKaryawan")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Karyawan");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Karyawan", b =>
                {
                    b.HasOne("HRD.API.Models.Entities.Pengguna", "Pengguna")
                        .WithOne("Karyawan")
                        .HasForeignKey("HRD.API.Models.Entities.Karyawan", "IdPengguna")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pengguna");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Presensi", b =>
                {
                    b.HasOne("HRD.API.Models.Entities.Karyawan", "Karyawan")
                        .WithMany("DaftarPresensi")
                        .HasForeignKey("IdKaryawan")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Karyawan");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Karyawan", b =>
                {
                    b.Navigation("DaftarCuti");

                    b.Navigation("DaftarGaji");

                    b.Navigation("DaftarPresensi");
                });

            modelBuilder.Entity("HRD.API.Models.Entities.Pengguna", b =>
                {
                    b.Navigation("Karyawan");
                });
#pragma warning restore 612, 618
        }
    }
}
