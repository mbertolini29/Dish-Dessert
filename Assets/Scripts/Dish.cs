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
    public List<GameObject> createdCake; //no seria, la lista de postres instanciado en cada plato.

    [Header("Drag & Drog")]
    public Vector3 posInicial = new Vector3();
    public bool isSelected = false;
    public bool isTouchingDish = false;
    public bool onCell = false;

    static Dish previousSelected = null;
    Vector3 mousePos;

    public Cell currentCell;

    //public Action OnCell;

    //colisiones de 
    public GameObject[] hitObject;
    public Vector3 collision = Vector3.zero;
    public LayerMask layerToHit;

    //vecinos
    Vector3[] adjecentDirections = new Vector3[]
    {
        Vector3.forward, //adelante (arriba)
        Vector3.back, //atras (abajo)
        Vector3.left,
        Vector3.right
    };

    List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach (Vector3 direction in adjecentDirections)
        {
            //neighbors.Add(GetNeighbor(direction));

        }
        return neighbors;
    }

    public void FindAllMatches()
    {
        if (this.gameObject == null)
        {
            return;
        }

        //esto busca coincidencias horizontal 

        bool hMatch = ClearMatch(new Vector3[2] { //vector con 2 direcciones..

            new Vector3(2.1f, 0f, 0f), new Vector3(-2.1f, 0f, 0f)
            //Vector3.left, Vector3.right
        });

        //esto busca coincidencias vertical

        bool vMatch = ClearMatch(new Vector3[2] { //vector con 2 direcciones..

            new Vector3(0f, 0f, 1.6f), new Vector3(0f, 0f, -1.6f)
            //Vector3.forward, Vector3.back
        });
    }

    bool ClearMatch(Vector3[] directions)
    {
        List<GameObject> matchCake = new List<GameObject>();

        foreach (Vector3 direction in directions) //
        {
            //agrega a la lista objetos iguales.
            matchCake.AddRange(FindMatch(direction));
        }

        //aca tiene que pasar algo... con la lista de match cake. 
        //si algo coincide, debes de limpear.
        //tenes que juntar las porciones que coinciden

        //obtenes una lista de cada objeto que encontraste en el find match.
        foreach (var item in matchCake)
        {

        }

        foreach (GameObject dish in matchCake)
        {
            //este destruye el plato de al lado..
            dish.SetActive(false);

            //primero saber cuantas piezas tiene
            //dish.GetComponentInChildren<Cake>().amountPieces 
               
            //segundo, que 
                
            //pero como recibe la informacion el instanciado?

        }


        return true;
    }

    List<GameObject> FindMatch(Vector3 direction)
    {
        //matchDish == combinacion de platos.
        List<GameObject> matchCake = new List<GameObject>();

        Ray ray;
        //RaycastHit hit;
        RaycastHit[] hits;

        ray = new Ray(transform.position, direction);

        hits = Physics.RaycastAll(ray);

        foreach (var item in hits)
        {
            if (item.collider.gameObject.layer == 6)
            {
                Debug.Log("coincidieron platos");

                //el numero de torta que dejaste en la celda,
                //es igual al numero de torta de la celda de al lado, 
                //y por lo tanto, tenes que unirlo.?

                if (this.numCake == item.collider.GetComponentInChildren<Dish>().numCake)
                {
                    //esto busca el match, para luego eliminarlo
                    matchCake.Add(item.collider.gameObject);

                    //pero antes de eliminarlo, hay que pasarlo al plato que pusiste
             
                    //cantidad de piezas del plato adyacente.
                    var amountPieceOtherCake = item.collider.GetComponentInChildren<Dish>().amountPiece;

                    //num maximo de cantidad de piezas para ese postre.
                    if (createdCake.Count < cakePrefab[numCake].amountPieces)
                    {
                        //aca tendrias que ponerle al plato instanciado. 
                        //la cantidad de postres que tenia la torta adjacente.
                        InstantiatePieceCake(createdCake, numCake, amountPieceOtherCake, matchCake);
                        //createdCake.Add();
                    }
                }
            }
        }

        return matchCake;
    }

    void InstantiatePieceCake(List<GameObject> createdCake, int numCake, int amountPieceOtherCake, List<GameObject> matchCake)
    {
        //Cada plato tiene que tener su lista de postres
        //createdCake = new List<GameObject>();

        for (int i = this.amountPiece; i < this.amountPiece + amountPieceOtherCake; i++)
        {
            //instanciar cada postre en su plato
            GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]); //uno mas del que ya hay

            pieceCake.transform.SetParent(transform, false);
            pieceCake.transform.position = new Vector3(transform.position.x,
                                                        pieceCake.transform.position.y,
                                                        transform.position.z);
            createdCake.Add(pieceCake);    
        }
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

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos();
    }

    Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDrag()
    {
        SelectDish();

        if (!onCell)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);

            //esto mueve el plato.
            Vector3 pos;
            pos.x = transform.position.x;
            pos.y = 2f;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }

    void SelectDish()
    {
        isSelected = true;
        previousSelected = gameObject.GetComponent<Dish>();
    }

    void DeselectDish()
    {
        isSelected = false;
        previousSelected = null;
    }

    private void OnMouseUp()
    {
        DeselectDish();

        if (isTouchingDish)
        {
            //DishManager.instance.AmountDish--;
            GameObject.FindObjectOfType<DishManager>().AmountDish--;
            //int amount = GameObject.FindObjectOfType<DishManager>().AmountDish;

            //plato esta en la celda.
            onCell = true;
            //this.OnCell?.Invoke();

            //esto deja el plato cerca de la celda.
            //Vector3 pos;
            //pos.x = 0f;
            //pos.y = 1f;
            //pos.z = 0f;
            transform.position = new Vector3(0f, 1f, 0f); ;

            //tmb, hay que hacerlo hijo de la celda/plato de la grilla.
            transform.SetParent(currentCell.gameObject.transform, false);

            transform.localScale = new Vector3(1f, 1f, 1f);

            //ocupas la celda donde se instancio el plato.
            currentCell.isBusy = true;

            //una vez que ocupa la celda.
            //buscar si coincide con el de al lado o de arriba.

            FindAllMatches();
        }
        else
        {
            //sino vuelve a la posicion inicial.
            transform.position = posInicial;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        // && !onCell) // && !isSelected) //"Cell")
        //LayerMask.NameToLayer("Cell")
        if (other.gameObject.layer == 9 && !onCell)
        {
            //esto recibe la info de la celda.
            currentCell = other.gameObject.GetComponent<Cell>();

            if (!currentCell.isBusy)
            {
                //Debug.Log("tocaste?");
                isTouchingDish = true;

                //Vector3 pos;
                //pos.x = other.gameObject.transform.position.x;
                //pos.y = 0.1f;
                //pos.z = other.gameObject.transform.position.z;
                //transform.position = pos;
            }
            else
            {
                isTouchingDish = false;
            }

            //transform.SetParent(other.gameObject.transform, false);            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9) // && !isSelected) //"Cell")
        {
            //Debug.Log("tocaste?");
            isTouchingDish = false;
            //transform.position = other.gameObject.transform.position;
        }
    }

}
