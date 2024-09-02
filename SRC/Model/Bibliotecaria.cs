

public class Bibliotecaria
{
    public int id {get;set;}

    public string name {get;set;}

    public string password {get;set;}

    public string Rut {get;set;}

    public string email {get;set;}

    public bool IsVaild {get;set;}
    public ICollection<LibroBibliotecaria> LibroBibliotecarias { get; set; }

}
