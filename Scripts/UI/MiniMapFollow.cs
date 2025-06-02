using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    private Transform player;
    public Vector3 offset;

    private float min = -100f;
    private float max = 100f;

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        newPos.y = transform.position.y; // 고정된 높이 유지
        newPos.x = Mathf.Clamp(newPos.x, min, max);
        newPos.z = Mathf.Clamp(newPos.z, min, max);
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, -player.eulerAngles.y, 0f); // 플레이어 방향에 따라 회전
    }
}
