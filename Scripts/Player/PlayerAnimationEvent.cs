using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerAnimationEvent : MonoBehaviour
{
    [Header("SFX 설정")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    public AudioClip RollAudioClip;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    CharacterController _controller;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    private void FootStepSound(AnimationEvent animationEvent)   //발소리 애니메이션 이벤트
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                SoundManager.Instance.PlaySFX(FootstepAudioClips[index], transform.TransformPoint(_controller.center));
            }
        }
    }

    private void LandingSound(AnimationEvent animationEvent)   //착지 소리 애니메이션 이벤트
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            SoundManager.Instance.PlaySFX(LandingAudioClip, transform.TransformPoint(_controller.center));
        }
    }

    private void RollSound(AnimationEvent animationEvent)
    {
        SoundManager.Instance.PlaySFX(RollAudioClip, transform.TransformPoint(_controller.center));
    }
}
