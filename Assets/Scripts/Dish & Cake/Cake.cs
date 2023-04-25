using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeCake
{
    Cupcake = 2, Donut = 3, Cinnamon = 4, Apple = 6
}

public enum AmountPiece
{ 
    //aca hay que poner el num maximo de porciones por torta que quieras que salgan.
    //cinnon diria que salgan 2 porciones y apple 3
    Cupcake = 2, Donut = 2, Cinnamon = 3, Apple = 4
}

public enum PuntuacionPiece
{
    Cupcake = 100, Donut = 125, Cinnamon = 150, Apple = 200
}

public class Cake : MonoBehaviour
{
    //todo esto ya esta completo.
    public string nameCake;
    public int piecesCount;
    public List<GameObject> piece; //lista de porciones del postre 

    [Header("Table 16")]
    public Vector3[] posOriginal;
    public Vector3[] rotOriginal;
    public Quaternion[] quatOriginal;
    public Vector3 scaleOriginal;

    //posiciones para 6 porciones de torta.
    [Header("Table 4")]
    public Vector3[] posBottom;
    public Vector3 scaleBottom;

    //lista de todas las tortas de un plato.
    //esta lista deberia ser del plato
    //public List<CakeItem> cakeItemList = new List<CakeItem>();

    //item que tiene una torta.
    CakeItem cakeItem;

    bool firstNum = true;

    private void Awake()
    {
        //aca deberia guardar la posicion y la scala.

    }

    private void Start()
    {
        Create();
    }

    public void GetPiece()
    {

    }

    public int NumCake() //ReturnNum
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

        //lanzas un numero random con posibilidad de que no salga torta.
        //int numRandom = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(TypeCake)).Length);

        //if (numRandom != -1 && numRandom != numCake)

        //Cuantas piezas
        switch (numCake)
        {
            case 0:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Donut);
                break;
            case 2:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cinnamon);
                break;
            case 3:
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

        Dish dish = FindObjectOfType<Dish>();
        dish.positionCount = cakeItem._pieceCount;

        int num = dish.cakePrefab[cakeItem._numCake].piece.Count;

        dish.positionBusy = new bool[num];

        //lista de porciones de torta.
        List<GameObject> createdCake = new List<GameObject>();
        for (int i = 0; i < cakeItem._pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.
            createdCake.Add(InstantiateCakePiece(i, cakeItem._numCake));

            FindObjectOfType<Dish>().positionBusy[i] = true;
            //dish = FindObjectOfType<Dish>();
            //dish.positionBusy[i] = true;
        }

        //guardas cada porcion de torta
        cakeItem._allCake = createdCake;

        //guardas la torta
        FindObjectOfType<Dish>().cakeItemList.Add(cakeItem);
    }

    public GameObject InstantiateCakePiece(int i, int numCake)
    {
        //esto hay q ver si queres que sea parte del plato o de la torta.
        GameObject piece = FindObjectOfType<Dish>().cakePrefab[numCake].piece[i]; //sos todas las tortas disponibles.

        //instanciar cada postre en su plato
        //Instantiate(piece);
        GameObject pieceCake = Instantiate<GameObject>(piece);

        Dish dish = FindObjectOfType<Dish>();

        pieceCake.transform.SetParent(dish.gameObject.transform, false);

        //pieceCake.transform.position = posOriginal[i];
        //pieceCake.transform.position = new Vector3(dish.transform.position.x,
        //                                           dish.transform.position.y,
        //                                           dish.transform.position.z);

        return pieceCake;
        //cakeItem._allCake.Add(pieceCake);
    }



}
