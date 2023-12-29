using SWEN.MTCG.Models.Base;

namespace SWEN.MTCG.Models.DataModels;

public class User(string username, string password, int id = -1, string bio = "", string image = "", int currency = 0) : BattleAble(username)
{
    public int Id { get; set; } = id;
    public string Password { get; set; } = password;
    public string Bio { get; set; } = bio;
    public string Image { get; set; } = image;
    public int Currency { get; set; } = currency;
}