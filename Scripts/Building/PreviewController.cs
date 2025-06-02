using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    private Color _colorRed;
    private Color _colorGreen;
    private Color _color;
    public bool isInstallable = false;
    private int _triggerCount;
    private BuildingData data;
    
    private void Start()
    {
        _colorRed = new Color(1, 0, 0, 0.1f);
        _colorGreen = new Color(0,1,0,0.1f);
        
        data = BuildingManager.Instance.ui_building.selectedSlot.data;
        _color = _colorRed;
        if (data.buildingType == BuildingType.CreateInstallableField)
        {
            _color = _colorGreen;
            isInstallable = true;
        }
        ChangeAllMaterials();
    }
    
    public void ChangeAllMaterials()
    {
        // 부모와 모든 자식의 Renderer를 가져옴
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            // 여러 머티리얼이 할당된 경우 모두 교체
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                // 기존 오브젝트의 셰이더 렌더링 모드가 opaque이기 때문에 반투명한 머터리얼을 만들 수 없어 이를 수정
                mats[i].SetFloat("_Mode", 3);                                                         // 셰이더의 렌더링 모드를 Transparent (투명)으로 설정
                mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);           // 소스 알파 값을 기준으로 블렌딩할때 사용
                mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);   // 소스 알파 값의 반대 값을 기준으로 블렌딩할때 사용
                mats[i].SetInt("_ZWrite", 0);                                                         // 깊이 버퍼 사용 여부 X, 투명 렌더링 모드에서는 일반적으로 버퍼를 비활성화해야 정상적으로 작동
                mats[i].DisableKeyword("_ALPHATEST_ON");                                                   // 알파테스트 비활성화, 알파 테스트는 픽셀의 알파 값에 따라 픽셀을 완전히 그리거나 버리는 방식
                mats[i].EnableKeyword("_ALPHABLEND_ON");                                                   // 알파 블렌딩 활성화, 알파값에 기반해 투명효과를 구현
                mats[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");                                            // 알파 프리멀티플라이 비활성화, 알파 값이 이미 색상에 곱해진 상태로 처리
                mats[i].renderQueue = 3000;                                                                // 랜더 큐는 랜더링 순서 결정, Unity에서는 기본적으로 투명이면 3000을 사용(Oqaque(불투명)상태에서는 2000)
                mats[i].color = _color;
            }
            rend.materials = mats;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InstallableField") 
            && data.buildingType == BuildingType.InstallInField)
        {
            _triggerCount++;
        }
        
        if (_triggerCount > 0
            && data.buildingType == BuildingType.InstallInField)
        {
            isInstallable = true;
            _color = _colorGreen;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InstallableField")
            && data.buildingType == BuildingType.InstallInField)
        {
            _triggerCount--;
            _triggerCount = Mathf.Max(0, _triggerCount);
            
        }

        if (_triggerCount <= 0
            && data.buildingType == BuildingType.InstallInField)
        {
            isInstallable = false;
            _color = _colorRed;
        }
    }

}
