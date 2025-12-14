namespace Stargate.Domain.Dtos
{
    public class UserResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }

    public class UserListResponse : BaseResponse
    {
        public IEnumerable<UserResponse> Users { get; set; } = new List<UserResponse>();
    }

    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class RoleListResponse : BaseResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; } = new List<RoleResponse>();
    }
}
