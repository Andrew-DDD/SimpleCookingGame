using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImputManager : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(IInput))] private Object gameInputInterfaceObj = default;
    private IInput gameInputInterface => gameInputInterfaceObj as IInput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            gameInputInterface.Order();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            gameInputInterface.AddFigure(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            gameInputInterface.AddFigure(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            gameInputInterface.AddFigure(2);
        }
    }
}
