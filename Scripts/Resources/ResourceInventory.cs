using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02.Scripts.Resource
{
    public class ResourceInventory : MonoBehaviour
    {
        [SerializeField]
        private PlayerStat playerStat;
        private Dictionary<Item, int> resourceDict = new();
        public Dictionary<Item, int> GetDict()
        {
            return new Dictionary<Item, int>(resourceDict);
        }
        public void SetDict(Dictionary<Item, int> newDict)
        {
            resourceDict = newDict;
            OnInventoryChanged?.Invoke(resourceDict);  // UI 등 갱신
        }

        // Observer 이벤트 정의
        public event Action<Dictionary<Item, int>> OnInventoryChanged;

        public void Add(Item item, int amount)
        {
            if (!resourceDict.ContainsKey(item))
                resourceDict[item] = 0;

            resourceDict[item] += amount;

            // 이벤트 호출
            OnInventoryChanged?.Invoke(resourceDict);
        }

        public bool Consume(Item item, int amount)
        {
            if (!resourceDict.TryGetValue(item, out int current) || current < amount)
                return false;

            resourceDict[item] -= amount;

            // 이벤트 호출
            OnInventoryChanged?.Invoke(resourceDict);
            return true;
        }

        public void ClearAll()
        {
            resourceDict.Clear();
            OnInventoryChanged?.Invoke(resourceDict); // UI 갱신을 위해 이벤트 호출
        }

        public Item GetItemAtQuickSlot(int index)
        {
            List<Item> consumables = new List<Item>();
            foreach (var item in resourceDict.Keys)
            {
                if (item != null && item.isConsumable)
                {
                    consumables.Add(item);
                }
            }

            return index >= 0 && index < consumables.Count ? consumables[index] : null;
        }

        public int GetItemAmount(Item item)
        {
            resourceDict.TryGetValue(item, out int amount);
            return amount;
        }
        
        public void TryUseSlot(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            string rawInputName = context.control.name; // 예: "digit1", "digit2"
    
            // 정규식으로 숫자만 추출
            string digitsOnly = Regex.Match(rawInputName, @"\d+").Value;

            if (int.TryParse(digitsOnly, out int slotNumber))
            {
                slotNumber -= 1; // 배열은 0부터 시작하므로 -1
                Item item = GetItemAtQuickSlot(slotNumber);

                if (item != null && item.isConsumable)
                {
                    bool consumed = Consume(item, 1);
                    if (consumed)
                    {
                        foreach (var effect in item.effects)
                        {
                            switch (effect.type)
                            {
                                case EffectType.Heal:
                                    playerStat.Heal(effect.value);
                                    break;
                                case EffectType.Eat:
                                    playerStat.Eat(effect.value);
                                    break;
                                case EffectType.Hydrate:
                                    playerStat.Hydrate(effect.value);
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"슬롯 번호 파싱 실패: {rawInputName}");
            }
        }
    }
}