using MaxVerse.API.Models;
using System.Linq;

namespace MaxVerse.API.Data;

/// <summary>
/// Seeder tạo tài khoản Admin mặc định khi khởi động ứng dụng (nếu chưa có).
/// </summary>
public static class DbSeeder
{
    public static void SeedAdmin(MaxVerseDbContext context)
    {
        // 1. Tìm hoặc tự động tạo Role "Admin" trước để giải quyết triệt để lỗi Khóa ngoại (Foreign Key)
        var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");

        if (adminRole == null)
        {
            adminRole = new Role
            {
                RoleName = "Admin"
                // Nếu DB của bạn bắt buộc nhập Id bằng tay và không tự tăng, hãy mở comment dòng dưới:
                // Id = 1 
            };
            context.Roles.Add(adminRole);
            context.SaveChanges(); // Lưu vào DB trước để sinh Id hợp lệ
        }

        // 2. Kiểm tra xem tài khoản Admin đã tồn tại dựa trên Email chưa
        var adminExists = context.Users.Any(u => u.Email == "admin@maxverse.com");
        if (adminExists) return;

        // 3. Tiến hành tạo tài khoản Admin an toàn
        var admin = new User
        {
            FullName = "Quản trị viên MaxVerse",
            Email = "admin@maxverse.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            RoleId = adminRole.RoleId, // Lấy Id thực tế từ Role vừa tìm/tạo ở bước 1
            IsActive = true
        };

        context.Users.Add(admin);
        context.SaveChanges();
    }
}