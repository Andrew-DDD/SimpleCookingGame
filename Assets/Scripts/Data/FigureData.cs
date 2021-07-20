using UnityEngine;
using System.Linq;

[System.Serializable]
public struct Figure
{
    public GameObject Prefab;
    public Sprite Icon;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FigureData", order = 1)]
public class FigureData : ScriptableObject
{
    public Figure[] figures;

    public GameObject[] GetFigureObjects()
    {
        GameObject[] objs = new GameObject[figures.Length];
        for (int i = 0; i < figures.Length; i++) objs[i] = figures[i].Prefab;
        return objs;
    }

    public Sprite[] GetFigureSprites()
    {
        Sprite[] icons = new Sprite[figures.Length];
        for (int i = 0; i < figures.Length; i++) icons[i] = figures[i].Icon;
        return icons;
    }
}