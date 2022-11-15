using System;
using UnityEngine;
using DG.Tweening;


public class Customer : MonoBehaviour
{
    private const int MAX_FIGURE_COUNT = 3;


    #region Events

    public event Action<bool> CustomerEvent;
    public event Action CustomerLeftEvent;

    #endregion


    #region Fields

    private float waitingTime = 10.0f;
    private float timer = 0.0f;

    private bool isCustomerWaiting = false;

    private int[] figures = new int[MAX_FIGURE_COUNT];

    private Vector3 conveyorPosition;
    private Vector3 startPosition;

    private Tween moveTween;

    #endregion


    #region Properties

    public float WaitingTime => waitingTime;
    public float Timer => timer;

    public bool IsCustomerWaiting => isCustomerWaiting;

    public Vector3 ConveyorPosition => conveyorPosition;
    public Vector3 StartPosition => startPosition;

    public int[] GetFigures() => figures;

    #endregion



    public void Init(float waitingTime, GameObject[] figureObjs = null)
    {
        this.waitingTime = waitingTime;

        int figure_count = UnityEngine.Random.Range(1, MAX_FIGURE_COUNT + 1);
        for (int i = 0; i < figure_count; i++) figures[i] = UnityEngine.Random.Range(1, MAX_FIGURE_COUNT + 1);
        conveyorPosition = transform.position;
        startPosition = transform.position + transform.forward * 5.0f;

        MoveToConveyor();
        timer = this.waitingTime;
    }

    private void Update()
    {
        if (!isCustomerWaiting) 
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0) return;

        CustomerEvent?.Invoke(true);
        MoveBack();
    }



    #region Order
    public void GetOrder(Tray tray)
    {
        tray.transform.SetParent(this.transform);
        CustomerEvent?.Invoke(false);
        MoveBack();
    }
    public void CancelOrder()
    {
        MoveBack();
    }
    #endregion



    #region MoveTo

    private void MoveToConveyor()
    {
        transform.position = startPosition;
        moveTween = transform.DOMove(conveyorPosition, 1.0f).OnComplete(() => { isCustomerWaiting = true; });
    }

    private void MoveBack()
    {
        isCustomerWaiting = false;
        CustomerLeftEvent?.Invoke();
        moveTween.Kill();
        moveTween = transform.DOMove(startPosition, 1.0f).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }

    #endregion
}
