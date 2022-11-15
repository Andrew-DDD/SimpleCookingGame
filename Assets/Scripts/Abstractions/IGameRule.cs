using System;


public interface IGameRule
{
    event Action GameStoppedEvent;
    event Action<string> UpdateGameRuleEvent;

    string ProgressInfo();
    string Description();

    void Init();
    void Tick(float delta);

    void OnGameStopped();
    void OnUpdateGameRule(string info);
}
