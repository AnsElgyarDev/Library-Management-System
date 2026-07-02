// suspended for a while 


// namespace Practice.Middlewares;

// class AuthorizationMiddleware
// {
//     private readonly RequestDelegate _next;
//     public AuthorizationMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }
//     public async Task InvokeAsync(HttpContext context)
//     {
//         var hasToken = context.Request.Headers.ContainsKey("Authorized");
        
//         if(!hasToken)
//         {
//             context.Response.StatusCode = 401;
//             await context.Response.WriteAsync("You are not Authorized!");
//             return;
//         }

//         await _next(context);
//     }
// }