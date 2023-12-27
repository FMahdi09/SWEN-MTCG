namespace SWEN.MTCG.Models.SerializationObjects;

public class AuthToken(string token)
{
    public string Token { get; set; } = token;
}