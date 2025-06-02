using System;

public static class ShipEvents
{
    public static event Action<float, float> OnHpChanged; // currentHp, maxHp

    public static void InvokeHpChanged(float currentHp, float maxHp)
    {
        OnHpChanged?.Invoke(currentHp, maxHp);
    }
}
