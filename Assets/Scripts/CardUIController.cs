using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public HandCardObject handCard;
    public GameController gameController;
    int myIndex;
    RectTransform Parent;
    public TextMeshProUGUI descriptionText;
    public List<int> RandomIndexesToChange;

    public bool Pressed;
    public bool MouseOverThisElement;
    public float JitterTimer;

    private void Start()
    {
        RandomIndexesToChange = new List<int>();
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
            float y = 0;
            if(gameController.selectedCard == this)
            {
                y = 100;
            }
            if(JitterTimer > 0)
            {
                x += Random.Range(-25, 25);
                y += Random.Range(-25, 25);
                JitterTimer -= Time.deltaTime;
            }
            Vector3 myPos = new Vector3(x, y, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, myPos, 0.1f);
        }
    }
    public void PlayCard()
    {
        if (!Pressed)
        {
            gameController.PlaySelectedCard(this, false);
            Pressed = true;
        }
        else if(Pressed)
        {
            gameController.PlaySelectedCard(this, true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        MouseOverThisElement = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverThisElement = false;
        //throw new System.NotImplementedException();
    }
}
