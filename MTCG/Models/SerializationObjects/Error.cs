namespace SWEN.MTCG.Models.SerializationObjects;

public class Error(string errorMessage)
{
    public string ErrorMessage { get; set; } = errorMessage;
}