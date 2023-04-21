using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float width = 2.3f; //ancho de la celda
    public float height = 1.8f; //alto de la celda

    public bool isBusy = false;

    public Vector3 posCell = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        posCell = transform.position;
    }
}
