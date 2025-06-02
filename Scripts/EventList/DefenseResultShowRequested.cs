public struct DefenseResultShowRequested
{
    public int StageIndex;
    public int CurrentKillCount;
    public int TotalSpawnCount;

    public DefenseResultShowRequested(int stageIndex, int currentKill, int totalSpawn)
    {
        StageIndex = stageIndex;
        CurrentKillCount = currentKill;
        TotalSpawnCount = totalSpawn;
    }
}