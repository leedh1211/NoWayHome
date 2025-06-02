using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 상호작용 범위 진입 이벤트 데이터
public struct InteractionEnterEvent
{
    public IInteractable Target { get; private set; }

    public InteractionEnterEvent(IInteractable interactable)
    {
        Target = interactable;
    }
}

// 2. 실제 상호작용 발생 이벤트 데이터
public struct InteractionPerformEvent
{
    public IInteractable Target { get; private set; }

    public InteractionPerformEvent(IInteractable interactable)
    {
        Target = interactable;
    }
}

// 3. 상호작용 범위 이탈 이벤트 데이터
public struct InteractionExitEvent
{
    public IInteractable Target { get; private set; }

    public InteractionExitEvent(IInteractable interactable)
    {
        Target = interactable;
    }
}