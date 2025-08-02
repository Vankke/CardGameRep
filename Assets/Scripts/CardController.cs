using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardObject card;
    public TextMeshPro textMesh;
    public SpriteRenderer spriteRenderer;
    public GameController gameController;
    private void Start()
    {
        if (card == null)
            return;
        //textMesh = GetComponentInChildren<TextMeshPro>();
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textMesh.text = card.description;
        spriteRenderer.sprite = card.sprite;
        var renderer = GetComponent<SpriteRenderer>();
        if (card.cardColor == CardObject.CardColor.GREEN)
        {
            renderer.color = Color.green;
        }
        else if (card.cardColor == CardObject.CardColor.RED) 
        {
            renderer.color = Color.red;
        }
    }
}
