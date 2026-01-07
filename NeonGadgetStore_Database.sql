-- =============================================
-- NEON GADGET STORE - DATABASE SCRIPT
-- =============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NeonGadgetStore')
BEGIN
    CREATE DATABASE NeonGadgetStore2;
END
GO

USE NeonGadgetStore2;
GO

-- =============================================
-- 1. CREATE AspNetUsers TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
BEGIN
    CREATE TABLE AspNetUsers (
        Id NVARCHAR(450) PRIMARY KEY,
        UserName NVARCHAR(256),
        NormalizedUserName NVARCHAR(256),
        Email NVARCHAR(256),
        NormalizedEmail NVARCHAR(256),
        EmailConfirmed BIT,
        PasswordHash NVARCHAR(MAX),
        SecurityStamp NVARCHAR(MAX),
        ConcurrencyStamp NVARCHAR(MAX),
        PhoneNumber NVARCHAR(MAX),
        PhoneNumberConfirmed BIT,
        TwoFactorEnabled BIT,
        LockoutEnd DATETIMEOFFSET,
        LockoutEnabled BIT,
        AccessFailedCount INT
    );
    
    CREATE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);
    CREATE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);
END
GO

-- =============================================
-- 2. CREATE AspNetRoles TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles')
BEGIN
    CREATE TABLE AspNetRoles (
        Id NVARCHAR(450) PRIMARY KEY,
        Name NVARCHAR(256),
        NormalizedName NVARCHAR(256),
        ConcurrencyStamp NVARCHAR(MAX)
    );
    
    CREATE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName);
END
GO

-- =============================================
-- 3. CREATE AspNetUserRoles TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles')
BEGIN
    CREATE TABLE AspNetUserRoles (
        UserId NVARCHAR(450) NOT NULL,
        RoleId NVARCHAR(450) NOT NULL,
        PRIMARY KEY (UserId, RoleId),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
        FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);
END
GO

-- =============================================
-- 4. CREATE AspNetUserClaims TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserClaims')
BEGIN
    CREATE TABLE AspNetUserClaims (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId NVARCHAR(450) NOT NULL,
        ClaimType NVARCHAR(MAX),
        ClaimValue NVARCHAR(MAX),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId);
END
GO

-- =============================================
-- 5. CREATE AspNetUserLogins TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserLogins')
BEGIN
    CREATE TABLE AspNetUserLogins (
        LoginProvider NVARCHAR(128) NOT NULL,
        ProviderKey NVARCHAR(128) NOT NULL,
        ProviderDisplayName NVARCHAR(MAX),
        UserId NVARCHAR(450) NOT NULL,
        PRIMARY KEY (LoginProvider, ProviderKey),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId);
END
GO

-- =============================================
-- 6. CREATE AspNetRoleClaims TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoleClaims')
BEGIN
    CREATE TABLE AspNetRoleClaims (
        Id INT PRIMARY KEY IDENTITY(1,1),
        RoleId NVARCHAR(450) NOT NULL,
        ClaimType NVARCHAR(MAX),
        ClaimValue NVARCHAR(MAX),
        FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims(RoleId);
END
GO

-- =============================================
-- 7. CREATE AspNetUserTokens TABLE (Identity)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserTokens')
BEGIN
    CREATE TABLE AspNetUserTokens (
        UserId NVARCHAR(450) NOT NULL,
        LoginProvider NVARCHAR(128) NOT NULL,
        Name NVARCHAR(128) NOT NULL,
        Value NVARCHAR(MAX),
        PRIMARY KEY (UserId, LoginProvider, Name),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
END
GO

-- =============================================
-- 8. CREATE Categories TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id NVARCHAR(450) PRIMARY KEY,
        Name NVARCHAR(256) NOT NULL,
        Slug NVARCHAR(256) NOT NULL UNIQUE,
        Description NVARCHAR(MAX),
        Icon NVARCHAR(256),
        ImageUrl NVARCHAR(MAX),
        IsActive BIT DEFAULT 1,
        SortOrder INT DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Categories_Slug ON Categories(Slug);
    CREATE INDEX IX_Categories_IsActive ON Categories(IsActive);
END
GO

-- =============================================
-- 9. CREATE Products TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    CREATE TABLE Products (
        Id NVARCHAR(450) PRIMARY KEY,
        Name NVARCHAR(256) NOT NULL,
        Slug NVARCHAR(256) NOT NULL UNIQUE,
        Price DECIMAL(18,2) NOT NULL,
        OriginalPrice DECIMAL(18,2),
        StockQuantity INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        IsFeatured BIT DEFAULT 0,
        Images NVARCHAR(MAX),
        CategoryId NVARCHAR(450),
        Description NVARCHAR(MAX),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
    );
    
    CREATE INDEX IX_Products_Slug ON Products(Slug);
    CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
    CREATE INDEX IX_Products_IsActive ON Products(IsActive);
    CREATE INDEX IX_Products_IsFeatured ON Products(IsFeatured);
END
GO

-- =============================================
-- 10. CREATE Profiles TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Profiles')
BEGIN
    CREATE TABLE Profiles (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId NVARCHAR(450) NOT NULL UNIQUE,
        Email NVARCHAR(256) NOT NULL,
        Username NVARCHAR(256) NOT NULL UNIQUE,
        FullName NVARCHAR(256),
        AvatarUrl NVARCHAR(MAX),
        Phone NVARCHAR(20),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Profiles_Username ON Profiles(Username);
    CREATE INDEX IX_Profiles_UserId ON Profiles(UserId);
END
GO

-- =============================================
-- 11. CREATE Orders TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id NVARCHAR(450) PRIMARY KEY,
        UserId NVARCHAR(450),
        OrderNumber NVARCHAR(256) NOT NULL,
        Subtotal DECIMAL(18,2),
        ShippingCost DECIMAL(18,2),
        TaxAmount DECIMAL(18,2),
        TotalAmount DECIMAL(18,2),
        PaymentMethod NVARCHAR(50) DEFAULT 'cod',
        PaymentStatus NVARCHAR(50) DEFAULT 'pending',
        Status NVARCHAR(50) DEFAULT 'pending',
        ShippingAddressJson NVARCHAR(MAX) DEFAULT '{}',
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
        ShippedAt DATETIME2,
        DeliveredAt DATETIME2,
        TrackingNumber NVARCHAR(256),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_Orders_UserId ON Orders(UserId);
    CREATE INDEX IX_Orders_Status ON Orders(Status);
    CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
END
GO

-- =============================================
-- 12. CREATE OrderItems TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id NVARCHAR(450) PRIMARY KEY,
        OrderId NVARCHAR(450) NOT NULL,
        ProductId NVARCHAR(450),
        ProductName NVARCHAR(256) NOT NULL,
        ProductImage NVARCHAR(MAX),
        Quantity INT,
        UnitPrice DECIMAL(18,2),
        TotalPrice DECIMAL(18,2),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
    CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
END
GO

-- =============================================
-- 13. CREATE WishlistEntries TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WishlistEntries')
BEGIN
    CREATE TABLE WishlistEntries (
        Id NVARCHAR(450) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        ProductId NVARCHAR(450) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_WishlistEntries_User_Product UNIQUE (UserId, ProductId)
    );
    
    CREATE INDEX IX_WishlistEntries_UserId ON WishlistEntries(UserId);
    CREATE INDEX IX_WishlistEntries_ProductId ON WishlistEntries(ProductId);
END
GO

-- =============================================
-- 14. CREATE ViewedEntries TABLE (Recently Viewed)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ViewedEntries')
BEGIN
    CREATE TABLE ViewedEntries (
        Id NVARCHAR(450) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        ProductId NVARCHAR(450) NOT NULL,
        ViewedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_ViewedEntries_User_Product UNIQUE (UserId, ProductId)
    );
    
    CREATE INDEX IX_ViewedEntries_UserId ON ViewedEntries(UserId);
    CREATE INDEX IX_ViewedEntries_ProductId ON ViewedEntries(ProductId);
END
GO

-- =============================================
-- INSERT SAMPLE DATA (Optional)
-- =============================================

-- Insert Sample Categories
IF NOT EXISTS (SELECT * FROM Categories WHERE Slug = 'smartphones')
BEGIN
    INSERT INTO Categories (Id, Name, Slug, Description, Icon, ImageUrl, IsActive, SortOrder)
    VALUES 
        (NEWID(), N'Smartphones', 'smartphones', N'Latest mobile phones and devices', 'smartphone', NULL, 1, 1),
        (NEWID(), N'Laptops', 'laptops', N'High-performance computers', 'laptop', NULL, 1, 2),
        (NEWID(), N'Accessories', 'accessories', N'Phone and gadget accessories', 'headphones', NULL, 1, 3),
        (NEWID(), N'Gaming', 'gaming', N'Gaming devices and consoles', 'gamepad', NULL, 1, 4);
END
GO

-- =============================================
-- PRINT SUCCESS MESSAGE
-- =============================================
PRINT '=============================================';
PRINT 'Neon Gadget Store Database Created Successfully!';
PRINT '=============================================';
PRINT 'Database: NeonGadgetStore';
PRINT 'Tables Created:';
PRINT '  - AspNetUsers';
PRINT '  - AspNetRoles';
PRINT '  - AspNetUserRoles';
PRINT '  - AspNetUserClaims';
PRINT '  - AspNetUserLogins';
PRINT '  - AspNetRoleClaims';
PRINT '  - AspNetUserTokens';
PRINT '  - Categories';
PRINT '  - Products';
PRINT '  - Profiles';
PRINT '  - Orders';
PRINT '  - OrderItems';
PRINT '  - WishlistEntries';
PRINT '  - ViewedEntries';
PRINT '=============================================';
