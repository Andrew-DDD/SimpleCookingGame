using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameRuleBase : MonoBehaviour
{
    //--------------------------------------------------------------
    //Rule Events
    public class GameRuleEvent : UnityEvent<string> { }
    [HideInInspector] public UnityEvent m_GameRuleStoppedEvent = new UnityEvent();
    [HideInInspector] public GameRuleEvent m_UpdateGameRuleEvent = new GameRuleEvent();
    //~Rule Events
    //--------------------------------------------------------------

    protected string progressInfo;
    protected string description;

    public string ProgressInfo { get { return progressInfo; } }
    public string Description { get { return description; } }

    private void OnDestroy()
    {
        m_GameRuleStoppedEvent.RemoveAllListeners();
        m_UpdateGameRuleEvent.RemoveAllListeners();
    }

    public virtual void Init(object[] p)
    {
        if (m_GameRuleStoppedEvent.GetPersistentEventCount() > 0) m_GameRuleStoppedEvent.RemoveAllListeners();
        if (m_UpdateGameRuleEvent.GetPersistentEventCount() > 0) m_UpdateGameRuleEvent.RemoveAllListeners();
    }
}
