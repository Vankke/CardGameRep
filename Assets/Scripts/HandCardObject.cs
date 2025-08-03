using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class HandCardObject : ScriptableObject
{
    public enum AreaOfEffect { NEXT_ADJACENT,
                               PREVIOUS_ADJACENT, 
                               ALL_ADJACENT,
                               ONLY_CURRENT,
                               RANDOM_CARDS,
                               ALL_CARDS };

    public CardObject.CardColor cardColor;

    [System.Serializable]
    public struct EffectOfCard
    {
        public AreaOfEffect areaOfEffect;
        public bool SwitchToRed;
        //public int QuantityToSwitchToRed;
        public bool SwitchToGreen;
       // public int QuantityToSwitchToGreen;
        public bool SwitchToOpposite;
       // public bool QuantityToSwitchToOpposite;
        public bool Shuffle;
        //public int QuantityToShuffle;
        public bool ReplaceToTypesActive;
        //public int RandomQuantity;
        public bool ChangeCurrentToo;
        public int QuantityOfEffect;
        public string ReplaceType;
        public string ReplaceToType;
        public List<int> RandomIndexesToChange;
    }

    public List<EffectOfCard> effects;
    public GameObject prefab;
    public Sprite sprite;
    public string Description;
}
