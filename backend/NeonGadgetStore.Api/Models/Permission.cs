namespace NeonGadgetStore.Api.Models;

/// <summary>
/// Permission Class - theo báo cáo DFD
/// Định nghĩa chi tiết các quyền hạn của hệ thống
/// </summary>
public class Permission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// permission_id - ID duy nhất của quyền
    /// </summary>
    public int? PermissionId { get; set; }
    
    /// <summary>
    /// permission_role_id - ID role liên kết
    /// </summary>
    public string? PermissionRoleId { get; set; }
    
    /// <summary>
    /// permission_title - Tên quyền hạn
    /// </summary>
    public string? PermissionTitle { get; set; }
    
    /// <summary>
    /// permission_module - Module/Tính năng (Shopping, Order, Product, Payment)
    /// </summary>
    public string? PermissionModule { get; set; }
    
    /// <summary>
    /// permission_description - Mô tả chi tiết quyền
    /// </summary>
    public string? PermissionDescription { get; set; }
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // Các phương thức cơ bản (từ báo cáo)
    public void AddPermission() { }
    public void DeletePermission() { }
    public void EditPermission() { }
    public List<Permission> SearchPermissions() => new List<Permission>();
}
