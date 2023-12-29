using System.Collections.Concurrent;
using System.Text;
using SWEN.MTCG.Models.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.BusinessLogic.Battle;

public class BattleManager
{
    private readonly BlockingCollection<BattleAble> _queue = [];

    public void QueueUp(BattleAble toQueue)
    {
        _queue.Add(toQueue);
    }

    public void StartMatchmaking()
    {
        while(true)
        {
            // get 2 participants
            BattleAble participantA = _queue.Take();
            BattleAble participantB = _queue.Take();

           // start battle
           Task.Run(() => StartBattle(participantA, participantB));
        }
    }

    private void StartBattle(BattleAble participantA, BattleAble participantB)
    {
        // battle
        string battleLog = Battle(participantA, participantB);

        // send result to participants
        participantA.BattleLog = battleLog;
        participantB.BattleLog = battleLog;
    }

    private string Battle(BattleAble participantA, BattleAble participantB)
    {
        StringBuilder battleLog = new();
        battleLog.AppendLine("Battle started");

        for(int rounds = 0; rounds < 10; ++rounds)
        {
            // get random card from each participant
            if(participantA.DrawRandomCard() is not Card cardA)
            {
                battleLog.AppendLine($"{ participantA.Username } is out of cards. { participantB.Username } wins the game.");
                return battleLog.ToString();
            }
            
            if(participantB.DrawRandomCard() is not Card cardB)
            {
                battleLog.AppendLine($"{ participantB.Username } is out of cards. { participantA.Username } wins the game.");
                return battleLog.ToString();
            }
            
            battleLog.AppendLine($"{ participantA.Username } : { cardA.Name } ({ cardA.Damage } Damage) vs  { participantB.Username } : { cardB.Name } ({ cardB.Damage } Damage)");


        }

        battleLog.AppendLine("Battle ended");

        return battleLog.ToString();
    }
}