#pragma warning disable 0649

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class ConveyorManager : MonoBehaviour
{

    #region Events

    public event Action<Customer> SpawnCustomerEvent;
    public event Action<int> TrayFigureCountEvent;
    public event Action TrayMoveToTrashEvent;

    #endregion


    #region Fields

    [Header("Prefabs")]
    [SerializeField] private Tray trayPrefab;
    [SerializeField] private Customer customerPrefab;
    [Header("Points")]
    [SerializeField] private GameObject traySpawnPoint;
    [SerializeField] private GameObject conveyorTrashPoint;
    [SerializeField] private GameObject[] customerPoints;

    private bool isConveyorWorking;
    private float customerSpawnDelay = 3.0f;
    private Vector2 customerWaitingTime = new Vector2(5, 15);
    private Tray currentTry;
    private List<Customer> customers = new List<Customer>();
    private GameObject[] figurePrefabs;

    #endregion


    #region Properties

    public bool IsConveyorWorking => isConveyorWorking;
    public Tray CurrentTry => currentTry;

    #endregion



    #region Conveyor

    public void StartConveyor(float spawnDelay, Vector2 waitingTime, GameObject[] figures = null)
    {
        isConveyorWorking = true;
        customerSpawnDelay = spawnDelay;
        customerWaitingTime = waitingTime;
        figurePrefabs = figures;

        EnterNewTray();
        StartSpawnCustomers(customerSpawnDelay);
    }

    public void StopConveyor()
    {
        isConveyorWorking = false;
        foreach (var customer in customers) customer.CancelOrder();
        foreach (var point in customerPoints) point.SetActive(true);
        customers.Clear();
        if (currentTry) currentTry.TrayToTrash();
    }

    #endregion



    #region Tray

    public void EditTrayComplete()
    {
        if (!isConveyorWorking) 
        {
            return;
        }

        if (currentTry)
        {
            var valid_customers = 
                (from customer in customers where Enumerable.SequenceEqual(customer.GetFigures().OrderBy(t => t), 
                currentTry.GetFigures().OrderBy(t => t)) select customer).ToArray();

            Customer valid_customer = null;
            Tray tray = currentTry;

            if (valid_customers.Length > 0)
            {
                valid_customer = valid_customers[tray.transform.GetClosestObject(valid_customers)]; //<= Select nearest customer
            }

            if (valid_customer)
            {
                Vector3 customer_pos = new Vector3(valid_customer.transform.position.x, tray.transform.position.y, tray.transform.position.z);
                tray.OnEditComplete(customer_pos)
                    .OnComplete(() => 
                    {
                        if (!valid_customer) tray.OnEditComplete(conveyorTrashPoint.transform.position).OnComplete(() => { TrayToTrash(tray); });
                        else
                        {
                            if (valid_customer.IsCustomerWaiting) valid_customer.GetOrder(tray);
                            else tray.OnEditComplete(conveyorTrashPoint.transform.position).OnComplete(() => { TrayToTrash(tray); });
                        }
                    });
            }
            else
            {
                tray.OnEditComplete(conveyorTrashPoint.transform.position).OnComplete(()=> { TrayToTrash(tray); });
            }
        }
        EnterNewTray();
    }

    public void EnterNewTray()
    {
        if (!isConveyorWorking) 
        {
            return;
        }

        currentTry = Instantiate(trayPrefab, traySpawnPoint.transform.position, Quaternion.identity);
        currentTry.Init(figurePrefabs);
        TrayFigureCountEvent?.Invoke(0);
    }

    public void AddFigure(int figure)
    {
        if (!isConveyorWorking)
        {
            return;
        }

        if (currentTry)
        {
            int figureCount = currentTry.AddFigure(figure);
            TrayFigureCountEvent?.Invoke(figureCount);
        }
    }

    private void TrayToTrash(Tray tray)
    {
        TrayMoveToTrashEvent?.Invoke();
        tray.TrayToTrash();
    }

    #endregion



    #region Customer

    public void StartSpawnCustomers(float delay)
    {
        if (!isConveyorWorking)
        {
            return;
        }

        StartCoroutine(CheckNeedNewCustomer());
    }
    private IEnumerator CheckNeedNewCustomer()
    {
        while (customers.Count < customerPoints.Length && isConveyorWorking)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(customerSpawnDelay);
        }
    }
    private Customer SpawnCustomer()
    {
        var free_points = (from t in customerPoints where t.activeSelf select t).ToArray();
        int randpoint = UnityEngine.Random.Range(0, free_points.Count() - 1);
        GameObject point = free_points[randpoint];
        point.SetActive(false);
        Customer customer = Instantiate(customerPrefab, point.transform.position, Quaternion.identity);
        customers.Add(customer);
        float randomTime = UnityEngine.Random.Range(customerWaitingTime.x, customerWaitingTime.y);
        customer.Init(randomTime);
        SpawnCustomerEvent?.Invoke(customer);

        Action<bool> handler = null;
        handler = (arg) =>
        {
            customer.CustomerEvent -= handler;
            point.SetActive(true);
            StartCoroutine(CheckNeedNewCustomer());
            customers.Remove(customer);
        };
        customer.CustomerEvent += handler;

        return customer;
    }

    #endregion
}
