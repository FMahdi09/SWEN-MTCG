using System.Text.Json.Serialization;

namespace SWEN.MTCG.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Cardtype
{
    monster,
    spell
}