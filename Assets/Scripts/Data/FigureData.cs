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

    public GameObject[] GetFigureObjects() => figures.Select(x => x.Prefab).ToArray();
    public Sprite[] GetFigureSprites() => figures.Select(x => x.Icon).ToArray();

}