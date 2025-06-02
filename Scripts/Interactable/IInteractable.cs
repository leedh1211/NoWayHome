using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    InteractableType GetInteractableType();
    string GetInteractPrompt();     //상호작용 범위에 들어올 시 보여주는 텍스트 (E: 줍기)
    void OnInteract(PlayerInteract player);     // 상호작용 로직
}


