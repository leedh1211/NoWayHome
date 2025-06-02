public struct GunReloadEvent
{
    public int maxBullet { get; private set; }          //탄창 크기
    public int currentBullet { get; private set; }      //탄창 내 총알 개수
    public int totalBullet { get; private set; }        //남은 총알 개수

    public GunReloadEvent(int maxBullet, int currentBullet, int totalBullet)
    {
        this.maxBullet = maxBullet;
        this.currentBullet = currentBullet;
        this.totalBullet = totalBullet;
    }
}