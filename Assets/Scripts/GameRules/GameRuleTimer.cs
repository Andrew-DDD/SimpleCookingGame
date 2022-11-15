using System;


public class GameRuleTimer : IGameRule
{
    public event Action GameStoppedEvent;
    public event Action<string> UpdateGameRuleEvent;

    string description = "TIMER: ";
    string progressInfo;

    float time;
    bool countdown;
    bool tformat;

    float timer;
    bool timerActive;

    public string Description() => description;
    public string ProgressInfo() => progressInfo;


    public GameRuleTimer(float t, bool cd, bool tform)
    {
        time = t;
        countdown = cd;
        tformat = tform;
        timerActive = true;

        timer = countdown ? time : 0.0f;
    }


    public void Init()
    {

    }


    public void Tick(float delta)
    {
        if (!timerActive)
        {
            return;
        }

        timer += countdown ? -delta : delta;

        if (tformat)
        {
            progressInfo = GetFormatTime(timer);
        }
        else
        {
            progressInfo = Convert.ToInt32(timer).ToString();
        }

        bool timeEnd;
        if (countdown)
        {
            timeEnd = timer <= 0.0f;
        }
        else 
        {
            timeEnd = timer >= time;
        }

        if (!timeEnd)
        {
            return;
        }

        timerActive = false;
        OnGameStopped();
    }


    private string GetFormatTime(float time)
    {
        return string.Format("{0:00}:{1:00}", time / 60, time % 60);
    }

    public void OnGameStopped()
    {
        GameStoppedEvent?.Invoke();
    }

    public void OnUpdateGameRule(string info)
    {
        UpdateGameRuleEvent?.Invoke(info);
    }
}
