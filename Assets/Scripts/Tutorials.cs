using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    public bool enableEntergame = false;
    public bool enableStarAppear = false;
    public bool enableTouchStar = false;
    public bool enableMoonTip = false;

    public bool enterGame=false;
    public bool StarAppear=false;
    public bool touchStar = false;
    public bool MoonTip = false;
    public static Tutorials Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    private void Update()
    {
        if (enableEntergame&&!enterGame)
        {
            enterGame = true;
            GameController.Instance.ShowTipForSecond(0, 2f);
        }
        if (enableMoonTip&&!MoonTip)
        {
            MoonTip = true;
            GameController.Instance.ShowTipForSecond(4, 2f);
        }
        if (enableStarAppear&&!StarAppear)
        {
            GameObject[] gs = GameObject.FindGameObjectsWithTag("STAR");
            bool dis = false;
            for(int i = 0; i < gs.Length; i++)
            {
                Star s = gs[i].GetComponent<Star>();
                if (s == null) continue;
                if (s.haveBeenLighten == true)
                {
                    dis = true;
                    break;
                }
            }
            if (dis)
            {
                StarAppear = true;
                GameController.Instance.ShowTipForSecond(1, 2f);
            }
        }
        if (enableTouchStar&&!touchStar)
        {
            GameObject[] gs = GameObject.FindGameObjectsWithTag("STAR");
            bool dis = false;
            int x=-1, y = -1;
            for (int i = 0; i < gs.Length; i++)
            {
                Star s = gs[i].GetComponent<Star>();
                if (s == null) continue;
                if (s.haveBeenLighten == true)
                {
                    x = (int)gs[i].transform.position.x;
                    y = (int)gs[i].transform.position.z;
                    break;
                }
            }
            if (MapController.Instance.GetPlayerBlock(x-1, y) != null) dis = true;
            if (MapController.Instance.GetPlayerBlock(x+1, y) != null) dis = true;
            if (MapController.Instance.GetPlayerBlock(x, y+1) != null) dis = true;
            if (MapController.Instance.GetPlayerBlock(x, y-1) != null) dis = true;
            if (dis)
            {
                touchStar = true;
                GameController.Instance.ShowTipForSecond(2, 2f);
            }
        }
    }
}
