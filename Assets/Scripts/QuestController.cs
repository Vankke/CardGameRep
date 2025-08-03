using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [System.Serializable]
    public struct Quest
    {
        public enum WinMethod
        {
            WIN_BY_QUANTITY,
            WIN_BY_COMBINATION
        };
        public enum WinCondition
        {
            SAME_COLOR_WIN,
            SAME_TYPE_WIN
        };
        public enum LoseMethod
        {
            LOSE_BY_QUANTITY,
            LOSE_BY_COMBINATION,
        };
        public enum LoseCondition
        {
            SAME_COLOR_LOSE,
            SAME_TYPE_LOSE
        };

        public Sprite sprite;
        public string Description;

        public WinMethod winMethod;
        public WinCondition winCondition;
        public LoseMethod loseMethod;
        public LoseCondition loseCondition;

        //public bool InstantWin;
        //public bool InstantLose;

        public int TurnQToWin;
        public int TurnQToLose;

        public string WinningType;
        public CardObject.CardColor WinColor;

        public string LosingType;
        public CardObject.CardColor LoseColor;

        public int CardQToWin;
        public int CardQToLose;

        public bool Completed;

        public int WinTurnTracker;
        public int LoseTurnTracker;
        
    }

    [System.Serializable]
    public struct QuestGroup
    {
        public List<Quest> Quests;
        public List<CardObject> addedCardToDeck;
        public List<HandCardObject> addedCardToHand;
    }

    public List<QuestGroup> QuestGroups;
    public GameObject questPanelPrefab;
    public RectTransform QuestPanelScroll;
    public List<QuestPanelController> QuestPanels;

}
