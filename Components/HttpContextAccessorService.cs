using Microsoft.AspNetCore.Http;

namespace attendance.Components
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
    }
}