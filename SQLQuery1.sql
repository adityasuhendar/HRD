-- Aktifkan IDENTITY_INSERT untuk tabel tb_pengguna
SET IDENTITY_INSERT [HRD_System].[dbo].[tb_pengguna] ON;

-- Jalankan INSERT dengan id_pengguna manual
INSERT INTO [HRD_System].[dbo].[tb_pengguna]
           ([id_pengguna],
            [username],
            [email],
            [password_hash],
            [peran],
            [aktif],
            [tgl_dibuat])
     VALUES
           (1,
            'admin',
            'hrd@company.com',
            '$2a$11$HXzJmitOQMik5obAyQrQVuZxTBcevpyk6QZpoE74fCgWEtgvisyZ2',
            'HRD',
            1,
            '2025-07-08 12:49:58.6606523');

-- Matikan kembali IDENTITY_INSERT
SET IDENTITY_INSERT [HRD_System].[dbo].[tb_pengguna] OFF;
