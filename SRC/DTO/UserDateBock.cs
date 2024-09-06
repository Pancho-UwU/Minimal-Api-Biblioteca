using System.ComponentModel.DataAnnotations;

public class UserDateBock
{
    [Required]
    public string date_return {get;set;}
    [Required]
    public string date_deliver {get;set;}
}