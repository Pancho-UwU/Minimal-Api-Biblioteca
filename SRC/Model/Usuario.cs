
public class Usuario
{
    public int id{get;set;}

    public string Name {get;set;}
    public DateTime date_return{get;set;}
    public DateTime date_deliver{get;set;}
    

    public ICollection<Alumno> alumnosC {get;set;}
    public ICollection<Profesor> Profesors {get;set;}
}