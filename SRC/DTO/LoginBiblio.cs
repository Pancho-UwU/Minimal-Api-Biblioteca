using System.ComponentModel.DataAnnotations;

public class LoginBiblio{
    public string idBiblio {get;set;}
    
    [EmailAddress]
    public string email {get;set;}
    public string password {get;set;}
}