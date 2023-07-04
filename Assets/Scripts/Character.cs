using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character:MonoBehaviour
{
    public int x, y;
    public float movementDuration = 0.4f;

    protected virtual void Start()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.z;
    }
    public virtual void MoveTo(int x, int y,Vector3 direction)
    {
        StartCoroutine(MovementTo(this.x, this.y, x, y));
        this.x = x;
        this.y = y;
        
    }
    IEnumerator MovementTo(int ox,int oy,int x,int y)
    {
        float timer = 0;
        while(timer < movementDuration)
        {
            transform.position = Vector3.Lerp(new Vector3(ox,transform.position.y,oy),new Vector3(x,transform.position.y,y),timer/movementDuration);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        if(this is PC) MapController.Instance.RefreshStageLight();
        yield return null;
    }
}