using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ET
{
    public class AdminAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly AuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminAuthStateProvider(AuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _authService.OnAuthStateChanged += NotifyAuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var requestUser = httpContext.User;
                if (requestUser.Identity != null && requestUser.Identity.IsAuthenticated)
                {
                    _authService.RestoreSession(requestUser);
                    return Task.FromResult(new AuthenticationState(requestUser));
                }
            }

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
