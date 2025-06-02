public struct  CameraLayerAddEvent  //카메라에 보고싶은 레이어를 추가하는 이벤트
{
    public string layerName { get; private set; }

    public CameraLayerAddEvent(string layerName)
    {
        this.layerName = layerName;
    }
}

public struct CameraLayerRemoveEvent
{
    public string layerName { get; private set; }

    public CameraLayerRemoveEvent(string layerName)
    {
        this.layerName = layerName;
    }
}