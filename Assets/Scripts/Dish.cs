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

    public List<GameObject> neighbors;

    [Header("Drag & Drop")]
    public Vector3 posInicial = new Vector3();
    public bool isSelected = false;
    public bool isTouchingDish = false;
    public bool onCell = false;

    static Dish previousSelected = null;
    static Dish neighborsPrefab = null;
    Vector3 mousePos;

    public Cell currentCell;

    //public Action OnCell;

    //colisiones de 
    public GameObject[] hitObject;
    public Vector3 collision = Vector3.zero;
    public LayerMask layerToHit;

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
            //esto mueve el plato.
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);
            transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        }

        Debug.DrawRay(transform.position, new Vector3(0, 0, -2f), Color.white);

    }

    void SelectDish()
    {
        isSelected = true;
        previousSelected = gameObject.GetComponent<Dish>();
    }

    void DeselectDish()
    {
        isSelected = false;
        //previousSelected = null;
    }

    private void OnMouseUp()
    {
        DeselectDish();

        if (this.isTouchingDish && !this.onCell)
        {
            //cant de platos intanciados.
            GameObject.FindObjectOfType<DishManager>().AmountDish--;
            
            //this.OnCell?.Invoke();
            this.onCell = true; //plato esta en la celda.

            //ocupas la celda donde se instancio el plato.
            currentCell.isBusy = true;

            transform.position = new Vector3(0f, 1f, 0f);            
            transform.SetParent(currentCell.gameObject.transform, false);
            transform.localScale = new Vector3(1f, 1f, 1f);

            //una vez que ocupa la celda.
            //buscar si coincide con el de al lado o de arriba.

            CanSwipe();

            //if (CanSwipe())
            //{
            //    //si encontro un vecino plato, va a devolver true
            //    //entonces, vas a poder ver, si hay una torta igual
            //    //y ver si le falta algun porcion.
            //    //e intercambiar la porcion
            //    //si no quedan mas porciones en el plato, se destruye el vecino
            //    //si hay mas porciones, se resta la intercambiada.

            //}
            //else
            //{
            //    Debug.Log("No hay vecinos");
            //}

            //FindAllMatches();
        }
        else
        {
            //sino vuelve a la posicion inicial.
            transform.position = posInicial;
        }

    }

    void SwapCakePiece(Dish dishClone)
    {

    }

    //poder intercambiar
    void CanSwipe()
    {
        //si hay vecinos que tengan la misma porcion de torta, se puede intercambiar        
        //return GetAllNeighbors().Contains(previousSelected.gameObject);

        neighbors = GetAllNeighbors();

        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor != null)
            {
                //vecino que encontraste
                neighborsPrefab = neighbor.gameObject.GetComponent<Dish>();

                //plato que recien apoyaste en la grilla, y su tipo de torta
                if(previousSelected.numCake == neighborsPrefab.numCake)
                {
                    //void CakeFill()
                    //{

                    //}                   

                    //num maximo de cantidad de piezas para ese postre.                    
                    if (previousSelected.createdCake.Count < cakePrefab[previousSelected.numCake].amountPieces)
                    {
                        //llenar torta seleccionada.
                        for (int i = previousSelected.amountPiece; i < previousSelected.amountPiece + neighborsPrefab.amountPiece; i++)
                        {
                            //instanciar porcion de torta del vecino al seleccionado.
                            InstantiateCakePiece(i);

                            //libera la celda, si la porcion de torta se completa.
                            CellRelease(); //release=liberar

                            //destruir la porcion de torta movida del vecino.
                            DestroyCakePiece(neighbor);                          

                            //si la torta instanciada en el plato seleccionado, ya esta llena, salir.
                            if(createdCake.Count >= cakePrefab[previousSelected.numCake].amountPieces)
                            {
                                return;
                            }
                        }
                    }                    
                }
            }
        }
    }

    void InstantiateCakePiece(int i)
    {
        //instanciar cada postre en su plato
        GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]); //uno mas del que ya hay

        pieceCake.transform.SetParent(transform, false);
        pieceCake.transform.position = new Vector3(transform.position.x,
                                                    pieceCake.transform.position.y,
                                                    transform.position.z);
        createdCake.Add(pieceCake);
    }

    void CellRelease()
    {
        if (previousSelected.createdCake.Count >= cakePrefab[previousSelected.numCake].amountPieces)
        {
            //Debug.Log("Plato lleno perri");
            //sumar puntos!

            //liberas la celda seleccionada, si es que se completo.
            previousSelected.transform.parent.GetComponent<Cell>().isBusy = false;

            //destruis el objeto.
            Destroy(previousSelected.gameObject, 1.0f);
        }
    }

    void DestroyCakePiece(GameObject neighbor)
    {
        int numPieceCake = neighborsPrefab.createdCake.Count - 1;

        //destruye la porcion de la torta del vecino
        GameObject npc = neighborsPrefab.createdCake[numPieceCake].transform.gameObject;
        Destroy(npc, 0.5f);

        //lo remueve de la lista de porcion de tortas del vecino.
        neighborsPrefab.createdCake.RemoveAt(numPieceCake);

        //esto destruye al vecino si se queda sin porcion.
        if (neighborsPrefab.createdCake.Count == 0)
        {
            //liberas la celda del vecino. si se quedo sin porciones.
            neighborsPrefab.transform.parent.GetComponent<Cell>().isBusy = false;
            Destroy(neighbor, 0.25f);
        }
    }

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
        //lista para todos los vecinos.
        List<GameObject> neighbors = new List<GameObject>();

        //recorre las direcciones adyacentes del plato.
        //y para cada direccion consulta el vecino, y lo agrega a la lista de todos los vecinos. 
        foreach (Vector3 direction in adjecentDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }

        return neighbors;
    }

    GameObject GetNeighbor(Vector3 direction)
    {
        Ray ray;

        ray = new Ray(previousSelected.transform.position, direction);

        //ray = new Ray(transform.position, direction);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);

        //this.gameObject
        //        
        RaycastHit hit;
        float maxDistance = 1.5f;
        Physics.Raycast(ray, out hit, maxDistance, layerToHit);

        //golpear a un vecino con la misma porcion de torta??
        if (hit.collider != null)
        {
            //Debug.Log("Hola, choque con un plato de torta.");
            //return hit.collider.gameObject;

            //si el padre es igual al padre que colisiono.
            // destruir. esto no deberia pasar.
            if (transform.parent == hit.collider.transform.parent)
            {
                return null;
            }
            else
            {
                //Debug.Log("Hola, choque con un plato de torta.");
                return hit.collider.gameObject;
            }
        }
        else
        {
            return null;
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
            //instanciar porcion de torta del vecino al seleccionado.
            InstantiateCakePiece(i);

            //instanciar cada postre en su plato
            //GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]);

            //pieceCake.transform.SetParent(transform, false);
            //pieceCake.transform.position = new Vector3(transform.position.x,
            //                                           pieceCake.transform.position.y,
            //                                           transform.position.z);
            //createdCake.Add(pieceCake);
        }
    } 

    private void OnTriggerEnter(Collider other)
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
            }
            //else
            //{
            //    isTouchingDish = false;
            //}  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9) // && !isSelected) //"Cell")
        {
            //Debug.Log("dejaste de tocar");
            isTouchingDish = false;
            //currentCell = null;
        }
    }
}
