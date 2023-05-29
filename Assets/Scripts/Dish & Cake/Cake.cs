using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    //todo esto ya esta completo.
    public string nameCake;
    public int piecesCount;
    public GameObject piece;
    public GameObject fullPiece;

    //public List<GameObject> listPiece; //lista de porciones del postre 

    public int positionCount;
    public bool[] positionBusy;

    [Header("Mesa")]
    public Vector3[] posInicial;
    public Vector3[] posOriginal;
    public Vector3[] rotOriginal;
    public Quaternion[] quatOriginal;
    public Vector3 scaleOriginal;

    //posiciones para 6 porciones de torta.
    [Header("Mesada Posiciones")]
    public Vector3[] posBottom;
    public Vector3 scaleBottom;

    //lista de todas las tortas de un plato.
    //esta lista deberia ser del plato
    //public List<CakeItem> cakeItemList = new List<CakeItem>();

    //cantidad de postres por nivel..
    //int numCurrentCake = 1;
    int numCurrentCake = 8;

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
        numCurrentCake = 8;
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

            //Elije el postre q quiere instanciar, al azar
            //numCurrentCake++;
            //int numCake = UnityEngine.Random.Range(0, numCurrentCake);

            //Elije el postre q quiere instanciar, al azar
            int numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TypeCake)).Length);

            return numCake;
        }
        else
        {
            //lanzas un numero random con posibilidad de que no salga torta.
            int numCake = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(TypeCake)).Length);
            //int numCake = UnityEngine.Random.Range(-1, numCurrentCake);            
            return numCake;
        }
    }

    public int PieceCount(int numCake)
    {
        int pieceCount = 0;

        switch (numCake)
        {
            case 0:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Donut);
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 2:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cinnamon);
                break;
            case 3:         
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Bagel);
                break;
            case 4:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Apple);
                break;
            case 5:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                //pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Rainbow);
                break;
        }
        return (pieceCount);
    }

    public int ModulesCount(int numCake)
    {
        int modulesCount = 0;

        switch (numCake)
        {
            case 0:
                modulesCount = 4;
                break;
            case 1:
                //modulesCount = 4; 
                modulesCount = 2; 
                break;
            case 2:
                //modulesCount = 2;
                modulesCount = 1;
                break;
            case 3:         
                modulesCount = 2;
                break;
            case 4:
                modulesCount = 1;
                break;
            case 5:
                modulesCount = 1;
                break;
        }
        return modulesCount;
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
            //createdCake.Add(InstantiateCakePiece(i, cakeItem._numCake));

            //dish.positionBusy[i] = true;
        }

        //guardas cada porcion de torta
        cakeItem._allCake = createdCake;

        //guardas la torta
        dish.cakeItemList.Add(cakeItem);
    }
       
    int amountBusyPlaces = 0;
    int amountDessert = 0;

    public void Create() //crear 3 platos, y 2 tipos de tortas.
    {
        int aux; //para guardar el num de torta que salio primero.
        
        /* primer tipo de torta */

        //item que tiene una torta.
        cakeItem = new CakeItem(); //nueva torta
        cakeItem._numCake = aux = NumCake(); //num de torta
        cakeItem._pieceCount = PieceCount(cakeItem._numCake); //cant de piezas por torta.
        cakeItem._modulesCount = ModulesCount(cakeItem._numCake); //numero de modulos/porciones que tiene una torta.

        //guardas, que lugar ocupo el postre. O sea, 
        //si es 0 esta en el primer lugar
        //si es 2 es un 1/4
        //si es 4 esta en la mitad del plato.
        cakeItem._filledPlace = amountDessert; 

        DishSelect dish = FindObjectOfType<DishSelect>();

        //lista de porciones de torta.
        List<GameObject> createdCake = new List<GameObject>();
        for (int i = 0; i < cakeItem._pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.

            //instanciar porcion de torta del vecino al seleccionado.

            //primero tenes que saber que torta vas a instanciar..
            createdCake.Add(InstantiateCakePiece(cakeItem._numCake));

            //porque, no es lo mismo un cupcake que un cinnamon

            //un cupcake, ocupa 4 lugares.

            //un cinnamon ocupa 2 lugares.  

            //primer vuelta, esta bien, porque no hay nada ocupado, pero el problema es la siguiente porcion si sale
            //dish.positionBusy[i] = true;
        }

        //guarda la cant de lugares ocupados.
        cakeItem._busyPlace = amountBusyPlaces;

        //guardas cada porcion de torta
        cakeItem._allCake = createdCake;

        //guardas la torta
        dish.cakeItemList.Add(cakeItem);

        /* segunda tipo de torta */

        //amountDessert++;
        cakeItem = new CakeItem(); //nueva torta
        cakeItem._numCake = NumCake(); //num de torta
        
        if (cakeItem._numCake != -1 && cakeItem._numCake != aux)
        {
            //cant de piezas por torta.
            cakeItem._pieceCount = PieceCount(cakeItem._numCake); 
            
            //numero de modulos que tiene una torta.
            cakeItem._modulesCount = ModulesCount(cakeItem._numCake);

            //guardas, que lugar ocupo el postre.
            cakeItem._filledPlace = amountDessert; 

            //lsita de porciones de torta
            createdCake = new List<GameObject>();
            for (int i = aux; i < aux+cakeItem._pieceCount; i++)
            {
                //instanciar porcion de torta del vecino al seleccionado.
                createdCake.Add(InstantiateCakePiece(cakeItem._numCake));

                //primer vuelta, esta bien, porque no hay nada ocupado, pero el problema es la siguiente porcion si sale
                //dish.positionBusy[amountDessert] = true;
            }

            //guarda la cant de lugares ocupados.
            cakeItem._busyPlace = amountBusyPlaces;

            //guardas cada porcion de torta
            cakeItem._allCake = createdCake;

            //guardas la torta
            dish.cakeItemList.Add(cakeItem);
        }
    }

    public GameObject InstantiateCakePiece(int numCake)
    {
        //esto hay q ver si queres que sea parte del plato o de la torta.
        GameObject _piece = FindObjectOfType<DishSelect>().cakePrefab[numCake].piece; //sos todas las tortas disponibles.
    
        //instanciar cada postre en su plato
        //Instantiate(piece);
        GameObject pieceCake = Instantiate<GameObject>(_piece);

        DishSelect dish = FindObjectOfType<DishSelect>();        

        //pieceCake.transform.SetParent(dish.gameObject.transform, false);
        //esto se haria hijo de la posicion del plato correcta, y desocupada.
        pieceCake.transform.SetParent(dish.posRotPiece[amountBusyPlaces].gameObject.transform, false);

        //ocupar espacios dependiendo el tipo de torta.
        if(pieceCake.gameObject.tag == "Cupcake")
        {
            //ocupa 4 lugares.
            if(!dish.positionBusy[amountBusyPlaces])
            {
                //ocupa 4 lugares.
                for (int k = amountBusyPlaces; k < amountDessert + 4; k++)
                {
                    dish.positionBusy[k] = true;
                    amountBusyPlaces++;
                }
            }
        }   
        else if(pieceCake.gameObject.tag == "Donut")
        {
            //ocupa 4 lugares.
            if (!dish.positionBusy[amountBusyPlaces])
            {
                //ocupa 4 lugares.
                //for (int k = amountBusyPlaces; k < amountDessert + 4; k++)
                for (int k = amountBusyPlaces; k < amountDessert + 2; k++)
                {
                    dish.positionBusy[k] = true;
                    amountBusyPlaces++;
                }
            }
        }
        else if (pieceCake.gameObject.tag == "Cinnamon")
        {
            if (!dish.positionBusy[amountBusyPlaces])
            {
                //ocupa 2 lugares.
                //for (int k = amountBusyPlaces; k < amountDessert + 2; k++)
                for (int k = amountBusyPlaces; k < amountDessert + 1; k++)
                {
                    dish.positionBusy[k] = true;
                    amountBusyPlaces++;
                }
            }
        }

        amountDessert = amountBusyPlaces;

        return pieceCake;
    }

}
