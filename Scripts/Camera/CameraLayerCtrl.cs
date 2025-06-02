using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayerCtrl : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();   
    }

    private void OnEnable()
    {
        EventBus.Subscribe<CameraLayerAddEvent>(CameraLayerAddHandler);
        EventBus.Subscribe<CameraLayerRemoveEvent>(CameraLayerRemoveHandler);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<CameraLayerAddEvent>(CameraLayerAddHandler);
        EventBus.UnSubscribe<CameraLayerRemoveEvent>(CameraLayerRemoveHandler);
    }

    private void CameraLayerAddHandler(CameraLayerAddEvent evnt)
    {
        int layerIndex = LayerMask.NameToLayer(evnt.layerName);
        _camera.cullingMask |= (1 << layerIndex);
    }

    private void CameraLayerRemoveHandler(CameraLayerRemoveEvent evnt)
    {
        int layerIndex = LayerMask.NameToLayer(evnt.layerName);
        _camera.cullingMask &= ~(1 << layerIndex);
    }
}
