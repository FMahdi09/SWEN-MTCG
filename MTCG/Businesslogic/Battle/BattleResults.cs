using System.Text.Json.Serialization;

namespace SWEN.MTCG.Businesslogic.Battle;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BattleResult
{
    win,
    lose,
    draw
}