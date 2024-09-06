using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



public class Validations 
{
    public static bool ValidatorRut(string rut)
    {
        
        if(rut.Length ==10)
        {
            rut = rut.Replace("-","");
        }
        else if (rut.Length == 12)
        {
            rut = rut.Replace(".","").Replace("-","");
        }
        string numeroStr = rut.Substring(0,rut.Length -1);
        char dv = rut[rut.Length-1];
        int numero;
        if(!int.TryParse(numeroStr,out numero))
        {
            return false;
        }
        int[] factores = {2,3,4,5,6,7};
        int suma =0;
        int factorIndex = 0;
        for (int i = numeroStr.Length-1;i >=0 ;i--)
        {
            suma += (numeroStr[i] - '0') * factores[factorIndex];
            factorIndex = (factorIndex+1)%factores.Length;
        }
        int resto = suma%11;
        char dvCalculado;
        if(resto == 1){
            dvCalculado = 'k';
        }
        else if(resto == 0){
            dvCalculado = '0';
        }else{
            dvCalculado = (char)(11-resto+'0');
        }


        return dvCalculado== char.ToUpper(dv);
    }
    public static string  GenerateJWToken(LoginBiblio loginBiblio)
    {
        
        var keyBytes = new byte[32]; // 256 bits = 32 bytes
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(keyBytes); // Rellenar con bytes aleatorios
        }
        var security = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, loginBiblio.idBiblio),
            new Claim(JwtRegisteredClaimNames.Email, loginBiblio.email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };
        var token = new JwtSecurityToken(
            issuer: "Biblioteca Juan Pablo 2",
            audience: loginBiblio.email,
            claims: claims,
            expires: DateTime.Now.AddHours(10),
            signingCredentials:credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}