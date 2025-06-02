using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace _02.Scripts.Resource
{
    public class ResourceConverterNPC : MonoBehaviour
    {
        public List<TradeRecipe> recipes;

        public bool TryConvert(TradeRecipe recipe, ResourceInventory inventory)
        {
            bool result = false;
            foreach (var t in recipe.GetInputDict())
            {
                if (!inventory.Consume(t.Key, t.Value))
                {
                    return false;
                }
            }

            foreach (var r in recipe.GetOutputDict())
            {
                inventory.Add(r.Key, r.Value);    
            }
            return true;
        }
    }
}