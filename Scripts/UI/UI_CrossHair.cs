using UnityEngine;
using UnityEngine.UI;

public class UI_CrossHair : MonoBehaviour
{
    private Animator _animator;
    private Image _image;
    private int _animIDZoom = Animator.StringToHash("Zoom");

    private async void Start()
    {
        if(!TryGetComponent<Animator>(out _animator))
        {
            Debug.LogWarning(this.name + " : Cannot Find Animator Component");
            this.enabled = false;
        }
        if(!TryGetComponent<Image>(out _image))
        {
            Debug.LogWarning(this.name + " : Cannot Find Image Component");
            this.enabled = false;
        }
    }
    private void OnEnable()
    {
        EventBus.Subscribe<PlayerAimEvent>(PlayerAimEventHandler);
        EventBus.Subscribe<PlayerAimMonsterEvent>(PlayerAimMonsterEventHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<PlayerAimEvent>(PlayerAimEventHandler);
        EventBus.UnSubscribe<PlayerAimMonsterEvent>(PlayerAimMonsterEventHandler);
    }

    private void PlayerAimEventHandler(PlayerAimEvent evnt)
    {
        //플레이어가 조준 시 크로스헤어도 Zoom In
        _animator.SetBool(_animIDZoom, evnt.isAiming);
        if (!evnt.isAiming)
        {
            _image.color = Color.white;
        }
    }

    private void PlayerAimMonsterEventHandler(PlayerAimMonsterEvent evnt)
    {
        //플레이어가 몬스터 조준 시 크로스헤어 빨간색으로
        _image.color =  evnt.isAimingMonster ? Color.red : Color.white;
    }
}
