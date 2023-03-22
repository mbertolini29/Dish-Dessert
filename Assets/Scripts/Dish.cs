using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    //Num de postre
    [SerializeField] int numCake;
    //Cant de piezas del postre
    [SerializeField] int amountPiece; 

    //Todos los tipos de postres
    [SerializeField] Cake[] cakePrefab;

    //lista de postres a instanciar en cada plato
    public List<GameObject> createdCake;

    [Header("Drag & Drog")]
    public bool selected = false;
    public Vector3 posInicial = new Vector3();

    Vector3 mousePos;

    private void Start()
    {

    }

    public void CreatedCake() //crea el postre.
    {
        //Elije el postre q quiere instanciar, al azar
        //UnityEngine.Random.Range(0, 2);
        numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(typeCake)).Length);

        //Cuantas piezas
        //numPiece = UnityEngine.Random.Range(0, Enum.GetValues(typeof(AmountPiece)).Length);
        switch (numCake)
        {
            case 0:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Rosquilla);
                break;
            case 2:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Canela);
                break;
        }

        //Cada plato tiene que tener su lista de postres
        createdCake = new List<GameObject>();

        for (int i = 0; i < amountPiece; i++)
        {
            //instanciar cada postre en su plato
            GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]);

            pieceCake.transform.SetParent(transform, false);
            pieceCake.transform.position = new Vector3(transform.position.x,
                                                       pieceCake.transform.position.y, 
                                                       transform.position.z);
            createdCake.Add(pieceCake);
        }
    }

    private void OnMouseOver()
    {
        //cuando presionas el objeto
        if (Input.GetMouseButtonDown(0))
        {
            //sonido
            //
            selected = true;
        }
    }

    Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);

        Vector3 pos;
        pos.x = transform.position.x; 
        pos.y = 2f; 
        pos.z = transform.position.z;
        transform.position = pos;
    }

}
