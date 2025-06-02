public struct PlayerConditionChangedEvent
{
    public PlayerConditionType ConditionType { get; private set; }
    public float CurCondition { get; private set; }
    public float MaxCondition { get; private set;}

    public PlayerConditionChangedEvent(PlayerConditionType type, float current, float max)
    {
        ConditionType = type;
        CurCondition = current;
        MaxCondition = max;
    }
}
