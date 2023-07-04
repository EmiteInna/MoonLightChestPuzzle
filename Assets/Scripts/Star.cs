
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Star: Character
{
    public int existTime;
    public int currentExistTime;
    Material material;
    public bool haveBeenLighten=true;
    public bool haveMoved = false;
    public float transition = 0.1f;
    Material GetStarMat()
    {
        return transform.GetChild(0).GetComponent<Renderer>().materials[0];
    }
    private void Awake()
    {
        material = GetStarMat();
        transform.GetChild(0).GetComponent<Renderer>().materials[0] = material;
    }
    protected override void Start()
    {
        base.Start();
        
    }
    void TransparencyTransition(float destination)
    {
        float origin = GetStarMat().GetFloat("_Transparency");
      //  Debug.Log("origin=" + origin);
        StartCoroutine(Transition(origin, destination));
    }
    IEnumerator Transition(float origin,float destination)
    {
        float timer = 0;
        while (timer < transition)
        {
            //    Debug.Log(gameObject.name+ Mathf.Lerp(origin, destination, timer / transition));
            GetStarMat().SetFloat("_Transparency", Mathf.Lerp(origin, destination, timer / transition));
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    public void UpdateStarState()
    {
        haveMoved = false;
        if(currentExistTime <= 0)
        {
            haveBeenLighten = false;
        }
        Lighten();
        TransparencyTransition((float)currentExistTime/(float)existTime);
    }
    public void Lighten()
    {
        bool flg = false;
        //1级
        MapController i = MapController.Instance;
        if (i.GetMoonBlockLevel(x + 1, y) >= 1) flg = true;
        if (i.GetMoonBlockLevel(x - 1, y) >= 1) flg = true;
        if (i.GetMoonBlockLevel(x , y+1) >= 1) flg = true;
        if (i.GetMoonBlockLevel(x , y-1) >= 1) flg = true;
        //2级
        for(int dx = -1; dx <= 1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                if (i.GetMoonBlockLevel(x + dx, y + dy) >= 2) flg = true;
            }
        }
        //3级
        if (i.GetMoonBlockLevel(x + 2, y) >= 3) flg = true;
        if (i.GetMoonBlockLevel(x - 2, y) >= 3) flg = true;
        if (i.GetMoonBlockLevel(x, y + 2) >= 3) flg = true;
        if (i.GetMoonBlockLevel(x, y - 2) >= 3) flg = true;
        if (flg)
        {
            haveBeenLighten = true;
            currentExistTime = existTime;
        }
    }
    public override void MoveTo(int x, int y, Vector3 direction)
    {
        if (haveMoved) return;
        Debug.Log("我好了");

        if (x == this.x && y == this.y) return;
        Debug.Log("我好了");

        if (haveBeenLighten == false) return;
        Debug.Log("我好了");
        Character c = MapController.Instance.GetCharacter(x, y);
        if (c!=null&&c.x==x&&c.y==y)
        {
            return;
        }
        Debug.Log("我好了");

        int dx = this.x;
        int dy = this.y;
        base.MoveTo(x, y, direction);
        
        haveMoved = true;
        bool find = false;
        int nx = dx - (int)direction.x;
        int ny = dy - (int)direction.z;
        Star star = MapController.Instance.GetStarBlock(nx, ny);
        if (star != null && star.haveBeenLighten)
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
        if (haveBeenLighten)
        {
            currentExistTime--;
        }
        
    }
}