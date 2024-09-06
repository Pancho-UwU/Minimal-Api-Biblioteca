using Microsoft.EntityFrameworkCore;    

public class DataContext:DbContext
{   
    public DataContext(DbContextOptions<DataContext> options):base(options)
    {
    }
    
    public DbSet<Bibliotecaria> bibliotecaria {get;set;}

    public DbSet<Genero> genero {get;set;}

    public DbSet<LibroBibliotecaria> libroBibliotecaria {get;set;}
    public DbSet<Usuario> Usuario {get;set;}
    public DbSet<Libro> libro {get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configura la tabla de unión para la relación muchos a muchos
    modelBuilder.Entity<LibroBibliotecaria>()
        .HasKey(lb => new { lb.LibroId, lb.BibliotecariaId });

    modelBuilder.Entity<LibroBibliotecaria>()
        .HasOne(lb => lb.libro)
        .WithMany() // Asegúrate de tener esta colección en Libro
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
        .WithMany() // Asegúrate de tener esta colección en Genero
        .HasForeignKey(l => l.GeneroId);

    modelBuilder.Entity<Genero>()
        .HasKey(g => g.id);

    modelBuilder.Entity<Usuario>()
        .HasKey(u => u.id);

 
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