using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Admin
{
    public int AdminId { get; set; }

    [Required]
    [StringLength(50)]
    public string AdminName { get; set; }

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } // dùng SHA256 hash

    [StringLength(50)]
    public string AdminEmail { get; set; }

    [StringLength(20)]
    public string AdminPhone { get; set; }

    [StringLength(200)]
    public string AdminAddress { get; set; }

    // Không lưu password plaintext
    public string AdminPassword { get; set; }

}
