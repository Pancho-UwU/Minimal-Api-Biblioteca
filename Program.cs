using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Asegúrate de usar el paquete correcto

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 5))));


var app = builder.Build();


var Libros = app.MapGroup("Libros");
var Bibliotecaria = app.MapGroup("Bibliotecaria");
var usuario = app.MapGroup("Usuario");
var arriendo = app.MapGroup("LibroBibliotecaria");

Libros.MapPost("/", [Authorize] async (Libro libro,DataContext db) =>
{
    string query = $"INSERT INTO libro( name, author, pages, GeneroId) VALUES ( @name, @author,@pages,@GeneroId)";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@name",libro.name),
        new MySqlParameter("@author", libro.author),
        new MySqlParameter("@pages", libro.pages),
        new MySqlParameter("@GeneroId", libro.GeneroId)
    );
    
    return Results.Created($"/Libros/{libro.id}",libro);  
});


/// <summary>
/// Endopint que representa la creación de una bibliotecaria.
/// </summary>
/// <param name="BibliotecariaDto"> DTO para verificar los datos de la bibliotecaria</param>
/// <param name="db">Base de datos.</param>
/// <returns>200 si es creada junto a su dto y 400 si hay algo mal</returns>
Bibliotecaria.MapPost("/", async (CreateBiblio BibliotecariaDto, DataContext db) => 
{
    
    if(BibliotecariaDto.password != BibliotecariaDto.passwordConfirm)
    {
        return Results.BadRequest("Las contraseñas no son iguales.");
    }
    string hashPassword = BCrypt.Net.BCrypt.HashPassword(BibliotecariaDto.password);
    string queryBusqueda = $"select * from bibliotecaria where Rut = @Rut";
    string queryBusquedaEmail = $"select * from bibliotecaria where email = @email";


    var data = await db.bibliotecaria.FromSqlRaw(queryBusqueda,new MySqlParameter("@Rut", BibliotecariaDto.rut)).ToArrayAsync();
    var emailData = await db.bibliotecaria.FromSqlRaw(queryBusquedaEmail,new MySqlParameter("@email",BibliotecariaDto.email)).ToArrayAsync();

    if(data.Length != 0 || emailData.Length !=0)
    {
        return Results.BadRequest("Usuario registrado.");
    }
    if(!Validations.ValidatorRut(BibliotecariaDto.rut)){
        return Results.BadRequest("no valido");
    }
    string query = $"Insert Into bibliotecaria(name,password,Rut,email,IsVaild) values (@name, @password, @Rut, @email, @IsVaild);";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@name", BibliotecariaDto.name),
        new MySqlParameter("@password", hashPassword),
        new MySqlParameter("@Rut", BibliotecariaDto.rut),
        new MySqlParameter("@email", BibliotecariaDto.email),
        new MySqlParameter("@IsVaild", 1)
    );
    return Results.Created($"/Bibliotecarias/{BibliotecariaDto.id}", BibliotecariaDto);
});
/// <summary>
/// Endpoint para obtener todas las bibliotecarias.
/// </summary>
/// <value></value>
Bibliotecaria.MapGet("/all",[Authorize]async(DataContext db)=>{
    string query = $"Select * from bibliotecaria";
    // para traer tablas tienen que ocupar esto ↓
    var data = await db.bibliotecaria.FromSqlRaw(query).ToArrayAsync();
    return Results.Ok(data);
});
/// <summary>
/// Endpoint para obtener todos los libros.
/// </summary>
/// <param name="db">Base de datos</param>
/// <returns></returns>
Libros.MapGet("/all",[Authorize] async(DataContext db) =>{
    string query = $"select *from libro;";
    var data = await db.libro.FromSqlRaw(query).ToArrayAsync();
    return Results.Ok(data);
});
/// <summary>
/// Endpoint para obtener un libro unico.
/// </summary>
/// <param name="db">Base de datos</param>
/// <param name="id">Id del libro.</param>
/// <returns></returns>
Libros.MapGet("/{id}", async (DataContext db, int id) =>
{
   return db.libro.Find(id) is Libro libro ? Results.Ok(libro) : Results.NotFound("No encontrado");
});


/// <summary>
/// Creación de un usuario de la para la inscripción de alumnos.
/// </summary>
/// <param name="user"></param>
/// <param name="db"></param>
/// <returns></returns>

usuario.MapPost("/CreateStudent", [Authorize]async (CreateStudent user, DataContext db) =>{
    string queryAlumno = $"INSERT INTO usuario(Name,cantidad,is_Student,is_Teacher) values(@Name,@cantidad,1,0);";
    await db.Database.ExecuteSqlRawAsync(queryAlumno,
        new MySqlParameter("@Name", user.name),
        new MySqlParameter("@cantidad", user.grade)
    );
    return Results.Created($"/Usuario/{user.name}",user.name);
});
/// <summary>
/// Endopoint del login de la bibliotecaria.
/// </summary>
/// <value>retorna el token si es correcto, si no, un 400 </value>

Bibliotecaria.MapGet("/login",[Authorize]async (DataContext db, [FromBody]LoginBiblio bibliotecaria) =>{
    string query = $"select * from bibliotecaria where email = @email";
    var data =  await db.bibliotecaria.FromSqlRaw(query, new MySqlParameter("@email",bibliotecaria.email)).FirstOrDefaultAsync();
    if(data == null)
    {
        return Results.BadRequest("Usuario no encontrado.");
    }
    bibliotecaria.idBiblio = data.id.ToString();
    var token = Validations.GenerateJWToken(bibliotecaria);
    return Results.Ok(new {token});
});
/// <summary>
/// Endpoint para crear un prestamo del libro.
/// </summary>
/// <value></value>
arriendo.MapPost("/Prestamo",[Authorize]async(DataContext db,int id_bliblio,int id_libro,int id_user, UserDateBock userDate)=>{
    var dataBiblio  = db.bibliotecaria.Find(id_bliblio);
    var dataLibro = db.libro.Find(id_libro);
    var dataUser = db.Usuario.Find(id_user);

    if(dataBiblio == null || dataUser == null || dataLibro == null)
    {
        return Results.BadRequest("Datos no encontrados");
    }
    
    string query = $"insert into librobibliotecaria(LibroId,BibliotecariaId,userId) values(@LibroId,@BibliotecariaId,@userId);";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@LibroId",id_libro),
        new MySqlParameter("@BibliotecariaId", id_bliblio),
        new MySqlParameter("@userId",id_user)
    );

    string queryDate = $"update usuario set date_return = @date_retrun, date_deliver=@date_deliver where id=@id";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@date_return",userDate.date_return),
        new MySqlParameter("@date_deliver", userDate.date_deliver),
        new MySqlParameter("@id", id_user)
    );
    return Results.Created($"/arriendo/Realizado",id_user);
});
usuario.MapPost("/CreateTeacher",[Authorize]async(DataContext db, createTeacher ct)=>{
    string query = $"Insert into usuario(Name,is_Student,is_Teacher) values(@Name,0,1)";
    await db.Database.ExecuteSqlRawAsync(query,
        new MySqlParameter("@Name", ct.name)
    );
    return Results.Created("Usuario/{user.name}",ct);
});


app.Run();
