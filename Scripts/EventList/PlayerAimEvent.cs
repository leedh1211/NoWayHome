public struct PlayerAimEvent 
{ 
    public bool isAiming { get; private set; }

    public PlayerAimEvent(bool isAiming)
    {
        this.isAiming = isAiming;   
    }
}

public struct PlayerAimMonsterEvent
{
    public bool isAimingMonster { get; private set; }

    public PlayerAimMonsterEvent(bool isAimingMonster)
    {
        this.isAimingMonster = isAimingMonster;
    }
}
