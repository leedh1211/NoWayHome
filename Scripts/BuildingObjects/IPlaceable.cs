using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaceable
{
    // installableField 내부에 있는지 없는지 판별하여 내부에 있다면 설치를 허용
    bool IsPlaceable(Vector3 position, GameObject installableField);
}
