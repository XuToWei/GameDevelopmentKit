
using LiteDB;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ET
{
    public class AuthService
    {
        private const string Pbkdf2Prefix = "PBKDF2";
        private const int Pbkdf2Iterations = 120_000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        private readonly IConfiguration _configuration;
        private readonly ILiteCollection<AdminUser> _users;

        public event Action OnAuthStateChanged = delegate { };
        public bool IsAuthenticated { get; private set; }
        public AdminUserInfo CurrentUser { get; private set; }

        public AuthService(IConfiguration configuration, AdminDatabase adminDb)
        {
            _configuration = configuration;
            _users = adminDb.Database.GetCollection<AdminUser>("users");
            _users.EnsureIndex(x => x.Username, true);
        }

        public void Initialize()
        {
            // Create default admin user if no users exist
            if (_users.Count() == 0)
            {
                var bootstrapUsername = GetRequiredSetting("Admin:BootstrapUsername");
                var bootstrapPassword = GetRequiredSetting("Admin:BootstrapPassword");
                var defaultUser = new AdminUser
                {
                    Username = bootstrapUsername,
                    PasswordHash = HashPassword(bootstrapPassword),
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

            user.LastLoginAt = DateTime.UtcNow;
            _users.Update(user);

            IsAuthenticated = true;
            CurrentUser = new AdminUserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            OnAuthStateChanged();

            return new LoginResponse
            {
                Success = true,
                User = CurrentUser
            };
        }

        public void Logout()
        {
            IsAuthenticated = false;
            CurrentUser = null;
            OnAuthStateChanged();
        }

        public void RestoreSession(ClaimsPrincipal principal)
        {
            var identity = principal.Identity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return;
            }

            if (!int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                return;
            }

            var username = identity.Name;
            var role = principal.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
            {
                return;
            }

            IsAuthenticated = true;
            CurrentUser = new AdminUserInfo
            {
                Id = userId,
                Username = username,
                Role = role,
            };
        }

        private string GetRequiredSetting(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Missing required configuration: {key}");
            }

            return value;
        }

        private static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Pbkdf2Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{Pbkdf2Prefix}${Pbkdf2Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string hash)
        {
            if (!hash.StartsWith($"{Pbkdf2Prefix}$", StringComparison.Ordinal))
            {
                return false;
            }

            var parts = hash.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations) || iterations <= 0)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
