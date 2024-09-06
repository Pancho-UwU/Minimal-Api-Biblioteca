using System.ComponentModel.DataAnnotations;

public class CreateBiblio 
{
    public int id {get;set;}
    public string? name {get;set;}

    public string? password {get;set;}

    public string? passwordConfirm {get;set;}
    [EmailAddress]
    public string? email {get;set;}

    public string? rut {get;set;}

   
}