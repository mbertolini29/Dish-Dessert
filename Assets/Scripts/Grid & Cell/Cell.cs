using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float width = 1.9f; //2.3 ancho de la celda
    public float height = 1.4f; //1.8 alto de la celda

    public bool isBusy = false;

    public Vector3 posCell = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        posCell = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("todo?");
        //if(other.gameObject)
    }
}
