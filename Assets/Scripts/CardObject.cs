//using TMPro;
using UnityEngine;
[CreateAssetMenu]
public class CardObject : ScriptableObject
{
    public enum CardColor { RED, GREEN};

    public GameObject prefab;
    public Sprite sprite;
    //public TextMeshPro textMesh;
    public string description;
    public string Type;
    public CardColor cardColor;
}
