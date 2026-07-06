USE [Dev.BamboeHR]
GO

-- =========================================================================
-- IMPORT SCRIPT
-- =========================================================================
-- Script ini digunakan untuk melakukan migrasi data dari tabel lama ke 
-- tabel baru [core].[Country], [core].[Province], dan [core].[City].
-- Kita menggunakan SET IDENTITY_INSERT ON untuk mempertahankan ID lama,
-- sehingga relasi Foreign Key (prov_id, locationid/CountryId) tidak rusak.
-- =========================================================================

-- 1. IMPORT DATA NEGARA (COUNTRY)
-- Asumsi tabel lama bernama [dbo].[Country] (sesuaikan jika namanya berbeda)
PRINT 'Importing Country...'
SET IDENTITY_INSERT [core].[Country] ON;

INSERT INTO [core].[Country] (
    [CountryId], [CountryGuid], [CountryIso], [CountryName], [CountryIso3], 
    [PhoneCode], [CurrencyCode], [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [Id], 
    NEWID(), 
    [iso], 
    [name], 
    [iso3], 
    [phonecode], 
    [CurrencyCode], -- Sesuaikan dengan nama kolom yang lama, misal Currency / Currancy
    1, -- Default Status Active
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[Country]; -- Sesuaikan nama tabel source

SET IDENTITY_INSERT [core].[Country] OFF;
GO

-- 2. IMPORT DATA PROVINSI (PROVINCE)
PRINT 'Importing Province...'
SET IDENTITY_INSERT [core].[Province] ON;

INSERT INTO [core].[Province] (
    [ProvinceId], [ProvinceGuid], [ProvinceName], [CountryId], 
    [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [prov_id], 
    NEWID(), 
    [prov_name], 
    [locationid],
    ISNULL([status], 1), -- Menggunakan status lama, jika NULL = 1
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[provinces];

SET IDENTITY_INSERT [core].[Province] OFF;
GO

-- 3. IMPORT DATA KOTA (CITY)
PRINT 'Importing City...'
SET IDENTITY_INSERT [core].[City] ON;

INSERT INTO [core].[City] (
    [CityId], [CityGuid], [CityName], [ProvinceId], 
    [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [city_id], 
    NEWID(), 
    [city_name], 
    [prov_id],
    1, -- Default Status Active
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[cities];

SET IDENTITY_INSERT [core].[City] OFF;
GO

-- 4. IMPORT DATA KECAMATAN (DISTRICT)
PRINT 'Importing District...'
SET IDENTITY_INSERT [core].[District] ON;

INSERT INTO [core].[District] (
    [DistrictId], [DistrictGuid], [DistrictName], [CityId], 
    [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [dis_id], 
    NEWID(), 
    [dis_name], 
    [city_id],
    1, -- Default Status Active
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[districts];

SET IDENTITY_INSERT [core].[District] OFF;
GO

-- 5. IMPORT DATA KELURAHAN (SUBDISTRICT)
PRINT 'Importing SubDistrict...'
SET IDENTITY_INSERT [core].[SubDistrict] ON;

INSERT INTO [core].[SubDistrict] (
    [SubDistrictId], [SubDistrictGuid], [SubDistrictName], [DistrictId], 
    [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [subdis_id], 
    NEWID(), 
    [subdis_name], 
    [dis_id],
    1, -- Default Status Active
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[subdistricts];

SET IDENTITY_INSERT [core].[SubDistrict] OFF;
GO

-- 6. IMPORT DATA KODE POS (POSTALCODE)
PRINT 'Importing PostalCode...'
SET IDENTITY_INSERT [core].[PostalCode] ON;

INSERT INTO [core].[PostalCode] (
    [PostalCodeId], [PostalCodeGuid], [SubDistrictId], [PostalCode], 
    [StatusId], [CreatedById], [CreatedTime]
)
SELECT 
    [postal_id], 
    NEWID(), 
    [subdis_id], 
    [postal_code],
    1, -- Default Status Active
    1, -- Default System Admin ID
    SYSDATETIME()
FROM [dbo].[postalcode];

SET IDENTITY_INSERT [core].[PostalCode] OFF;
GO

PRINT 'Import Completed Successfully!'
