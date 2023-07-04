using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapController : MonoBehaviour
{
    const int stageSize = 100;
    public static MapController Instance;
    public List<GameObject> stageList;
    public int currentStageNumber=-1;
    GameObject currentStage;
    
    //地形中心，表示玩家的出生点位置
    int[,] mapSpawn = new int[stageSize,stageSize];
    //录入地图信息，第一维是关卡，第二维是竖向，第三维是横向，第四维是信息类型
    //0是是否亮，1是是否可以通行，2是是否是终点
    int[,,,] mapState = new int[stageSize, stageSize, stageSize,3];
    //每个关卡摄像机的位置
    public List<Vector3> camPos;
    void BuildStage(int stage,string str)
    {
        /*将关卡构造的字符串转换为实际的场地信息，我们规定0为空地，1为墙壁，2为终点
          例如示意图中的地图用字符串表示为
          "
          111100002222\n
          11110111\n
          00000111\n
          00000000\n
          00000000\n
          00000000\n
          00000000\n
          00000000\n
          "
         */
        int idx = 0;
        while (str.Length > idx)
        {
            for (int i = 0; i < stageSize&&idx<str.Length; i++)
            {
                for (int j = 0; j < stageSize&&idx<str.Length; j++)
                {
                    char c = str[idx];
                    idx++;
                    if (c == '\n')
                    {
                        break;
                    }
                    if (c == '0')//空地
                    {
                        mapState[stage, i, j, 0] = 0;
                        mapState[stage, i, j, 1] = 1;
                        mapState[stage, i, j, 2] = 0;
                    }else if (c == '1')//墙壁
                    {
                        mapState[stage, i, j, 0] = 0;
                        mapState[stage, i, j, 1] = 0;
                        mapState[stage, i, j, 2] = 0;
                    }else if (c == '2')//终点
                    {
                        mapState[stage, i, j, 0] = 0;
                        mapState[stage, i, j, 1] = 1;
                        mapState[stage, i, j, 2] = 1;
                    }
                }
            }
            
        }
    }
    private void Awake()
    {
        if(Instance != null)Destroy(this);
        Instance = this;
        //初始化，写入关卡
        for(int i = 0; i < stageSize; i++)
        {
            for(int j = 0; j < stageSize; j++)
            {
                for(int k = 0; k < stageSize; k++)
                {
                    //默认值为没有地面
                    //默认为不亮，不可通行，不是终点
                    mapState[i, j, k, 0] = 0;
                    mapState[i, j, k, 1] = 0;
                    mapState[i, j, k, 2] = 0;

                }
            }
        }
        //写入关卡
        //第0关
        BuildStage(0,   "11111111001\n" +
                        "00000000001\n" +
                        "00000000001\n" +
                        "11111111001\n" +
                        "11111111001\n" +
                        "11122000001\n" +
                        "11100000001\n"+
                        "11111111111");

        BuildStage(1,   "1111122221\n" +
                        "0000001111\n" +
                        "0000000001\n" +
                        "0000000001\n" +
                        "0111111111\n" +
                        "0000000001\n" +
                        "0000000001\n" +
                        "0000000001\n" +
                        "0000001101\n" +
                        "0000001101\n" +
                        "00000001101\n" +
                        "11111111111");

        BuildStage(3,   "0010010022221\n" +
                        "0010100011111\n" +
                        "0000000011111\n" +
                        "1000001111111\n" +
                        "1111111111111\n");

        BuildStage(2,   "000022221\n" +
                        "000011111\n" +
                        "000011111\n" +
                        "000011111\n" +
                        "1111111111111\n");
        BuildStage(4,   "1000000022221\n" +
                        "1011000011111\n" +
                        "0000100011111\n" +
                        "0000001011111\n" +
                        "0110001011111\n"+
                        "0010101011111\n"+
                        "1010101011111\n"+
                        "1000100011111");
    }
    /// <summary>
    /// 清除当前已经加载的关卡
    /// </summary>
    public void ClearStage()
    {
        if (currentStage != null)
        {
            currentStageNumber = -1;
            Destroy(currentStage);
            currentStage = null;
        }
    }
    /// <summary>
    /// 载入第x个关卡。
    /// </summary>
    /// <param name="x"></param>
    public void SpawnStage(int x)
    {
        ClearStage();
        GC.Collect();
        Camera.main.transform.position = camPos[x];
        
        //疑点一：重开要点两下
        currentStage = Instantiate(stageList[x]);
      //  Debug.Log("stage " + currentStage.transform.position);

        currentStageNumber = x;
        Invoke("RefreshStageLight", 0.1f);
    }

    public void Restart()
    {

        GameController.Instance.StopAllCoroutines();
        Time.timeScale = 1;
        SpawnStage(currentStageNumber);
    }
    /// <summary>
    /// 判断连通性，前方必须是没有collider的空地
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CanPass(int x, int y)
    {
        if (currentStageNumber == -1) return false;
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return false;
        if (GetMoonBlock(x,y)!=null) return false;
        if (GetStarBlock(x, y) != null ) return false;
        if (mapState[currentStageNumber, x, y,1] == 1) return true;
        else return false;
    }
    /// <summary>
    /// 能不能推
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CanPush(int x, int y)
    {
        if(CanPass(x, y)) return true;
        Star s = GetStarBlock(x, y);
        if(s!= null)
        {
            if (s.haveBeenLighten) return true;
        }
        Moon m = GetMoonBlock(x, y);
        if (m != null) return true;
        return false;
    }
    /// <summary>
    /// 获取当前格子里的角色
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Character GetCharacter(int x, int y)
    {
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return null;
        Collider[] cols = Physics.OverlapBox(new Vector3(x, 0, y), new Vector3(0.5f, 100, 0.5f));
        Character ret = null;
        for (int i = 0; i < cols.Length; i++)
        {
            Character m = cols[i].gameObject.GetComponent<Character>();
            if (m != null) ret = m;
        }
        return ret;
    }
    /// <summary>
    /// 工具函数，获取当前点的星星、月亮碰撞体
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Character GetPointColliderValue(int x,int y)
    {
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return null;
        Moon m = GetMoonBlock(x, y);
        if (m != null) return m;
        Star s = GetStarBlock(x, y);
        if (s != null) return s;
        return null;
    }
    /// <summary>
    /// 获取当前格子的月亮
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Moon GetMoonBlock(int x,int y)
    {
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return null;
        Collider[] cols = Physics.OverlapBox(new Vector3(x, 0, y), new Vector3(0.5f, 100, 0.5f));
        Moon ret = null;
        for(int i = 0; i < cols.Length; i++)
        {
            Moon m = cols[i].gameObject.GetComponent<Moon>();
            if (m != null) ret = m;
        }
        return ret;
    }
    /// <summary>
    /// 获取当前格子的玩家
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public PC GetPlayerBlock(int x, int y)
    {
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return null;
        Collider[] cols = Physics.OverlapBox(new Vector3(x, 0, y), new Vector3(0.5f, 100, 0.5f));
        PC ret = null;
        for (int i = 0; i < cols.Length; i++)
        {
            PC m = cols[i].gameObject.GetComponent<PC>();
            if (m != null) ret = m;
        }
        return ret;
    }
    /// <summary>
    /// 获取当前格子的月亮等级
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetMoonBlockLevel(int x, int y)
    {
        if (x < 0 || x >= stageSize || y < 0 || y >= stageSize) return 0;
        Moon m = GetMoonBlock(x, y);
        if (m == null) return 0;
        return m.level;
    }
    /// <summary>
    /// 获取当前格子的星星
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Star GetStarBlock(int x, int y)
    {
        Collider[] cols = Physics.OverlapBox(new Vector3(x, 0, y), new Vector3(0.5f, 100, 0.5f));
        Star ret = null;
        for(int i = 0; i < cols.Length; i++)
        {
            Star s = cols[i].GetComponent<Star>();
            if (s == null) continue;
            if (s.haveBeenLighten == false)
            {
                if (ret == null) ret = s;
            }
            else
            {
                ret = s;
            }
        }
        return ret;
    }
    /// <summary>
    /// 更新地图的光照情况
    /// </summary>
    public void RefreshStageLight()
    {
        if (currentStageNumber == -1) return;
        for(int i = 0; i < stageSize; i++)
        {
            for(int j = 0; j < stageSize; j++)
            {
                Star m = GetStarBlock(i, j);
                if(m != null)
                    m.UpdateStarState();
            }
        }
        CheckWin();
    }
    /// <summary>
    /// 绘制坐标轴，红色为x轴，蓝色为y轴
    /// </summary>
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    for(int i = 0; i < stageSize; i++)
    //    {
    //        for(int j = 0; j < stageSize; j++)
    //        {
    //            if (currentStageNumber != -1) {
    //                if (mapState[currentStageNumber, i, j, 1] == 0)
    //                {
    //                    Gizmos.color = Color.yellow;
    //                }
    //                else
    //                {
    //                    Gizmos.color = Color.green;
    //                }
    //            }
                
    //            Gizmos.DrawWireCube(new Vector3(i, 0, j), new Vector3(1f, 0.1f, 1f));
    //        }
    //    }
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(Vector3.zero, new Vector3(100, 0, 0));
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(Vector3.zero, new Vector3(0, 0, 100));
    //}
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    public void CheckWin()
    {
        bool win = true;
        for (int i = 0; i < stageSize; i++)
        {
            for (int j = 0; j < stageSize; j++)
            {
                if (mapState[currentStageNumber, i, j, 2] == 1)
                {
                    Star s = GetStarBlock(i, j);
                    bool b1 = true;
                    if (s == null)
                    {
                        b1 = false;
                    }
                    if (s!=null&&s.haveBeenLighten == false)
                    {
                        b1 = false;
                    }
                    PC p = GetPlayerBlock(i, j);
                    bool b2 = true;
                    if (p == null) b2 = false;
                    if (!(b1 || b2))
                    {
                        win = false;
                        break;
                    }
                }
            }
        }
        if (win)
        {
            GameController.Instance.Win();
        }
    }
}
