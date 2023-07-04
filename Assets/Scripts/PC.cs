using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : Character
{
    public KeyCode moveNorth = KeyCode.W;
    public KeyCode moveSouth = KeyCode.S;
    public KeyCode moveEast = KeyCode.D;
    public KeyCode moveWest = KeyCode.A;
    public float movementCD=0.45f;
    public bool lookAtRight = true;
    float currentCD = 0f;

    

    // Update is called once per frame
   
    void TurnLeft()
    {
        if (lookAtRight)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(30, 90, 180));
            //transform.Rotate(Vector3.right, 180f);
        }
        lookAtRight = false;
    }
    void TurnRight()
    {
        if (!lookAtRight)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(30, 90, 0));
        }
        lookAtRight = true;
    }
    private void FixedUpdate()
    {
        if(currentCD>0)currentCD-=Time.fixedDeltaTime;
    }
    private void Update()
    {
        if (currentCD <= 0&&GameController.Instance.AllowOperation)
        {
            Vector3 dir = Vector3.zero;
            if (Input.GetKeyDown(moveNorth))
            {
                dir.x = -1;
            }
            else if(Input.GetKeyDown(moveSouth))
            {
                dir.x = 1;
            }
            else if(Input.GetKeyDown(moveWest))
            {
                TurnLeft();
                dir.z = -1;
            }
            else if (Input.GetKeyDown(moveEast))
            {
                TurnRight();
                dir.z = 1;
            }
            if (dir == Vector3.zero) return;
            int tx = x + (int)dir.x;
            int ty=y + (int)dir.z;
            TryMoveTo(tx, ty, dir);
            currentCD = movementCD;
        }   
    }
    bool DFS(int x,int y,Vector3 direction)
    {
        bool flg = false;
        int nx = x - (int)direction.x;
        int ny= y - (int)direction.z;
        Character ch = MapController.Instance.GetCharacter(nx, ny);
        if (ch == null)
        {
            Debug.Log("true at "+nx+","+ny);
            return true;
        }
        Debug.Log("here's" + ch.name);
        if (MapController.Instance.CanPush(x, y))
        {
            flg = DFS(x + (int)direction.x, y + (int)direction.z, direction);
            if (flg)
            {
                ch.MoveTo(x, y,direction);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 玩家试图朝该地方移动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void TryMoveTo(int x,int y,Vector3 direction)
    {
        DFS(x,y,direction);
        //if (MapController.Instance.CanPass(x, y) == true)
        //{
        //    MoveTo(x, y,direction);
        //}
        //else
        //{
        //    Debug.Log("stage 2");
        //    int a = 1;
        //    Stack<Character> cs=new Stack<Character>();
        //    int xx = this.x+(int)direction.x;
        //    int yy = this.y+(int)direction.z;
        //    if (MapController.Instance.CanPush(xx, yy) == true)
        //        cs.Push(this);
        //    while (a>0)
        //    {
        //        a++;
        //        if (a == 10) break;
        //        Character chr = MapController.Instance.GetPointColliderValue(xx, yy);
        //        if (chr == null) break;
        //        if (chr is Star && (chr as Star).haveBeenLighten == false) break;
        //        xx += (int)direction.x;
        //        yy += (int)direction.z;
        //        if (MapController.Instance.CanPush(xx, yy) == false) break;
        //        cs.Push(chr);
                
        //    }
        //    while (cs.Count > 0)
        //    {
        //        Debug.Log(cs.Peek().name);
        //        int tp = cs.Count-1;
        //        cs.Peek().MoveTo(x + tp * (int)direction.x, y + tp * (int)direction.z,direction);
        //        cs.Pop();
        //    }
        //}

    }
    public override void MoveTo(int x, int y, Vector3 direction)
    {
        
        bool find = false;
        int dx = this.x;
        int dy = this.y;
        base.MoveTo(x, y, direction);
        int nx = dx - (int)direction.x;
        int ny = dy - (int)direction.z;
        Star star = MapController.Instance.GetStarBlock(nx, ny);
        if (star != null&&star.haveBeenLighten)
        {
            find = true;
            star.MoveTo(dx, dy, direction);
        }
        if (!find)
        {

            nx = dx + (int)Vector3.Cross(direction, Vector3.up).x;
            ny = dy + (int)Vector3.Cross(direction, Vector3.up).z;
            star = MapController.Instance.GetStarBlock(nx, ny);
            if (star != null && star.haveBeenLighten)
            {
                find = true;
                star.MoveTo(dx, dy, -Vector3.Cross(direction, Vector3.up));
            }
        }
        if (!find)
        {

            nx = dx - (int)Vector3.Cross(direction, Vector3.up).x;
            ny = dy - (int)Vector3.Cross(direction, Vector3.up).z;
            star = MapController.Instance.GetStarBlock(nx, ny);
            if (star != null && star.haveBeenLighten)
            {
                star.MoveTo(dx, dy, Vector3.Cross(direction, Vector3.up));
            }
        }
        
    }
}
