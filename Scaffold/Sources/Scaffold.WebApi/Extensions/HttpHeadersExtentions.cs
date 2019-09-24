namespace Scaffold.WebApi.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;

    public static class HttpHeadersExtentions
    {
        public static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
        {
            return headers.ToDictionary(header => header.Key, header => string.Join(", ", header.Value));
        }
    }
}
