using System.Collections.Concurrent;
using SWEN.MTCG.Models.Base;

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
           Task.Run(() => Battle(participantA, participantB));
        }
    }

    private void Battle(BattleAble participantA, BattleAble participantB)
    {
        // battle
        string battleLog = "Battle finished";

        // send result to participants
        participantA.BattleLog = battleLog;
        participantB.BattleLog = battleLog;
    }
}