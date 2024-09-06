public class Libro
{
    public int id{get;set;}

    public string name {get;set;}

    public string author {get;set;}

    public int pages{get;set;}

    //claves foraneas

    public int GeneroId{get;set;}

    public Genero genero{get;set;}
    

}