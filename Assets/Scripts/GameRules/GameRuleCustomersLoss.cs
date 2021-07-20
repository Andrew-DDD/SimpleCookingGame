using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameRuleCustomersLoss : GameRuleBase
{
    private int customerLossIndexer;
    private int customersLossCount;

    public GameRuleCustomersLoss()
    {
        description = "Customer loss: ";
    }
    /// <summary>
    /// p0 = loss count (int); p1 = customers loss action (Action);
    /// </summary>
    /// <param name="p"></param>
    public override void Init(object[] p)
    {
        base.Init(p);

        customerLossIndexer = 0;
        customersLossCount = Convert.ToInt32(p[0]);
        UnityEvent action = p[1] as UnityEvent;
        action.AddListener(() => 
        {
            CheckToEndRule();
        });

        progressInfo = "0" + "/" + customersLossCount.ToString();
        m_UpdateGameRuleEvent.Invoke(progressInfo);
    }

    private void CheckToEndRule()
    {
        customerLossIndexer++;
        progressInfo = customerLossIndexer.ToString() + "/" + customersLossCount.ToString();
        m_UpdateGameRuleEvent.Invoke(progressInfo);
        if (customerLossIndexer >= customersLossCount)
        {
            m_GameRuleStoppedEvent.Invoke();
        }
    }
}
