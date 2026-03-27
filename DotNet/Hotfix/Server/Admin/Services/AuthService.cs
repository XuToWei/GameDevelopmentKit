
using LiteDB;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ET
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILiteCollection<AdminUser> _users;

        public event Action OnAuthStateChanged;
        public bool IsAuthenticated { get; private set; }
        public AdminUserInfo CurrentUser { get; private set; }

        public AuthService(IConfiguration configuration, AdminDatabase adminDb)
        {
            _configuration = configuration;
            _users = adminDb.Database.GetCollection<AdminUser>("users");
            _users.EnsureIndex(x => x.Username, true);
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;

            // Create default admin user if no users exist
            if (_users.Count() == 0)
            {
                var defaultUser = new AdminUser
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    Role = "SuperAdmin",
                    CreatedAt = DateTime.UtcNow,
                    IsEnabled = true
                };
                _users.Insert(defaultUser);
            }
        }

        public LoginResponse Login(LoginRequest request)
        {
            var user = _users.FindOne(x => x.Username == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "用户名或密码错误"
                };
            }

            if (!user.IsEnabled)
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = "账号已被禁用"
                };
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            _users.Update(user);

            // Generate JWT token
            var token = GenerateJwtToken(user);

            IsAuthenticated = true;
            CurrentUser = new AdminUserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            OnAuthStateChanged?.Invoke();

            return new LoginResponse
            {
                Success = true,
                Token = token,
                User = CurrentUser
            };
        }

        public void Logout()
        {
            IsAuthenticated = false;
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
        }

        public bool HasPermission(string requiredRole)
        {
            if (CurrentUser == null) return false;

            return CurrentUser.Role switch
            {
                "SuperAdmin" => true,
                "Operator" => requiredRole != "SuperAdmin",
                "ReadOnly" => requiredRole == "ReadOnly",
                _ => false
            };
        }

        private string GenerateJwtToken(AdminUser user)
        {
            var key = _configuration["Jwt:Key"] ?? "Admin_DefaultSecretKey_ChangeInProduction_32chars!";
            var issuer = _configuration["Jwt:Issuer"] ?? "Admin";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
