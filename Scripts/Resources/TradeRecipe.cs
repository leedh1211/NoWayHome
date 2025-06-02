using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Resource
{
    [CreateAssetMenu(fileName = "TradeRecipe", menuName = "Item/TradeRecipe")]
    public class TradeRecipe : ScriptableObject
    {
        public ResourceType resourceType;

        public List<TradeEntry> inputRecipe = new();
        public List<TradeEntry> outputRecipe = new();

        public Dictionary<Item, int> GetInputDict()
        {
            Dictionary<Item, int> dict = new();
            foreach (var entry in inputRecipe)
            {
                if (entry.item != null)
                    dict[entry.item] = entry.amount;
            }
            return dict;
        }

        public Dictionary<Item, int> GetOutputDict()
        {
            Dictionary<Item, int> dict = new();
            foreach (var entry in outputRecipe)
            {
                if (entry.item != null)
                    dict[entry.item] = entry.amount;
            }
            return dict;
        }
    }
}