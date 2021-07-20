using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleInfinity : GameRuleBase
{
    public GameRuleInfinity()
    {
        description = "INF GAME";
        progressInfo = "";
    }

    public override void Init(object[] p)
    {
        base.Init(p);
        m_UpdateGameRuleEvent.Invoke(progressInfo);
    }
}
