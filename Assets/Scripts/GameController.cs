using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    QuestController questController;
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
    public int CurrentQuestsGroup;

    public Vector3 PointerTarget;

    public CardUIController selectedCard;

    public GameObject ScreenBlocker;
    public TextMeshProUGUI WinLoseText;
    public Image WinLoseImg;
    public Sprite WinSprite;
    public Sprite LoseSprite;
    public GameObject MarkerParticles;

    public int DesiredNumberOfSteps;
    public float DistFromCenter;
    public GameObject CardPlacePrefab;

    void Start()
    {
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        CardsOnTable = new List<CardController>();
        playingField = GameObject.Find("Playing field");

        var cardPlace = CardPlacePrefab;

        for(int i = 0; i < DesiredNumberOfSteps; i++)
        {
            var place = Instantiate(cardPlace, playingField.transform);
            place.transform.localPosition = Quaternion.AngleAxis((360 / DesiredNumberOfSteps) * -1 * i, Vector3.forward) * transform.up * DistFromCenter;
        }
        //Destroy(cardPlace.gameObject);


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
            PlaceCardOnTable(TableDeck[Random.Range(0, TableDeck.Count)], i, true);
        }

        PointerTarget = transform.up;

        var arrayOfHandCards = Resources.LoadAll("ScriptableObjects/HandCards", typeof(HandCardObject));
        Debug.Log(arrayOfHandCards.Length);
        List<HandCardObject> ListInOrder = new List<HandCardObject>();
        for (int i = 0; i < arrayOfHandCards.Length; i++)
        {
            ListInOrder.Add((HandCardObject)arrayOfHandCards[i]);
        }
        while (ListInOrder.Count > 0)
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
                PlaceCardOnTable(TableDeck[Random.Range(0, TableDeck.Count)], i, true);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            DeselectCard();
        }
    }

    public void PlaceCardOnTable(CardObject card, int index, bool NowPlacingCards)
    {
        if (!NowPlacingCards)
        {
            CardsOnTable[index].SelectLightGameobject.gameObject.SetActive(true);
        }
        if (NowPlacingCards)
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
        var currentHour = StepIndex % DesiredNumberOfSteps;
        CurrentHour = currentHour;
        Debug.Log(currentHour);
        PointerTarget = Quaternion.AngleAxis((360 / DesiredNumberOfSteps) * -1 * currentHour, Vector3.forward) * transform.up;
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
        foreach (CardUIController card in CardsInHand)
        {
            card.RandomIndexesToChange.Clear();
        }
        DeselectCard();
        CheckQuests();
    }

    public void CheckQuests()  //œ–Œ¬≈– ¿  ¬≈—“Œ¬
    {
        

        // œ–Œ¬≈– ¿ ”—ÀŒ¬»…  ¬≈—“Œ¬
        if (questController.QuestPanels.Count != 0)
        {
            var placeholderForTableCards = new List<CardController>();
            placeholderForTableCards.AddRange(CardsOnTable);
            placeholderForTableCards.AddRange(CardsOnTable);
            /*for (int i = 0; i < placeholderForTableCards.Count; i++)
            {
                Debug.Log(placeholderForTableCards[i].card.cardColor);
            }*/

            foreach (QuestPanelController questPanel in questController.QuestPanels)
            {
                List<CardController> ParticleCards = new List<CardController>();
                if (!questPanel.quest.Completed)
                {
                    bool QuestWon = false;
                    bool QuestLost = false;
                    
                    
                    //Debug.Log(placeholderForTableCards.Count);
                    // ¬€»√–€ÿ

                    List<CardController> winCards = new List<CardController>();
                    CardController prevWinCard = null;
                    foreach (CardController cardController in CardsOnTable)
                    {
                        if (questPanel.quest.winMethod == QuestController.Quest.WinMethod.WIN_BY_QUANTITY) //—Œ–“»–”≈Ã œŒ  ŒÀ»◊≈—“¬” Õ¿ œŒÀ≈
                        {
                            if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_COLOR_WIN &&
                                cardController.card.cardColor == questPanel.quest.WinColor)
                            {
                                winCards.Add(cardController);
                            }
                            if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_TYPE_WIN &&
                                cardController.card.Type == questPanel.quest.WinningType)
                            {
                                winCards.Add(cardController);
                            }
                        }
                    }
                    
                    
                    foreach (CardController cardController in placeholderForTableCards)
                    {
                        if (questPanel.quest.winMethod == QuestController.Quest.WinMethod.WIN_BY_COMBINATION) //œŒ œŒ–ﬂƒ ”
                        {
                            if (winCards.Count >= questPanel.quest.CardQToWin)
                            {
                                continue;
                            }

                            if (prevWinCard != null)
                            {
                                if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_COLOR_WIN)  //œŒ ÷¬≈“”
                                {
                                    if (prevWinCard.card.cardColor == cardController.card.cardColor 
                                        && cardController.card.cardColor == questPanel.quest.WinColor)
                                    {
                                        winCards.Add(cardController);
                                        Debug.Log("AddedGreen");
                                    }
                                    else
                                    {
                                        if (winCards.Count > 0)
                                        {
                                            winCards.Clear();
                                            Debug.Log("ClearedGreen");
                                        }
                                        else if(winCards.Count <= 0 && cardController.card.cardColor == questPanel.quest.WinColor)
                                        {
                                            winCards.Add(cardController);
                                            Debug.Log("AddedGreen");
                                        }
                                    }
                                }
                                if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_TYPE_WIN)  //œŒ “»œ”
                                {
                                    if (prevWinCard.card.Type == cardController.card.Type &&
                                        cardController.card.Type == questPanel.quest.WinningType)
                                    {
                                        winCards.Add(cardController);
                                    }
                                    else
                                    {
                                        if (winCards.Count > 0)
                                        {
                                            winCards.Clear();
                                            Debug.Log("ClearedGreen");
                                        }
                                        else if (winCards.Count <= 0 && cardController.card.Type == questPanel.quest.WinningType)
                                        {
                                            winCards.Add(cardController);
                                            Debug.Log("AddedGreen");
                                        }
                                    }
                                }
                            }
                            if (prevWinCard == null)
                            {
                                if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_COLOR_WIN)
                                {
                                    if (cardController.card.cardColor == questPanel.quest.WinColor)
                                    {
                                        winCards.Add(cardController);
                                        Debug.Log("AddedGreenNull");

                                    }
                                    else
                                    {
                                        winCards.Clear();
                                        Debug.Log("ClearedGreenNull");

                                    }
                                }
                                if (questPanel.quest.winCondition == QuestController.Quest.WinCondition.SAME_TYPE_WIN)
                                {
                                    if (cardController.card.Type == questPanel.quest.WinningType)
                                    {
                                        winCards.Add(cardController);
                                    }
                                    else
                                    {
                                        winCards.Clear();
                                    }
                                }
                            }
                            //if (prevWinCard != null)
                                //Debug.Log(prevWinCard.card.cardColor + " " + cardController.card.cardColor);
                            prevWinCard = cardController;
                        }
                    }
                    if (winCards.Count >= questPanel.quest.CardQToWin)
                    {
                        QuestWon = true;
                        ParticleCards = winCards;
                    }


                    // œ–Œ»√–€ÿ
                    List<CardController> loseCards = new List<CardController>();
                    CardController prevCard = null;
                    foreach (CardController cardController in CardsOnTable)
                    {
                        if (questPanel.quest.loseMethod == QuestController.Quest.LoseMethod.LOSE_BY_QUANTITY) //—Œ–“»–”≈Ã œŒ  ŒÀ»◊≈—“¬” Õ¿ œŒÀ≈
                        {
                            if (questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_COLOR_LOSE &&
                                cardController.card.cardColor == questPanel.quest.LoseColor)
                            {
                                loseCards.Add(cardController);
                            }
                            if (questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_TYPE_LOSE
                                && cardController.card.Type == questPanel.quest.LosingType)
                            {
                                loseCards.Add(cardController);
                            }
                        }
                    }
                    foreach (CardController cardController in placeholderForTableCards)
                    {
                        if (questPanel.quest.loseMethod == QuestController.Quest.LoseMethod.LOSE_BY_COMBINATION) //œŒ œŒ–ﬂƒ ”
                        {
                            if (loseCards.Count >= questPanel.quest.CardQToLose)
                            {
                                continue;
                            }

                            if(prevCard != null)
                            {
                                if (questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_COLOR_LOSE)
                                {
                                    if (prevCard.card.cardColor == cardController.card.cardColor 
                                        && cardController.card.cardColor == questPanel.quest.LoseColor)
                                    {
                                        loseCards.Add(cardController);
                                    }
                                    else
                                    {
                                        //loseCards.Clear();
                                        if (loseCards.Count > 0)
                                        {
                                            loseCards.Clear();
                                            Debug.Log("ClearedGreen");
                                        }
                                        else if (loseCards.Count <= 0 && cardController.card.cardColor == questPanel.quest.WinColor)
                                        {
                                            loseCards.Add(cardController);
                                            Debug.Log("AddedGreen");
                                        }
                                        //Debug.Log("Cleared" + loseCards.Count);
                                    }
                                }
                                if(questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_TYPE_LOSE)
                                {
                                    if (prevCard.card.Type == cardController.card.Type
                                        && cardController.card.Type == questPanel.quest.LosingType)
                                    {
                                        loseCards.Add(cardController);
                                    }
                                    else
                                    {
                                        if (loseCards.Count > 0)
                                        {
                                            loseCards.Clear();
                                            Debug.Log("ClearedGreen");
                                        }
                                        else if (loseCards.Count <= 0 && cardController.card.cardColor == questPanel.quest.WinColor)
                                        {
                                            loseCards.Add(cardController);
                                            Debug.Log("AddedGreen");
                                        }
                                        //Debug.Log("Cleared" + loseCards.Count);
                                    }
                                }
                            }
                            if (prevCard == null)
                            {
                                if (questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_COLOR_LOSE)
                                {
                                    if (cardController.card.cardColor == questPanel.quest.LoseColor)
                                    {
                                        loseCards.Add(cardController);
                                    }
                                    else
                                    {
                                        loseCards.Clear();
                                        //Debug.Log("Cleared" + loseCards.Count);
                                    }
                                }
                                if (questPanel.quest.loseCondition == QuestController.Quest.LoseCondition.SAME_TYPE_LOSE)
                                {
                                    if (prevCard.card.Type == cardController.card.Type)
                                    {
                                        loseCards.Add(cardController);
                                    }
                                    else
                                    {
                                        loseCards.Clear();
                                        //Debug.Log("Cleared" + loseCards.Count);
                                    }
                                }
                            }
                            //if(prevCard!= null)
                                //Debug.Log(prevCard.card.cardColor + " " + cardController.card.cardColor);
                            prevCard = cardController;
                        }
                    }
                    if (loseCards.Count >= questPanel.quest.CardQToLose)
                    {
                        QuestLost = true;
                        ParticleCards = loseCards;
                    }

                    if (QuestWon && !QuestLost)
                    {
                        questPanel.quest.WinTurnTracker++;
                        if (questPanel.quest.WinTurnTracker > questPanel.quest.TurnQToWin) // ¬≈—“ «¿¬≈–ÿ®Õ
                        {
                            foreach (CardController card in ParticleCards)
                            {
                                Instantiate(MarkerParticles, card.transform.position, MarkerParticles.transform.rotation);
                            }
                            questPanel.quest.Completed = true;
                            questPanel.CompletionMarker.SetActive(true);
                        }
                    }
                    else if (!QuestWon)
                    {
                        questPanel.quest.WinTurnTracker = 0;
                    }
                    if (QuestLost && !QuestWon)
                    {
                        questPanel.quest.LoseTurnTracker++;
                        if (questPanel.quest.LoseTurnTracker > questPanel.quest.TurnQToLose)
                        {
                            foreach (CardController card in ParticleCards)
                            {
                                Instantiate(MarkerParticles, card.transform.position, MarkerParticles.transform.rotation);
                            }
                            LoseAllGame();
                        }
                    }
                    else if (!QuestLost)
                    {
                        questPanel.quest.LoseTurnTracker = 0;
                    }
                }
            }
        }

        bool AllQuestsCompleted = false;
        if (questController.QuestPanels.Count != 0)  //œ–Œ¬≈–»“‹, ¬—≈ À»  ¬≈—“€ ¬€œŒÀÕ≈Õ€
        {
            AllQuestsCompleted = true;
            foreach (QuestPanelController questPanel in questController.QuestPanels)
            {
                if (!questPanel.quest.Completed)
                {
                    AllQuestsCompleted = false;
                }
            }
            if (AllQuestsCompleted)
            {
                CurrentQuestsGroup++;
            }
        }
        if (AllQuestsCompleted || questController.QuestPanels.Count == 0)
        {
            SpawnQuests();
        }
    }
    public void SpawnQuests()
    {
        if (CurrentQuestsGroup < questController.QuestGroups.Count)  //≈—À» ≈Ÿ® Õ≈  ŒÕ≈÷ »√–€
        {
            if (questController.QuestPanels.Count != 0)  //œ–Œ¬≈–»“‹, ≈—“‹ À» œ¿Õ≈À»  ¬≈—“Œ¬ Õ¿ › –¿Õ≈ » ”Õ»◊“Œ∆»“‹
            {
                while (questController.QuestPanels.Count > 0)
                {
                    var holder = questController.QuestPanels[0];
                    questController.QuestPanels.Remove(holder);
                    holder.StartSelfDestruct();
                }
            }
            var questGroupToSpawn = questController.QuestGroups[CurrentQuestsGroup];   //¬€¡»–¿≈Ã √–”œœ”  ¬≈—“Œ¬ ƒÀﬂ —œ¿¬Õ¿
            if (questGroupToSpawn.addedCardToDeck.Count != 0)
            {
                TableDeck.AddRange(questGroupToSpawn.addedCardToDeck);
            }
            if (questGroupToSpawn.addedCardToHand.Count != 0)
            {
                HandDeck.AddRange(questGroupToSpawn.addedCardToHand);
            }
            for(int i = 0; i < questGroupToSpawn.Quests.Count; i++)
            {
                var spawned = Instantiate(questController.questPanelPrefab, 
                questController.QuestPanelScroll.position, 
                Quaternion.identity, 
                questController.QuestPanelScroll);
                var spawnedController = spawned.GetComponent<QuestPanelController>();
                spawnedController.quest = questGroupToSpawn.Quests[i];
                questController.QuestPanels.Add(spawnedController);
            }
        }
        else  // ŒÕ≈÷ »√–€
        {
            WinAllGame();
        }
    }

    public void LoseAllGame()
    {
        ScreenBlocker.SetActive(true);
        WinLoseText.text = "YOU LOST!";
        WinLoseImg.sprite = LoseSprite;
    }
    public void WinAllGame()
    {
        ScreenBlocker.SetActive(true);
        WinLoseText.text = "YOU WON!";
        WinLoseImg.sprite = WinSprite;

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
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
    public void PlaySelectedCard(CardUIController cardController, bool IsPlacingCardNow)
    {
        var handCard = cardController.handCard;
        bool CardPlayedSuccesful = false;
        selectedCard = cardController;
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
                    for (int i = 0; i < CardsOnTable.Count; i++)
                    {
                        indexesToChange.Add(i);
                    }
                    if (!effect.ChangeCurrentToo)
                    {
                        indexesToChange.Remove(CurrentHour);
                    }
                    break;
                case HandCardObject.AreaOfEffect.RANDOM_CARDS:
                    if (cardController.RandomIndexesToChange.Count == 0)
                    {
                        var TableCardsPlaceholder = new List<int>();//”¡≈∆ƒ¿≈Ã—ﬂ, ◊“Œ ¬€¡–¿ÕÕ€≈ –¿ÕƒŒÃÕ€≈  ¿–“€ Õ¿ —“ŒÀ≈ ¡”ƒ”“ œŒƒ’Œƒ»“‹ œŒƒ ”—ÀŒ¬»ﬂ (Á‡ÍÓÏÏÂÌ˜.)
                        for (int i = 0; i < CardsOnTable.Count; i++)
                        {
                            if (effect.SwitchToGreen && CardsOnTable[i].card.cardColor == CardObject.CardColor.RED)
                            {
                                TableCardsPlaceholder.Add(i);
                            }
                            if (effect.SwitchToRed && CardsOnTable[i].card.cardColor == CardObject.CardColor.GREEN)
                            {
                                TableCardsPlaceholder.Add(i);
                            }
                            if (effect.ReplaceToTypesActive && (CardsOnTable[i].card.Type == effect.ReplaceType || effect.ReplaceType == "ANY"))
                            {
                                TableCardsPlaceholder.Add(i);
                            }
                            if (effect.Shuffle || effect.SwitchToOpposite)
                            {
                                TableCardsPlaceholder.Add(i);
                            }
                        }
                        Debug.Log(TableCardsPlaceholder.Count + " random suitable cards found");
                        for (int i = 0; i < effect.QuantityOfEffect; i++)
                        {
                            if (TableCardsPlaceholder.Count > 0)
                            {
                                int randomPositionOfIndex = Random.Range(0, TableCardsPlaceholder.Count);
                                int randIndex = TableCardsPlaceholder[randomPositionOfIndex];
                                indexesToChange.Add(randIndex);
                                TableCardsPlaceholder.RemoveAt(randomPositionOfIndex);
                            }
                        }                  
                        cardController.RandomIndexesToChange = indexesToChange;
                    }
                    else
                    {
                        indexesToChange = cardController.RandomIndexesToChange;
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
                        PlaceCardOnTable(greenCards[Random.Range(0, greenCards.Count)], indexesToChange[i], IsPlacingCardNow);
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
                        PlaceCardOnTable(redCards[Random.Range(0, redCards.Count)], indexesToChange[i], IsPlacingCardNow);
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
                        PlaceCardOnTable(greenCards[Random.Range(0, greenCards.Count)], indexesToChange[i], IsPlacingCardNow);
                        CardPlayedSuccesful = true;
                        continue;
                    }
                    if (CardsOnTable[indexesToChange[i]].card.cardColor == CardObject.CardColor.GREEN)
                    {
                        PlaceCardOnTable(redCards[Random.Range(0, redCards.Count)], indexesToChange[i], IsPlacingCardNow);
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
                    PlaceCardOnTable(shuffled[i], indexesToChange[i], IsPlacingCardNow);
                }
                CardPlayedSuccesful = true;
            }
            if (effect.ReplaceToTypesActive)
            {
                List<int> indexesOfReplacedType = new List<int>();
                List<CardObject> cardsOfReplacingType = new List<CardObject>();
                foreach(int index in indexesToChange)
                {
                    if(CardsOnTable[index].card.Type == effect.ReplaceType || effect.ReplaceType == "ANY")
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
                        PlaceCardOnTable(randomCard, index, IsPlacingCardNow);
                    }
                    CardPlayedSuccesful = true;
                }
            }
        }
        if (CardPlayedSuccesful && IsPlacingCardNow)
        {
            RemoveCardFromHand(cardController);
        }
        else if(!CardPlayedSuccesful && IsPlacingCardNow)
        {
            cardController.JitterTimer = 0.5f;
        }
    }

    public void DeselectCard()
    {
        if (selectedCard != null && !selectedCard.MouseOverThisElement)
        {
            selectedCard.Pressed = false;
            selectedCard = null;
            foreach (CardController card in CardsOnTable)
            {
                if (card.SelectLightGameobject.activeInHierarchy)
                    card.SelectLightGameobject.SetActive(false);
            }
        }
    }
}
