using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Tray : MonoBehaviour
{
    private const int MAX_FIGURE_COUNT = 3;

    //--------------------------------------------------------------
    //Properties
    [SerializeField] private Transform[] m_FiguresPoints = new Transform[3];
    //~Properties
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    //Varuables
    private int[] figures = new int[MAX_FIGURE_COUNT];
    private GameObject[] figuresPrefabs;

    private int figureCount = 0;
    private Tween activetween;
    //~Varuables
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Get
    public int[] GetFigures() { return figures; }
    //~Get
    //--------------------------------------------------------------




    #region Initiolize
    public void Start()
    {
        transform.position -= Vector3.right * 2.0f;
        activetween = transform.DOMove(transform.position + Vector3.right * 2.0f, 1.0f);
    }
    public void Init(GameObject[] figureObjs = null)
    {
        figuresPrefabs = figureObjs;
    }
    #endregion




    #region Tray
    public Tween OnEditComplete(Vector3 endPos)
    {
        float distance = (transform.position - endPos).magnitude;
        float speed = distance * 0.5f;
        activetween = transform.DOMove(endPos, speed).SetEase(Ease.Linear);
        return activetween;
    }

    public int AddFigure(int figure)
    {
        if (figureCount < figures.Length)
        {
            figures[figureCount] = figure + 1;
            GameObject go = Instantiate(figuresPrefabs[figure], m_FiguresPoints[figureCount]);
            go.transform.localScale = Vector3.zero;
            go.transform.DOScale(1.0f, 0.1f);
            figureCount += 1;
            go.transform.localPosition = Vector3.zero;
        }
        return figureCount;
    }

    public void TrayToTrash()
    {
        activetween.Kill();
        activetween = transform.DOScale(0.0f, 0.25f).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }
    #endregion
}
