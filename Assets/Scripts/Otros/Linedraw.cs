using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linedraw : MonoBehaviour
{
    LineRenderer lineRender;

    Transform target;

    Vector3 mousePosition;
    Vector3 startPosition;
    float distance;

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        //lineRender.positionCount = 2;
    }

    //public void AssignTarget(Vector3 startPosition, Transform newTarget)
    //{
    //    lineRender.positionCount = 2;
    //    lineRender.SetPosition(0, startPosition);
    //    target = newTarget;         
    //}

    public void AssignTarget(Vector3 startPosition, Vector3 endPosition)
    {
        lineRender.positionCount = 2;
        lineRender.SetPosition(0, startPosition);
        lineRender.SetPosition(1, endPosition);
        //target = newTarget;         
    }

    private void Update()
    {
        //lineRender.SetPosition(1, target.position);


    }


}
