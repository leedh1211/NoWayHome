using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Resource
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
    public class Item : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;
        public bool isConsumable;
        public string description;
        public List<ItemEffect> effects;
    }
}