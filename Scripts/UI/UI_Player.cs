using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    public UI_Condition health;
    public UI_Condition water;
    public UI_Condition hunger;

    private void Update()
    {
        //if (hunger.curValue <= 0 || water.curHp <= 0)
        //{
        //    if (health.curHp <= 0)
        //    {
        //        Time.timeScale = 0.0f;
        //    }
        //    health.Minus(Time.deltaTime);
        //    return;
        //}
        //water.Minus(Time.deltaTime * 0.5f);
        //hunger.Minus(Time.deltaTime * 0.5f);
    }
}
