using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Middleware.Exceptions;
using Middleware.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// exception handling with middleware
// app.Use(async (context, next) =>
// {
//     try
//     {
//         await next();
//     }
//     catch (Exception ex)
//     {
//         var response = context.Response;
//         response.ContentType = "application/json";
//         switch (ex)
//         {
//             case AppException e:
//                 // custom application error
//                 response.StatusCode = (int)HttpStatusCode.BadRequest;
//                 break;
//             case KeyNotFoundException e:
//                 // not found error
//                 response.StatusCode = (int)HttpStatusCode.NotFound;
//                 break;
//             default:
//                 // unhandled error
//                 response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                 break;
//         }
//         await context.Response.WriteAsJsonAsync(new { error = ex.Message });
//     }
// });

// app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseErrorHandler();

app.MapControllers();

app.Run();
