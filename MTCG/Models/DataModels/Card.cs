using SWEN.MTCG.Models.Enums;

namespace SWEN.MTCG.Models.DataModels;

public class Card(int id, string guid, string name, int damage, string element, Cardtype type)
{
    public int Id { get; set; } = id;
    public string Guid { get; set; } = guid;
    public string Name { get; set; } = name;
    public int Damage { get; set; } = damage;
    public string Element { get; set; } = element;
    public Cardtype Type { get; set; } = type;
}