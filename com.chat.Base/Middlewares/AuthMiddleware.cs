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
                                "/api/v1/user/user/register",
                                 "/api/v1/user/user/forgotpassword",
                                "/api/v1/user/user/resetpassword",
                                "/api/v1/user/user/accountverification",
                                // "/chat",
                                "/chat/negotiate"
                        };

                var path = context.Request.Path.Value?.ToLower();

                var isWebSocketRequest = context.WebSockets.IsWebSocketRequest;


                Console.WriteLine(path);
                if (allowedPaths.Contains(path))
                {
                        await _next(context);
                        return;
                }
                var token = context?.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
                {
                        token = token.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrEmpty(token))
                {

                        context!.Response.StatusCode = 401;
                        await context!.Response.WriteAsync("No token provided");
                        return;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5090/api/Auth/validate");
                request.Headers.Add("Authorization", token);

                var response = await _httpClient.SendAsync(request);
                Console.WriteLine(response.IsSuccessStatusCode);

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
