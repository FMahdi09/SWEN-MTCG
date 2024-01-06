using System.Collections.Concurrent;
using System.Text.Json;
using Npgsql;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.Businesslogic.Battle;
using SWEN.MTCG.BusinessLogic.Attributes;
using SWEN.MTCG.BusinessLogic.Battle;
using SWEN.MTCG.DataAccess.UnitOfWork;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.BusinessLogic.EndpointHandlers;

[EndPointHandler]
public class BattleHandler
{
    private readonly string _connectionString;
    private readonly BattleManager _battleManager = new();

    public BattleHandler(string connectionString)
    {
        _connectionString = connectionString;

        Task.Run(_battleManager.StartMatchmaking);
    }


    [EndPoint(HttpMethods.POST, "/battles")]
    public HttpResponse PostBattle(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // get deck from user
            List<Card> deck = unit.DeckRepository.GetDeck(user);

            if(!user.SetDeck(deck))
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Set Deck before battling")));

            // enter user in queue
            _battleManager.QueueUp(user);

            // wait for battle result
            string battleLog = user.WaitForBattleLog();

            // check for win or loss
            if(user.BattleResult == BattleResult.win)
                unit.ScoreRepository.AddWin(user);
            else if(user.BattleResult == BattleResult.lose)
                unit.ScoreRepository.AddLoss(user);

            // add history
            unit.MatchHistoryRepository.InsertHistory(user.History, user.BattleResult, user);

            // return battle result
            return new HttpResponse("200 OK", JsonSerializer.Serialize(battleLog));
        }
        catch(JsonException)
        {
            return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));
        }
        catch(NpgsqlException)
        {
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect to database")));
        }
    }
}