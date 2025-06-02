using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    [SerializeField] private float checkDistance = 5f;     //상호작용 가능 사거리 
    public LayerMask layerMask;

    public GameObject curInteractableObject;
    private IInteractable curInteractable;

    private Animator animator;
    private int animIDPickup = Animator.StringToHash("Pickup");
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, checkDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractableObject)
                {
                    curInteractableObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    if(curInteractable.GetInteractableType() != InteractableType.Harvest)
                    {   //광맥이 아닌 경우에만 E키 표시
                        EventBus.Raise(new InteractionEnterEvent(curInteractable));
                    }
                }
            }
            else
            {
                if (curInteractableObject != null)
                {
                    EventBus.Raise(new InteractionExitEvent(curInteractable));
                    curInteractableObject = null;
                    curInteractable = null;
                }

            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            switch (curInteractable.GetInteractableType()) 
            {
                case InteractableType.PickUp:
                    curInteractable.OnInteract(this);
                    animator.SetTrigger(animIDPickup);
                    curInteractableObject = null;
                    curInteractable = null;
                    EventBus.Raise(new InteractionExitEvent(curInteractable));
                    break;
                case InteractableType.Harvest:
                    break;
                case InteractableType.NPC:
                    curInteractable.OnInteract(this);
                    break;
                default:
                    break;
            }
        }
    }

 
   
    private void OnMining()  //채광 애니메이션 이벤트
    {
        if(curInteractable != null && 
            curInteractable.GetInteractableType() == InteractableType.Harvest)
        {
            curInteractable.OnInteract(this);
        }
    }

    public void PickUp()        //줍기 상호작용
    {
        animator.SetTrigger(animIDPickup);
        curInteractableObject = null;
        curInteractable = null;
    }

    public void Harvest()       //채집 상호작용(곡괭이질)
    {

    }
}