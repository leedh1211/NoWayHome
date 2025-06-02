using System.Collections;
using System.Collections.Generic;
using _02.Scripts;
using UnityEngine;

public class Activator : MonoBehaviour,  IInstallable, IDamagable
{
    [Header("Installable Field")]
    public GameObject Installablefield;
    private GameObject _currentInstallablefield;
    [SerializeField] private BuildingData data;
    [SerializeField] private float _size;             // 오브젝트 크기
    [SerializeField] private float _ratSpeed;         // 회전 속도
    [SerializeField] private float _yRange = 0.5f; // 위아래 이동 범위
    [SerializeField] private float _ySpeed = 1f;   // 위아래 이동 속도
    private Vector3 _startPos;
    private Vector3 _installableFieldPos;

    public void Start()
    {
        CreateInstallableField();
        _startPos = transform.position;
        if (_currentInstallablefield != null)
            _installableFieldPos = _currentInstallablefield.transform.position;
    }
    public void Update() 
    {
        transform.Rotate(new Vector3(0,_ratSpeed * Time.deltaTime,0));
        float newY = _startPos.y + Mathf.Sin(Time.time * _ySpeed) * _yRange; // 삼각함수를 이용해 자연스러움 움직임 구현
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);
        if (_currentInstallablefield != null)
            _currentInstallablefield.transform.position = _installableFieldPos;
        
    }
    
    public void CreateInstallableField()
    {
        if (_currentInstallablefield != null) return;
        
        _currentInstallablefield = Instantiate(Installablefield, transform.position, Quaternion.identity, transform);
        _currentInstallablefield.transform.localScale = Vector3.one * _size;
        
    }
    
    public void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
    {
        data.currentHp = Mathf.Clamp(data.currentHp - damage, 0, data.maxHp);

        if (data.currentHp == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    
}
