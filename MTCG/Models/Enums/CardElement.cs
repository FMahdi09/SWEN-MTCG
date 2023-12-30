using System.Text.Json.Serialization;

namespace SWEN.MTCG.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardElement
{
    fire,
    water,
    earth,
    air,
    shadow,
    light
}