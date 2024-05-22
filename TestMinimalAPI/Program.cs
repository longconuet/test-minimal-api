using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using TestMinimalAPI;
using TestMinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// configure exception middleware
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/books", (IBookService bookService) =>
    TypedResults.Ok(bookService.GetBooks()))
    .WithName("GetBooks")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Get Library Books",
        Description = "Returns information about all the available books from the library.",
        Tags = new List<OpenApiTag> { new() { Name = "Long's Library" } }
    });

app.MapGet("/books/{id}", Results<Ok<Book>, NotFound> (IBookService bookService, int id) =>
        {
            var book = bookService.GetBook(id);
            if (book is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(book);
        })
    .WithName("GetBookById")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Get Library Book By Id",
        Description = "Returns information about selected book from the Amy's library.",
        Tags = new List<OpenApiTag> { new() { Name = "Long's Library" } }
    });

app.Run();
