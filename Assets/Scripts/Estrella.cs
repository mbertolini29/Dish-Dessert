using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estrella : MonoBehaviour
{
    Camera mainCam; 

    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.SetActive(false);

        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 Position = Camera.main.ScreenToWorldPoint(2dPosition)

        //transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
