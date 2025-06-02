using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUIController : MonoBehaviour
{
    public GameObject interactKeyUI;

    private void OnEnable()
    {
        EventBus.Subscribe<InteractionEnterEvent>(OnInteractionEnter);
        EventBus.Subscribe<InteractionExitEvent>(OnInteractionExit);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<InteractionEnterEvent>(OnInteractionEnter);
        EventBus.UnSubscribe<InteractionExitEvent>(OnInteractionExit);
    }

    private void OnInteractionEnter(InteractionEnterEvent e)
    {
        interactKeyUI.SetActive(true);
    }

    private void OnInteractionExit(InteractionExitEvent e)
    {
        interactKeyUI.SetActive(false);
    }
}
