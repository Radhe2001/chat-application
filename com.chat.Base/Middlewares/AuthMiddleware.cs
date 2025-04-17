using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace com.chat.Base.Middleware;

public class AuthMiddleware
{
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public AuthMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
                _next = next;
                _httpClient = httpClientFactory.CreateClient();
        }

        public async Task InvokeAsync(HttpContext context)
        {

                var allowedPaths = new[]
                        {
                                "/api/v1/user/user/login",
                                "/api/v1/user/user/sendmail",
                                "/api/v1/user/user/register",
                                "/chat",
                                "/chat/negotiate"
                        };

                var path = context.Request.Path.Value?.ToLower();

                var isWebSocketRequest = context.WebSockets.IsWebSocketRequest;

                // Bypass token validation for SignalR negotiate + WebSocket upgrade
                if ((path.StartsWith("/chat") && isWebSocketRequest) ||
                    path == "/chat/negotiate" ||
                    path == "/api/v1/user/user/login")
                {
                        await _next(context);
                        return;
                }
                Console.WriteLine(path);
                if (allowedPaths.Contains(path))
                {
                        await _next(context);
                        return;
                }
                var token = context.Request.Query["access_token"].FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                        var bearerToken = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(bearerToken) && bearerToken.StartsWith("Bearer "))
                        {
                                token = bearerToken.Substring("Bearer ".Length).Trim();
                        }
                }


                if (string.IsNullOrEmpty(token))
                {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("No token provided");
                        return;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5090/api/Auth/validate");
                request.Headers.Add("Authorization", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid token");
                        return;
                }

                var userInfo = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (userInfo == null || userInfo["userId"] == null || userInfo["role"] == null)
                {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid token");
                        return;
                }
                context.Request.Headers["X-User-Id"] = userInfo["userId"];
                context.Request.Headers["X-User-Role"] = userInfo["role"];
                await _next(context);
        }
}
