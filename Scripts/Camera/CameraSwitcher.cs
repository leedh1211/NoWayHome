using UnityEngine;
using Cinemachine; 

public class CameraSwitcher : Singleton<CameraSwitcher>
{
    private Camera mainCam;
    private CinemachineBrain cinemachineBrain;


    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera thirdPersonVCam; // 3인칭 VCam
    public CinemachineVirtualCamera lobbyCam; // 3인칭 VCam

    [Header("Culling Masks")]
    public LayerMask thirdPersonCullingMask; // 3인칭일 때 적용할 컬링 마스크
    public LayerMask lobbyCullingMask;
    
    void Start()
    {
        mainCam = Camera.main;
        cinemachineBrain = mainCam.GetComponent<CinemachineBrain>();
        SwitchToLobby();
        EventBus.Subscribe<StartGame>(HandleGameStart);
    }
    
    private void HandleGameStart(StartGame _)
    {
        Debug.Log("Game Started");
        SwitchToThirdPerson();
        thirdPersonVCam.Follow = GameObject.FindGameObjectWithTag("CinemachineTarget")?.transform;
    }

    void SwitchToThirdPerson()
    {
        thirdPersonVCam.gameObject.SetActive(true);
        lobbyCam.gameObject.SetActive(false);
        mainCam.cullingMask = thirdPersonCullingMask;
    }

    void SwitchToLobby()
    {
        lobbyCam.gameObject.SetActive(true);
        thirdPersonVCam.gameObject.SetActive(false);
        mainCam.cullingMask = lobbyCullingMask;
    }
    
    // 카메라 컬링 마스크에 새로운 레이어 추가
    public void AddLayerToCullingMask(string layerName)
    {
        thirdPersonCullingMask |= (1 << LayerMask.NameToLayer(layerName));

        UpdateCurrentCullingMask();
    }
    
    // 카메라 컬링 마스크에 있던 기존 레이어 제거
    public void RemoveLayerFromCullingMask(string layerName)
    {
        thirdPersonCullingMask &= ~(1 << LayerMask.NameToLayer(layerName));

        UpdateCurrentCullingMask();
    }
    
    // 현재 카메라에 레이어 적용
    private void UpdateCurrentCullingMask()
    {
        mainCam.cullingMask = thirdPersonCullingMask;
    }
}