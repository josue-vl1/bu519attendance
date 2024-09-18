using Microsoft.AspNetCore.Http;

namespace attendance.Services
{
    public class HttpContextAccessorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public string GetHeader(string headerName)
        {
            return _httpContextAccessor.HttpContext?.Request.Headers[headerName].ToString();
        }

        public string AllHeaders() {
            if (_httpContextAccessor.HttpContext?.Request.Headers == null)
            {
                return "No headers available";
            }

            return string.Join(", ", _httpContextAccessor.HttpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));

        }
        public ISession Session => _httpContextAccessor.HttpContext?.Session;
    }
}