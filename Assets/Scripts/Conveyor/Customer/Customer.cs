using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Customer : MonoBehaviour
{
    private const int MAX_FIGURE_COUNT = 3;

    public class CustomerEvent : UnityEvent<bool> { }
    //--------------------------------------------------------------
    //Events
    /// <summary> b : waiting time has expired </summary>
    [HideInInspector] public CustomerEvent m_CustomerEvent = new CustomerEvent();
    [HideInInspector] public UnityEvent m_CustomerLeftEvent = new UnityEvent();
    //~Events
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Variables
    private float waitingTime = 10.0f;
    private float timer = 0.0f;

    private bool customerIsWaiting = false;

    private int[] figures = new int[MAX_FIGURE_COUNT];

    private Vector3 conveyorPosition;
    private Vector3 startPosition;

    private Tween moveTween;
    //~Variables
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Get
    public float WaitingTime { get { return waitingTime; } }
    public float Timer { get { return timer; } }

    public bool CustomerIsWaiting { get { return customerIsWaiting; } }

    public Vector3 ConveyorPosition { get { return conveyorPosition; } }
    public Vector3 StartPosition { get { return startPosition; } }

    public int[] GetFigures() { return figures; }
    //~Get
    //--------------------------------------------------------------




    public void Init(float waitingTime, GameObject[] figureObjs = null)
    {
        this.waitingTime = waitingTime;

        int figure_count = Random.Range(1, MAX_FIGURE_COUNT + 1);
        for (int i = 0; i < figure_count; i++) figures[i] = Random.Range(1, MAX_FIGURE_COUNT + 1);
        conveyorPosition = transform.position;
        startPosition = transform.position + transform.forward * 5.0f;

        MoveToConveyor();
        timer = this.waitingTime;
    }

    private void Update()
    {
        if (!customerIsWaiting) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        m_CustomerEvent.Invoke(true);
        MoveBack();
    }




    #region Order
    public void GetOrder(Tray tray)
    {
        tray.transform.SetParent(this.transform);
        m_CustomerEvent.Invoke(false);
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
        moveTween = transform.DOMove(conveyorPosition, 1.0f).OnComplete(() => { customerIsWaiting = true; });
    }
    private void MoveBack()
    {
        customerIsWaiting = false;
        m_CustomerLeftEvent.Invoke();
        moveTween.Kill();
        moveTween = transform.DOMove(startPosition, 1.0f).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }
    #endregion
}
