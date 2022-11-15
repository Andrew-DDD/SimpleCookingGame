using System;


public interface IInput
{
    event Action<int> AddFigureEvent;
    event Action OrderEvent;
    event Action RestartGameEvent;

    void OnAddFigure(int figureIndex);
    void OnOrder();
    void OnRestartGame();
}