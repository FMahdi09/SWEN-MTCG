namespace SWEN.MTCG.Models.SerializationObjects;

internal class Error(string errorMessage)
{
    public string ErrorMessage { get; set; } = errorMessage;
}