-- =============================================
-- NEON GADGET STORE - SAMPLE DATA SCRIPT
-- =============================================

USE NeonGadgetStore2;
GO

-- =============================================
-- DELETE EXISTING DATA (Optional - uncomment if needed)
-- =============================================
DELETE FROM Products;
DELETE FROM Categories;
GO

-- =============================================
-- INSERT CATEGORIES
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Slug = 'laptops')
BEGIN
    INSERT INTO Categories (Id, Name, Slug, Description, Icon, IsActive, SortOrder)
    VALUES 
        ('cat-laptops', N'Laptops', 'laptops', N'Máy tính xách tay cao cấp', 'Laptop', 1, 1),
        ('cat-smartphones', N'Smartphones', 'smartphones', N'Điện thoại thông minh', 'Mobile', 1, 2),
        ('cat-tablets', N'Tablets', 'tablets', N'Máy tính bảng', 'Tablet', 1, 3),
        ('cat-headphones', N'Headphones', 'headphones', N'Tai nghe cao cấp', 'Headphones', 1, 4),
        ('cat-smartwatches', N'Smartwatches', 'smartwatches', N'Đồng hồ thông minh', 'Watch', 1, 5),
        ('cat-cameras', N'Cameras', 'cameras', N'Máy ảnh và phụ kiện', 'Camera', 1, 6),
        ('cat-accessories', N'Accessories', 'accessories', N'Phụ kiện điện tử', 'Accessories', 1, 7);
END
GO

-- =============================================
-- INSERT PRODUCTS (20 PRODUCTS)
-- =============================================

-- LAPTOPS (4 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'MacBook Pro 16" M3 Max', 'macbook-pro-16-m3-max', 2499.00, 2799.00, 25, 1, 1, '["https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=500"]', 'cat-laptops', N'Laptop cao cấp với chip M3 Max, màn hình Liquid Retina XDR 16 inch, RAM 36GB, SSD 512GB', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Dell XPS 15', 'dell-xps-15', 1799.00, 1999.00, 30, 1, 1, '["https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=500"]', 'cat-laptops', N'Laptop Windows cao cấp với Intel Core i9, màn hình OLED 15.6 inch 4K, RAM 32GB, SSD 1TB', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'ASUS ROG Zephyrus G16', 'asus-rog-zephyrus-g16', 2199.00, 2399.00, 20, 1, 0, '["https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500"]', 'cat-laptops', N'Laptop gaming với RTX 4090, Intel Core i9-14900H, màn hình 16 inch 240Hz', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Lenovo ThinkPad X1 Carbon', 'lenovo-thinkpad-x1-carbon', 1599.00, 1799.00, 35, 1, 0, '["https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=500"]', 'cat-laptops', N'Laptop doanh nhân siêu nhẹ, Intel Core i7, màn hình 14 inch 2.8K OLED', GETUTCDATE(), GETUTCDATE());

-- SMARTPHONES (4 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'iPhone 15 Pro Max', 'iphone-15-pro-max', 1199.00, 1299.00, 50, 1, 1, '["https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=500"]', 'cat-smartphones', N'iPhone cao cấp nhất với chip A17 Pro, camera 48MP, titanium design, USB-C', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Samsung Galaxy S24 Ultra', 'samsung-galaxy-s24-ultra', 1299.00, 1399.00, 45, 1, 1, '["https://images.unsplash.com/photo-1610945415295-d9bbf067e59c?w=500"]', 'cat-smartphones', N'Flagship Android với S Pen, camera 200MP, màn hình Dynamic AMOLED 6.8 inch', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Google Pixel 8 Pro', 'google-pixel-8-pro', 999.00, 1099.00, 40, 1, 0, '["https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=500"]', 'cat-smartphones', N'Điện thoại AI với chip Tensor G3, camera xuất sắc, 7 năm cập nhật', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'OnePlus 12', 'oneplus-12', 799.00, 899.00, 55, 1, 0, '["https://images.unsplash.com/photo-1591337676887-a217a6970a8a?w=500"]', 'cat-smartphones', N'Flagship killer với Snapdragon 8 Gen 3, sạc nhanh 100W, camera Hasselblad', GETUTCDATE(), GETUTCDATE());

-- TABLETS (2 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'iPad Pro 12.9" M2', 'ipad-pro-12-9-m2', 1099.00, 1199.00, 30, 1, 1, '["https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=500"]', 'cat-tablets', N'Tablet mạnh nhất với chip M2, màn hình Liquid Retina XDR, hỗ trợ Apple Pencil 2', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Samsung Galaxy Tab S9 Ultra', 'samsung-galaxy-tab-s9-ultra', 1199.00, 1299.00, 25, 1, 0, '["https://images.unsplash.com/photo-1561154464-82e9adf32764?w=500"]', 'cat-tablets', N'Tablet Android cao cấp với màn hình AMOLED 14.6 inch, S Pen đi kèm', GETUTCDATE(), GETUTCDATE());

-- HEADPHONES (3 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'Sony WH-1000XM5', 'sony-wh-1000xm5', 349.00, 399.00, 60, 1, 1, '["https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?w=500"]', 'cat-headphones', N'Tai nghe chống ồn tốt nhất thế giới, pin 30 giờ, âm thanh Hi-Res', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Apple AirPods Pro 2', 'apple-airpods-pro-2', 249.00, 279.00, 80, 1, 1, '["https://images.unsplash.com/photo-1606220588913-b3aacb4d2f46?w=500"]', 'cat-headphones', N'Tai nghe true wireless với chống ồn chủ động, Spatial Audio, USB-C', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Bose QuietComfort Ultra', 'bose-quietcomfort-ultra', 429.00, 479.00, 40, 1, 0, '["https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=500"]', 'cat-headphones', N'Tai nghe over-ear với Immersive Audio, chống ồn hàng đầu', GETUTCDATE(), GETUTCDATE());

-- SMARTWATCHES (2 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'Apple Watch Ultra 2', 'apple-watch-ultra-2', 799.00, 849.00, 35, 1, 1, '["https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=500"]', 'cat-smartwatches', N'Smartwatch cao cấp nhất của Apple, titanium, GPS dual-frequency, pin 36 giờ', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Samsung Galaxy Watch 6 Classic', 'samsung-galaxy-watch-6-classic', 399.00, 449.00, 45, 1, 0, '["https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=500"]', 'cat-smartwatches', N'Smartwatch Android với bezel xoay, theo dõi sức khỏe toàn diện', GETUTCDATE(), GETUTCDATE());

-- CAMERAS (2 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'Sony A7 IV', 'sony-a7-iv', 2499.00, 2699.00, 15, 1, 1, '["https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=500"]', 'cat-cameras', N'Máy ảnh full-frame 33MP, quay video 4K 60fps, autofocus AI', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Canon EOS R6 Mark II', 'canon-eos-r6-mark-ii', 2299.00, 2499.00, 18, 1, 0, '["https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=500"]', 'cat-cameras', N'Máy ảnh mirrorless 24.2MP, quay 4K 60fps, chống rung 8 stop', GETUTCDATE(), GETUTCDATE());

-- ACCESSORIES (3 products)
INSERT INTO Products (Id, Name, Slug, Price, OriginalPrice, StockQuantity, IsActive, IsFeatured, Images, CategoryId, Description, CreatedAt, UpdatedAt)
VALUES 
    (NEWID(), N'Anker 737 Power Bank', 'anker-737-power-bank', 149.00, 179.00, 100, 1, 0, '["https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?w=500"]', 'cat-accessories', N'Sạc dự phòng 24000mAh, công suất 140W, sạc laptop được', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Logitech MX Master 3S', 'logitech-mx-master-3s', 99.00, 119.00, 70, 1, 0, '["https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=500"]', 'cat-accessories', N'Chuột không dây cao cấp, MagSpeed scroll, kết nối 3 thiết bị', GETUTCDATE(), GETUTCDATE()),
    (NEWID(), N'Samsung T7 Shield 2TB', 'samsung-t7-shield-2tb', 189.00, 219.00, 55, 1, 0, '["https://images.unsplash.com/photo-1597848212624-a19eb35e2651?w=500"]', 'cat-accessories', N'Ổ cứng di động SSD chống sốc, tốc độ 1050MB/s', GETUTCDATE(), GETUTCDATE());

GO

-- =============================================
-- VERIFY DATA
-- =============================================
PRINT '=============================================';
PRINT 'Sample Data Inserted Successfully!';
PRINT '=============================================';

SELECT 'Total Categories: ' + CAST(COUNT(*) AS VARCHAR(10)) AS Result FROM Categories;
SELECT 'Total Products: ' + CAST(COUNT(*) AS VARCHAR(10)) AS Result FROM Products;

PRINT '=============================================';

-- Display sample data
SELECT TOP 10 * FROM Products ORDER BY CreatedAt DESC;
GO
