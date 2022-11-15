using System;
using UnityEngine;


public class PCInput : MonoBehaviour, IInput
{
    #region IInput

    public event Action<int> AddFigureEvent;
    public event Action OrderEvent;
    public event Action RestartGameEvent;

    public void OnAddFigure(int figureIndex)
    {
        AddFigureEvent?.Invoke(figureIndex);
    }

    public void OnOrder()
    {
        OrderEvent?.Invoke();
    }

    public void OnRestartGame()
    {
        RestartGameEvent?.Invoke();
    }

    #endregion



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestartGameEvent();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OrderEvent();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            OnAddFigure(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            OnAddFigure(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            OnAddFigure(2);
        }
    }
}
