public struct WeaponUpgradeEvent
{ 
    public GunData RifleData { get; private set; }      //라이플
    public GunData MachineData { get; private set; }    //머신건

    public WeaponUpgradeEvent(GunData rifleData, GunData machineData)
    {
        RifleData = rifleData;
        MachineData = machineData;
    }
}

public struct ShipUpgradeEvent
{
    public BuildingData ShipData { get; private set; }

    public ShipUpgradeEvent(BuildingData shipData)
    {
       ShipData = shipData;
    }
}
