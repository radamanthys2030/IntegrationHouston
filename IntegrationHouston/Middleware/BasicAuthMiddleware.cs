namespace IntegrationHouston.Middleware
{
    public class BasicAuthMiddleware
    {

        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;


            if (path.StartsWith("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var encoded = authHeader.Substring("Basic ".Length).Trim();
                    var decodedBytes = System.Convert.FromBase64String(encoded);
                    var decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);

                    var parts = decoded.Split(':');
                    var username = parts[0];
                    var password = parts[1];

                    // Cambia usuario y contraseña aquí
                    if (username == "swagger" && password == "$$Enero2025!!")
                    {
                        await _next(context);
                        return;
                    }
                }

                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }

    }
}
