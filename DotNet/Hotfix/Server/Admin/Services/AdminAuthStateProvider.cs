using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ET
{
    public class AdminAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly AuthService _authService;

        public AdminAuthStateProvider(AuthService authService)
        {
            _authService = authService;
            _authService.OnAuthStateChanged += NotifyAuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_authService.IsAuthenticated && _authService.CurrentUser != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _authService.CurrentUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, _authService.CurrentUser.Username),
                    new Claim(ClaimTypes.Role, _authService.CurrentUser.Role),
                };
                var identity = new ClaimsIdentity(claims, "AdminAuth");
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        private void NotifyAuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Dispose()
        {
            _authService.OnAuthStateChanged -= NotifyAuthStateChanged;
        }
    }
}
