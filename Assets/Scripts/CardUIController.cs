using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour
{
    public HandCardObject handCard;
    public GameController gameController;
    int myIndex;
    RectTransform Parent;
    public TextMeshProUGUI descriptionText;

    private void Start()
    {
        if(Parent == null)
        {
            Parent = transform.parent.gameObject.GetComponent<RectTransform>();
        }
        descriptionText = GetComponentInChildren<TextMeshProUGUI>();
        descriptionText.text = handCard.Description;
        var img = GetComponent<Image>();
        if(handCard.cardColor == CardObject.CardColor.GREEN)
        {
            img.color = Color.green;
        }
        else if(handCard.cardColor == CardObject.CardColor.RED)
        {
            img.color = Color.red;
        }
    }
    private void Update()
    {
        myIndex = gameController.CardsInHand.IndexOf(this);
        if (gameController.CardsInHand.Count > 0)
        {
            float width = Parent.rect.width;
            float x = (width / (gameController.QuantityOfCardsInHand + 1) * (myIndex + 1)) - (width / 2);
            Vector3 myPos = new Vector3(x, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, myPos, 0.1f);
        }
    }
    public void PlayCard()
    {
        gameController.PlaySelectedCard(this);
    }
}
