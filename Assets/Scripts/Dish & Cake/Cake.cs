using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeCake
{
    Cupcake = 2, Cinnamon = 4, Apple = 6
}

public enum AmountPiece
{
    Cupcake = 2, Cinnamon = 4, Apple = 6
}

public enum PuntuacionPiece
{
    Cupcake = 100, Cinnamon = 150, Apple = 200
}

public class Cake : MonoBehaviour
{
    //todo esto ya esta completo.
    public string nameCake;
    public int piecesCount;
    public List<GameObject> piece; //lista de porciones del postre 

    [Header("Table 16")]
    public Vector3[] posOriginal;
    public Vector3 scaleOriginal;

    //posiciones para 6 porciones de torta.
    [Header("Table 4")]
    public Vector3[] posBottom;
    public Vector3 scaleBottom;

    //lista de todas las tortas de un plato.
    public List<CakeItem> cakeItemList = new List<CakeItem>();

    //item que tiene una torta.
    CakeItem cakeItem;

    bool firstNum = true;

    private void Awake()
    {
        //aca deberia guardar la posicion y la scala.

    }


    public void GetPiece()
    {

    }

    public int NumCake()
    {
        if(firstNum)
        {
            firstNum = !firstNum;
            //Elije el postre q quiere instanciar, al azar
            //UnityEngine.Random.Range(0, 2);
            int numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TypeCake)).Length);
            return numCake;
        }
        else
        {
            //lanzas un numero random con posibilidad de que no salga torta.
            int numCake = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(TypeCake)).Length);
            return numCake;
        }

    }

    public int PieceCount(int numCake)
    {
        int pieceCount = 0;
        //Cuantas piezas
        switch (numCake)
        {
            case 0:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cinnamon);
                break;
            case 2:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Apple);
                break;
        }
        return pieceCount;
    }

    public void Create()
    {
        //lista de porciones de torta.
        //FindObjectOfType<Dish>().createdCake = new List<GameObject>();

        //item que tiene una torta.
        cakeItem = new CakeItem();

        cakeItem._numCake = NumCake();
        cakeItem._pieceCount = PieceCount(cakeItem._numCake);

        for (int i = 0; i < cakeItem._pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.
            InstantiateCakePiece(cakeItem, i, cakeItem._numCake);
        }

        //guardas la torta
        cakeItemList.Add(cakeItem);
    }

    public void InstantiateCakePiece(CakeItem cakeItem, int i, int numCake)
    {
        //lista de porciones de torta.
        FindObjectOfType<Dish>().createdCake = new List<GameObject>();

        GameObject piece = FindObjectOfType<Dish>().cakePrefab[numCake].piece[i];

        //instanciar cada postre en su plato
        Instantiate<GameObject>(piece);

        piece.transform.SetParent(FindObjectOfType<Dish>().transform, false);
        piece.transform.position = new Vector3(transform.position.x,
                                               piece.transform.position.y,
                                               transform.position.z);

        FindObjectOfType<Dish>().createdCake.Add(piece);

        cakeItem._allCake = FindObjectOfType<Dish>().createdCake;
    }



}
