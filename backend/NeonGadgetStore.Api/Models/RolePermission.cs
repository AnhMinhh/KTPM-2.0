namespace NeonGadgetStore.Api.Models;

/// <summary>
/// RolePermission Class
/// Ánh xạ giữa Role và Permission
/// Cho phép gán quyền hạn cụ thể cho từng role
/// </summary>
public class RolePermission
{
    /// <summary>
    /// ID của Role
    /// </summary>
    public string RoleId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID của Permission
    /// </summary>
    public string PermissionId { get; set; } = string.Empty;
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
