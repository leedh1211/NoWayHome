public struct GameOverEvent
{
    public float LifeTime;
    public int CurrentKillCount;
    public int TotalSpawnCount;

    public GameOverEvent(float lifeTime, int currentKill, int totalSpawn)
    {
        LifeTime = lifeTime;
        CurrentKillCount = currentKill;
        TotalSpawnCount = totalSpawn;
    }
}