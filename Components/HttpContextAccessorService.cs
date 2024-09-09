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
        public string GetHeader(string headerName)
        {
            return _httpContextAccessor.HttpContext?.Request.Headers[headerName].ToString();
        }
        public HttpContext HttpContext => _httpContextAccessor.HttpContext;
    }
}