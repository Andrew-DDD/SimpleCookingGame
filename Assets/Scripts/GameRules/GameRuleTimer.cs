using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleTimer : GameRuleBase
{
    public GameRuleTimer()
    {
        description = "TIMER: ";
    }
    /// <summary>
    /// p0 = time; p1 = countdown; p2 = time format;
    /// </summary>
    /// <param name="p"></param>
    public override void Init(object[] p)
    {
        base.Init(p);

        int t = Convert.ToInt32(p[0]);
        bool countdown = Convert.ToBoolean(p[1]);
        bool tformat = Convert.ToBoolean(p[2]);

        StartCoroutine(TimerStart(t, countdown, tformat));
    }

    private IEnumerator TimerStart(int t, bool c = false, bool f = false)
    {
        yield return new WaitForEndOfFrame();

        int timer = 0;
        if (c) timer = t;
        while (c ? timer >= 0 : timer < t)
        {
            if (f) progressInfo = GetFormatTime(timer);
            else progressInfo = timer.ToString();
            
            timer += c ? -1 : 1;

            m_UpdateGameRuleEvent.Invoke(progressInfo);
            yield return new WaitForSeconds(1.0f);
        }
        m_GameRuleStoppedEvent.Invoke();
    }

    private string GetFormatTime(int time)
    {
        return string.Format("{0:00}:{1:00}", time / 60, time % 60);
    }
}
