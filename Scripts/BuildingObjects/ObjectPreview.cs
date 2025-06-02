using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPreview : MonoBehaviour
{
    [SerializeField] private LayerMask _buildingLayers;
    [SerializeField] private float _buildingLength = 20f;
    private GameObject _buildingPrefab;  // 실제로 설치할 오브젝트 프리팹, Building_UI에서 정보를 받아옴
    private GameObject _preview;       // Preview 프리팹
    private Vector3 _scrollPoint;        // 스크롤로 이동한 위치
    private Vector3 PreviewPoint;        // 해당 위치에 프리팹 생성, 나중에 오브젝트 생성될 위치

    private GameObject _currentPreview;
    private PreviewController _previewController;
    private float _playerY;
    
    private void Update()
    {
        if (BuildingManager.Instance == null)
        {
            // 아직 BuildingManager가 생성되지 않았을 때
            return;
        }
        
        if (BuildingManager.Instance.showPreview)
        {
            // playerInput 좌클릭의 상태 변환, Normal -> Building
            BuildingManager.Instance.playercontroller.SwitchleftInputBuilding();
            
            var uiBuilding = BuildingManager.Instance.ui_building;
            
            if (uiBuilding == null)
            {
                Debug.Log("uiBuilding이 null 입니다");
            } 
            else if (uiBuilding.selectedSlot == null)
            {
                Debug.Log("uiBuilding.selectedSlot이 null 입니다");
            }
            else if (uiBuilding.selectedSlot.data == null)
            {
                Debug.Log("uiBuilding.selectedSlot.data가 null 입니다");
            }
            

            _buildingPrefab = uiBuilding.selectedSlot.data.prefab;
            _preview = uiBuilding.selectedSlot.data.preview;
            
            // 플레이어 시점에서 Ray를 쏘아 닿는 지점에 프리뷰 생성 (일정 거리 이상 설치 불가)
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _buildingLength, _buildingLayers))
            {
                Vector3 hitPoint = hit.point;

                float _scrollPointY = BuildingManager.Instance.playercontroller.scrollValue;
                _playerY = Camera.main.transform.eulerAngles.y;
                
                _scrollPoint = new Vector3(0, _scrollPointY, 0);
                
                // 스크롤을 올리고나 내림으로써 preview 위치 조정
                PreviewPoint = hitPoint + _scrollPoint;
                
                // 프리뷰가 없다면 생성, 있다면 위치만 이동
                if (_currentPreview == null)
                {
                    _currentPreview = Instantiate(_preview, PreviewPoint, Quaternion.Euler(0, _playerY, 0));
                    _previewController = _currentPreview.GetComponent<PreviewController>();
                }
                else
                {
                    _currentPreview.transform.position = PreviewPoint;
                    _currentPreview.transform.rotation = Quaternion.Euler(0, _playerY, 0);
                    _previewController.ChangeAllMaterials();
                }
            }
        }
        else
        {
            // Ray가 Building 오브젝트에 닿지 않을 경우 프리뷰 제거
            if (_currentPreview != null)
            {
                Destroy(_currentPreview);
                _currentPreview = null;
                _previewController = null;
            }
        }
    }
    
    // 프리뷰 생성
    
    
    // 설치하고자 하는 오브젝트 프리팹 생성
    public void InstallObject()
    {
        if (_currentPreview != null && _previewController.isInstallable)
        {
            // 설치하려는 건물 오브젝트 설치
            _playerY = Camera.main.transform.eulerAngles.y;
            Instantiate(_buildingPrefab, PreviewPoint, Quaternion.Euler(0, _playerY, 0));
            OffPreview();
        }
        else
        {
            Debug.Log("설치 불가");
        }
    }

    public void OffPreview()
    {
        // preview 오브젝트 파괴
        Destroy(_currentPreview);
        // preview 끄기
        BuildingManager.Instance.showPreview = false;
        BuildingManager.Instance.playercontroller.SwitchleftInputNormal();
    }
    
    
}
