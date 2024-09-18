using Microsoft.AspNetCore.Http;

namespace attendance.Services
{
    public class HttpContextAccessorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Console.WriteLine("got a context");
        }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public string GetHeader(string headerName)
        {
            try {
                var val = _httpContextAccessor.HttpContext.Request.Headers[headerName].ToString();
                if (val == null || val.Length == 0) {
                    return "unknown";
                }
                return val;
            } catch ( Exception e) {
                Console.WriteLine("Error reading header: " + e.Message);
                return "unknown";
            }
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