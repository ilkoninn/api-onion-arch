public class GlobalExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            var response = new
            {
                Message = ex.Message,
                Type = ex.GetType().Name,
                Path = ctx.Request.Path.Value
            };

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.SendAsync(response);
        }
    }
}
