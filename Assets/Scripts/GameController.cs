using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<CardObject> TableDeck;
    public List<HandCardObject> HandDeck;

    public GameObject playingField;
    public List<Transform> CardPlaces;

    public List<CardController> CardsOnTable;
    public List<CardUIController> CardsInHand;
    public List<HandCardObject> WastedCards;

    public int QuantityOfCardsInHand;

    public GameObject UICanvas;
    public GameObject ZaHand;

    public GameObject Pointer;
    public int StepIndex;
    public int CurrentHour;

    public Vector3 PointerTarget;

    void Start()
    {
        CardsOnTable = new List<CardController>();
        playingField = GameObject.Find("Playing field");
        var children = playingField.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            CardPlaces.Add(playingField.transform.GetChild(i));
        }

        var arrayOfTableCards = Resources.LoadAll("ScriptableObjects/TableCards", typeof(CardObject));
        Debug.Log(arrayOfTableCards.Length);
        List<CardObject> TableListInOrder = new List<CardObject>();
        for (int i = 0; i < arrayOfTableCards.Length; i++)
        {
            TableListInOrder.Add((CardObject)arrayOfTableCards[i]);
        }
        while (TableListInOrder.Count > 0)
        {
            var randIndex = Random.Range(0, TableListInOrder.Count);
            TableDeck.Add(TableListInOrder[randIndex]);
            TableListInOrder.RemoveAt(randIndex); //(digitsInOrder[randIndex]);
        }



        for (int i = 0; i < CardPlaces.Count; i++)
        {
            PlaceCardOnTable(TableDeck[Random.Range(0, TableDeck.Count)], i);
        }

        PointerTarget = transform.up;

        var arrayOfHandCards = Resources.LoadAll("ScriptableObjects/HandCards", typeof(HandCardObject));
        Debug.Log(arrayOfHandCards.Length);
        List<HandCardObject> ListInOrder = new List<HandCardObject>();
        for(int i = 0; i < arrayOfHandCards.Length; i++)
        {
            ListInOrder.Add((HandCardObject)arrayOfHandCards[i]);
        }
        while(ListInOrder.Count > 0)
        {
            var randIndex = Random.Range(0, ListInOrder.Count);
            HandDeck.Add(ListInOrder[randIndex]);
            ListInOrder.RemoveAt(randIndex); //(digitsInOrder[randIndex]);
        }
        //MakeNewStep();
    }

    void Update()
    {
        Pointer.transform.up = Vector3.Lerp(Pointer.transform.up, PointerTarget, 0.1f);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeNewStep();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < CardPlaces.Count; i++)
            {
                PlaceCardOnTable(TableDeck[Random.Range(0, TableDeck.Count)], i);
            }
        }
    }

    public void PlaceCardOnTable(CardObject card, int index)
    {
        GameObject spawningObj = card.prefab;
        var instantiated = Instantiate(spawningObj, CardPlaces[index].transform.position, Quaternion.identity);
        var cardController = instantiated.GetComponent<CardController>();
        cardController.card = card;
        cardController.gameController = this;
        if (CardsOnTable.Count <= index)
        {
            CardsOnTable.Add(cardController);
        }
        else
        {
            Destroy(CardsOnTable[index].gameObject);
            CardsOnTable[index] = cardController;
        }
    }

    public void GiveCardToHand(HandCardObject card)
    {
        GameObject spawningObj = card.prefab;
        var instantiated = Instantiate(spawningObj, ZaHand.transform);
        var handCardController = instantiated.GetComponent<CardUIController>();
        handCardController.handCard = card;
        handCardController.gameController = this;
        //handCardController.Parent = (RectTransform)ZaHand.transform;
        CardsInHand.Add(handCardController);
    }
    public void RemoveCardFromHand(CardUIController removedCard)
    {
        CardsInHand.Remove(removedCard);
        WastedCards.Add(removedCard.handCard);
        Destroy(removedCard.gameObject);
    }

    public void MakeNewStep()
    {
        StepIndex++;
        var currentHour = StepIndex % 12;
        CurrentHour = currentHour;
        Debug.Log(currentHour);
        PointerTarget = Quaternion.AngleAxis(-30 * currentHour, Vector3.forward) * transform.up;
        if (CardsInHand.Count < 5)
        {
            int MissingHandCards = 5 - CardsInHand.Count;
            for (int i = 0; i < MissingHandCards; i++)
            {
                if (HandDeck.Count <= 0)
                {
                    Reschuffle();
                }
                var randomIndex = Random.Range(0, HandDeck.Count);
                var randomCard = HandDeck[randomIndex];
                HandDeck.Remove(randomCard);
                GiveCardToHand(randomCard);
            }
        }
    }

    public void Reschuffle()
    {
        while (WastedCards.Count > 0)
        {
            int rand = Random.Range(0, WastedCards.Count);
            HandDeck.Add(WastedCards[rand]);
            WastedCards.RemoveAt(rand);
        }
    }
    public void PlaySelectedCard(CardUIController cardController)
    {
        var handCard = cardController.handCard;
        bool CardPlayedSuccesful = false;
        foreach (HandCardObject.EffectOfCard effect in handCard.effects)
        {
            List<int> indexesToChange = new List<int>();
            switch (effect.areaOfEffect)
            {
                case HandCardObject.AreaOfEffect.ONLY_CURRENT:
                    indexesToChange.Add(CurrentHour);
                    break;
                case HandCardObject.AreaOfEffect.NEXT_ADJACENT: 
                    for (int i = 0; i < effect.QuantityOfEffect + 1; i++)
                    {
                        indexesToChange.Add((CurrentHour + i) % 12);
                    }
                    if (!effect.ChangeCurrentToo)
                    {
                        indexesToChange.Remove(CurrentHour);
                    }
                    break;
                case HandCardObject.AreaOfEffect.PREVIOUS_ADJACENT: 
                    for (int i = 0; i < effect.QuantityOfEffect + 1; i++)
                    {
                        var indToChange = CurrentHour - i;
                        if(indToChange < 0)
                        {
                            indToChange += 12;
                        }
                        indexesToChange.Add(indToChange);
                    }
                    if (!effect.ChangeCurrentToo)
                    {
                        indexesToChange.Remove(CurrentHour);
                    }
                    break;
                case HandCardObject.AreaOfEffect.ALL_ADJACENT:
                    for (int i = 0; i < effect.QuantityOfEffect + 1; i++)
                    {
                        indexesToChange.Add((CurrentHour + i) % 12);
                        var indToChange = CurrentHour - i;
                        if (indToChange < 0)
                        {
                            indToChange += 12;
                        }
                        indexesToChange.Add(indToChange);
                    }
                    if (!effect.ChangeCurrentToo)
                    {
                        indexesToChange.Remove(CurrentHour);
                    }
                    break;
                case HandCardObject.AreaOfEffect.ALL_CARDS:
                    for(int i = 0; i < CardsOnTable.Count; i++)
                    {
                        indexesToChange.Add(i);
                    }
                    if (!effect.ChangeCurrentToo)
                    {
                        indexesToChange.Remove(CurrentHour);
                    }
                    break;
                case HandCardObject.AreaOfEffect.RANDOM_CARDS:
                    var TableCardsPlaceholder = new List<int>();
                    for (int i = 0; i < CardsOnTable.Count; i++)
                    {
                        TableCardsPlaceholder.Add(i);
                    }
                    for (int i = 0; i < effect.QuantityOfEffect; i++)
                    {
                        int randIndex = TableCardsPlaceholder[Random.Range(0, TableCardsPlaceholder.Count)];
                        indexesToChange.Add(randIndex);
                        TableCardsPlaceholder.RemoveAt(randIndex);
                    }
                    break;
                default:
                    break;
            }
            List<CardObject> greenCards = new List<CardObject>();
            foreach (CardObject card in TableDeck)
            {
                if (card.cardColor == CardObject.CardColor.GREEN)
                {
                    greenCards.Add(card);
                }
            }
            List<CardObject> redCards = new List<CardObject>();
            foreach (CardObject card in TableDeck)
            {
                if (card.cardColor == CardObject.CardColor.RED)
                {
                    redCards.Add(card);
                }
            }
            foreach(int i in indexesToChange)
            {
                Debug.Log(i + "!!!");
            }
            if (effect.SwitchToGreen)
            {
                for(int i = 0; i < indexesToChange.Count; i++)
                {
                    if(CardsOnTable[indexesToChange[i]].card.cardColor == CardObject.CardColor.RED)
                    {
                        PlaceCardOnTable(greenCards[Random.Range(0, greenCards.Count)], indexesToChange[i]);
                        CardPlayedSuccesful = true;
                    }
                }
            }
            if (effect.SwitchToRed)
            {
                for (int i = 0; i < indexesToChange.Count; i++)
                {
                    if (CardsOnTable[indexesToChange[i]].card.cardColor == CardObject.CardColor.GREEN)
                    {
                        PlaceCardOnTable(redCards[Random.Range(0, redCards.Count)], indexesToChange[i]);
                        CardPlayedSuccesful = true;
                    }
                }
            }
            if (effect.SwitchToOpposite)
            {
                for (int i = 0; i < indexesToChange.Count; i++)
                {
                    if (CardsOnTable[indexesToChange[i]].card.cardColor == CardObject.CardColor.RED)
                    {
                        PlaceCardOnTable(greenCards[Random.Range(0, greenCards.Count)], indexesToChange[i]);
                        CardPlayedSuccesful = true;
                        continue;
                    }
                    if (CardsOnTable[indexesToChange[i]].card.cardColor == CardObject.CardColor.GREEN)
                    {
                        PlaceCardOnTable(redCards[Random.Range(0, redCards.Count)], indexesToChange[i]);
                        CardPlayedSuccesful = true;
                        continue;
                    }
                }
            }
            if (effect.Shuffle)
            {
                List<CardObject> unshuffled = new List<CardObject>();
                for(int i = 0; i < indexesToChange.Count; i++)
                {
                    unshuffled.Add(CardsOnTable[indexesToChange[i]].card);
                }
                List<CardObject> shuffled = new List<CardObject>();
                while(unshuffled.Count > 0)
                {
                    var indexToShuffle = Random.Range(0, unshuffled.Count);
                    shuffled.Add(unshuffled[indexToShuffle]);
                    unshuffled.RemoveAt(indexToShuffle);
                }
                for(int i = 0; i < shuffled.Count; i++)
                {
                    PlaceCardOnTable(shuffled[i], indexesToChange[i]);
                }
                CardPlayedSuccesful = true;
            }
            if (effect.ReplaceToTypesActive)
            {
                List<int> indexesOfReplacedType = new List<int>();
                List<CardObject> cardsOfReplacingType = new List<CardObject>();
                foreach(int index in indexesToChange)
                {
                    if(CardsOnTable[index].card.Type == effect.ReplaceType)
                    {
                        indexesOfReplacedType.Add(index);
                    }
                }
                foreach(CardObject card in TableDeck)
                {
                    if(card.Type == effect.ReplaceToType)
                    {
                        cardsOfReplacingType.Add(card);
                    }
                }
                if (indexesOfReplacedType.Count != 0)
                {
                    foreach (int index in indexesOfReplacedType)
                    {
                        var randomCard = cardsOfReplacingType[Random.Range(0, cardsOfReplacingType.Count)];
                        PlaceCardOnTable(randomCard, index);
                    }
                    CardPlayedSuccesful = true;
                }
            }
        }
        if (CardPlayedSuccesful)
        {
            RemoveCardFromHand(cardController);
        }
    }
}
