using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardObject card;
    public TextMeshPro textMesh;
    public SpriteRenderer spriteRendererLogo;
    public GameObject SelectLightGameobject;
    public GameObject BackgroundGameobject;
    public GameController gameController;
    private void Start()
    {
        if (card == null)
            return;
        //textMesh = GetComponentInChildren<TextMeshPro>();
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textMesh.text = card.description;
        spriteRendererLogo.sprite = card.sprite;
        //var renderer = GetComponent<SpriteRenderer>();
        if (card.cardColor == CardObject.CardColor.GREEN)
        {
            BackgroundGameobject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (card.cardColor == CardObject.CardColor.RED) 
        {
            BackgroundGameobject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
