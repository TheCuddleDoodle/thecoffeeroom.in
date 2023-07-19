namespace Coffeeroom.Core.Helpers
{
    public class HttpUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpUtils(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentPageUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var url = $"{baseUrl}{request.Path}{request.QueryString}";

            return url;
        }
    }
}
