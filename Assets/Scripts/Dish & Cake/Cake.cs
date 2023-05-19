using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    //todo esto ya esta completo.
    public string nameCake;
    public int piecesCount;
    public GameObject fullPiece;
    public List<GameObject> piece; //lista de porciones del postre 

    public int positionCount;
    public bool[] positionBusy;

    [Header("Table 16")]
    public Vector3[] posInicial;
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

    //cantidad de postres por nivel..
    int numCurrentCake = 1;

    public int NumCurrentCake
    {
        get { return numCurrentCake; }
        set
        {
            numCurrentCake = value;
        }
    }

    //item que tiene una torta.
    public CakeItem cakeItem;

    bool firstNum = true;

    private void Awake()
    {
        //aca deberia guardar la posicion y la scala.
        positionBusy = new bool[positionCount];
    }

    private void Start()
    {
        numCurrentCake = 1;
        //Create();
    }

    public void GetPiece()
    {

    }

    public int NumCake() //ReturnNum
    {
        if(firstNum)
        {
            firstNum = !firstNum;

            numCurrentCake++;

            //Elije el postre q quiere instanciar, al azar
            //int numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TypeCake)).Length);
            int numCake = UnityEngine.Random.Range(0, numCurrentCake);

            return numCake;
        }
        else
        {
            //lanzas un numero random con posibilidad de que no salga torta.
            //int numCake = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(TypeCake)).Length);
            int numCake = UnityEngine.Random.Range(-1, numCurrentCake);            
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
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Donut);
                break;
            case 2:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Donut);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cinnamon);
                break;
            case 3:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Donut);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Apple);
                break;
        }
        return pieceCount;
    }

    public void CreateTutorial()
    {
        cakeItem = new CakeItem();

        cakeItem._numCake = 0; //solo cupcake.
        cakeItem._pieceCount = PieceCount(cakeItem._numCake);

        DishSelect dish = FindObjectOfType<DishSelect>();
        dish.positionCount = cakeItem._pieceCount;

        //int num = dish.cakePrefab[cakeItem._numCake].piece.Count;
        //dish.positionBusy = new bool[num];

        //lista de porciones de torta.
        List<GameObject> createdCake = new List<GameObject>();
        for (int i = 0; i < cakeItem._pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.
            createdCake.Add(InstantiateCakePiece(i, cakeItem._numCake));

            dish.positionBusy[i] = true;
        }

        //guardas cada porcion de torta
        cakeItem._allCake = createdCake;

        //guardas la torta
        dish.cakeItemList.Add(cakeItem);
    }

    public void Create() //crear 3 platos, y 2 tipos de tortas.
    {
        int amountDessert = 0;
        int aux; //para guardar el num de torta que salio primero.
        
        /* primer tipo de torta */

        //item que tiene una torta.
        cakeItem = new CakeItem(); //nueva torta
        cakeItem._numCake = aux = NumCake(); //num de torta
        cakeItem._pieceCount = PieceCount(cakeItem._numCake); //cant de piezas por torta.

        DishSelect dish = FindObjectOfType<DishSelect>();

        //lista de porciones de torta.
        List<GameObject> createdCake = new List<GameObject>();
        for (int i = 0; i < cakeItem._pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.
            createdCake.Add(InstantiateCakePiece(amountDessert, cakeItem._numCake));

            //primer vuelta, esta bien, porque no hay nada ocupado, pero el problema es la siguiente porcion si sale
            dish.positionBusy[i] = true;
        }

        //guardas cada porcion de torta
        cakeItem._allCake = createdCake;

        //guardas la torta
        dish.cakeItemList.Add(cakeItem);

        /* segunda tipo de torta */

        amountDessert++;
        cakeItem = new CakeItem(); //nueva torta
        cakeItem._numCake = NumCake(); //num de torta
        if (cakeItem._numCake != -1 && cakeItem._numCake != aux)
        {
            //cant de piezas por torta.
            cakeItem._pieceCount = PieceCount(cakeItem._numCake);

            //lsita de porciones de torta
            createdCake = new List<GameObject>();
            for (int i = aux; i < aux+cakeItem._pieceCount; i++)
            {
                //instanciar porcion de torta del vecino al seleccionado.
                createdCake.Add(InstantiateCakePiece(amountDessert, cakeItem._numCake));

                //primer vuelta, esta bien, porque no hay nada ocupado, pero el problema es la siguiente porcion si sale
                dish.positionBusy[amountDessert] = true;
            }

            //guardas cada porcion de torta
            cakeItem._allCake = createdCake;

            //guardas la torta
            dish.cakeItemList.Add(cakeItem);
        }
    }

    public GameObject InstantiateCakePiece(int i, int numCake)
    {
        //esto hay q ver si queres que sea parte del plato o de la torta.
        GameObject piece = FindObjectOfType<DishSelect>().cakePrefab[numCake].piece[i]; //sos todas las tortas disponibles.
    
        //instanciar cada postre en su plato
        //Instantiate(piece);
        GameObject pieceCake = Instantiate<GameObject>(piece);

        DishSelect dish = FindObjectOfType<DishSelect>();

        pieceCake.transform.SetParent(dish.gameObject.transform, false);
        
        pieceCake.transform.localPosition = FindObjectOfType<DishSelect>().cakePrefab[numCake].posInicial[i];
        
        Quaternion quat = Quaternion.Euler(dish.rotOriginal[i]);
        pieceCake.transform.localRotation = quat;

        return pieceCake;
        //cakeItem._allCake.Add(pieceCake);
    }

}
