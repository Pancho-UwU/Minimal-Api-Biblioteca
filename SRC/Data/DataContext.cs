using Microsoft.EntityFrameworkCore;    

public class DataContext:DbContext
{   
    public DataContext(DbContextOptions<DataContext> options):base(options)
    {
    }
    
    public DbSet<Bibliotecaria> bibliotecarias {get;set;}

    public DbSet<Genero> generos {get;set;}

    public DbSet<LibroBibliotecaria> libroBibliotecarias {get;set;}

    public DbSet<Profesor> profesors {get;set;}
    
    public DbSet<Alumno> alumnos {get;set;}

    public DbSet<Libro> libros {get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configura la tabla de unión para la relación muchos a muchos
    modelBuilder.Entity<LibroBibliotecaria>()
        .HasKey(lb => new { lb.LibroId, lb.BibliotecariaId });

    modelBuilder.Entity<LibroBibliotecaria>()
        .HasOne(lb => lb.libro)
        .WithMany(l => l.LibroBibliotecarias) // Asegúrate de tener esta colección en Libro
        .HasForeignKey(lb => lb.LibroId);

    modelBuilder.Entity<LibroBibliotecaria>()
        .HasOne(lb => lb.bibliotecaria)
        .WithMany(b => b.LibroBibliotecarias) // Asegúrate de tener esta colección en Bibliotecaria
        .HasForeignKey(lb => lb.BibliotecariaId);

    modelBuilder.Entity<LibroBibliotecaria>()
        .HasOne(lb => lb.usuario)
        .WithMany() // Si Usuario no tiene una colección para LibroBibliotecarias, mantén esto
        .HasForeignKey(lb => lb.userId);

    // Configura las claves primarias
    modelBuilder.Entity<Bibliotecaria>()
        .HasKey(b => b.id);

    modelBuilder.Entity<Libro>()
        .HasKey(l => l.id);

    // Configura la relación entre Libro y Genero
    modelBuilder.Entity<Libro>()
        .HasOne(l => l.genero)
        .WithMany(g => g.librosC) // Asegúrate de tener esta colección en Genero
        .HasForeignKey(l => l.GeneroId);

    modelBuilder.Entity<Genero>()
        .HasKey(g => g.id);

    modelBuilder.Entity<Usuario>()
        .HasKey(u => u.id);

    modelBuilder.Entity<Alumno>()
        .HasKey(a => a.id);

    modelBuilder.Entity<Profesor>()
        .HasKey(p => p.id);

    modelBuilder.Entity<Alumno>()
        .HasOne(a => a.usuario)
        .WithMany(u => u.alumnosC) // Asegúrate de tener esta colección en Usuario
        .HasForeignKey(a => a.idUsuario);

    modelBuilder.Entity<Profesor>()
        .HasOne(p => p.usuario)
        .WithMany(u => u.Profesors) // Asegúrate de tener esta colección en Usuario
        .HasForeignKey(p => p.usuarioId);
}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql("server=localhost;database=taller2;user=root;password=4565",
                new MySqlServerVersion(new Version(8,0,5)));
        }
        
    }

}