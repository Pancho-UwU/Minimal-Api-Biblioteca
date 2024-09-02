using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Asegúrate de usar el paquete correcto

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 5))));


var app = builder.Build();


var Libros = app.MapGroup("Libros");


Libros.MapPost("/",async (Libro libro,DataContext db) =>
{

    string query = $"INSERT INTO libros( name, author, pages, GeneroId) VALUES ( @name, @author,@pages,@GeneroId)";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@name",libro.name),
        new MySqlParameter("@author", libro.author),
        new MySqlParameter("@pages", libro.pages),
        new MySqlParameter("@GeneroId", libro.GeneroId)

    );
    
    return Results.Created($"/Libros/{libro.id}",libro);  
});



app.Run();
