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
        UIManager.instance.PlaySoundDish();

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

            UIManager.instance.PlaySoundDish();

            //ocupas la celda donde se instancio el plato.
            currentCell.isBusy = true;

            transform.position = new Vector3(0f, 1f, 0f);            
            transform.SetParent(currentCell.gameObject.transform, false);
            transform.localScale = new Vector3(1f, 1f, 1f);

            //una vez que ocupa la celda. Busca si las porciones de tortas coinciden.
            CanSwipe();

            //desp de buscar coincidencia, si todos los platos estan ocupados. GameOver
            Grid.instance.CheckBusyCell();
        }
        else
        {
            //sino vuelve a la posicion inicial.
            transform.position = posInicial;
        }

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

                //donde habia previousSelected, lo reemplace por this.

                //plato que recien apoyaste en la grilla, y su tipo de torta
                if(this.numCake == neighborsPrefab.numCake)
                {
                    //void CakeFill()   

                    //falla cuando canela tien 2 y el instanciado 1
                    //

                    //cant de piezas antes de realizar el cambio..
                    //int auxNeighbor = neighborsPrefab.amountPiece;
                    //int aux = this.amountPiece;

                    //cant de piezas del objeto seleccionado y el vecino.
                    int aux = this.amountPiece + neighborsPrefab.amountPiece;

                    //controlar la cant de piezas que le faltan y puede tener.          
                    if (this.createdCake.Count < cakePrefab[this.numCake].amountPieces)
                    {
                        //llenar torta seleccionada.

                        //cant de piezas del plato. 
                        for (int i = this.amountPiece; i < aux; i++)
                        {
                            //instanciar porcion de torta del vecino al seleccionado.
                            InstantiateCakePiece(i);

                            this.amountPiece++; //si le instancias, tenes que sumarle.

                            //libera la celda, si la porcion de torta se completa.
                            CellRelease(); //release=liberar

                            //destruir la porcion de torta movida del vecino.
                            DestroyCakePiece(neighbor);                          

                            //si la torta instanciada en el plato seleccionado, ya esta llena, salir.
                            if(this.createdCake.Count >= cakePrefab[this.numCake].amountPieces)
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
        GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]); 
        pieceCake.transform.SetParent(transform, false);
        pieceCake.transform.position = new Vector3(transform.position.x,
                                                    pieceCake.transform.position.y,
                                                    transform.position.z);
        createdCake.Add(pieceCake);
    }

    void CellRelease() //liberar celda, si se completo..
    {
        if (this.createdCake.Count >= cakePrefab[this.numCake].amountPieces)
        {
            //sumar puntos!
            GameManager.instance.Score += ReturnScore(this.numCake);

            //sonido de torta completa.
            UIManager.instance.PlaySoundFullCake();

            //liberas la celda seleccionada, si es que se completo.
            this.transform.parent.GetComponent<Cell>().isBusy = false;

            //destruis el objeto.
            Destroy(this.gameObject, 1.0f);
        }
    }

    int ReturnScore(int numCake)
    {
        int numScore = 0;
        switch (numCake)
        {
            case 0:
                numScore = (int)PuntuacionPiece.Cupcake;
                return numScore;
            case 1:
                numScore = (int)PuntuacionPiece.Rosquilla;
                return numScore;
            case 2:
                numScore = (int)PuntuacionPiece.Canela;
                return numScore;
        }
        return numScore;
    }

    void DestroyCakePiece(GameObject neighbor)
    {
        int numPieceCake = neighborsPrefab.createdCake.Count - 1;

        //destruye la porcion de la torta del vecino
        GameObject npc = neighborsPrefab.createdCake[numPieceCake].transform.gameObject;
        Destroy(npc, 0.5f);

        //lo remueve de la lista de porcion de tortas del vecino.
        neighborsPrefab.createdCake.RemoveAt(numPieceCake);
        neighborsPrefab.amountPiece--;

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
        //un plato tiene la posibilidad de tener mas de un tipo de postre.
        //como seria esa logica?

        //una logicas seria, 
        //si o si elegis una porcion de torta
        //lo que sucede hasta ahora 
        //si tiene lugar el plato.
        //lanzas un numero random con posibilidad de que no salga torta.
        //si es -1 no hace nada.
        //si no. elije un numero al azar de esa torta.
        //       pero si el num, es mayor a la cantidad de espacio, elige la unica
        //       posibilida o lanza un valor en base a la cant de espacio que haya?


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
        }

        //osea que esto deberia elegirse desp de instanciarlo?? 
        //int numRandom = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(typeCake)).Length);
        //Debug.Log(numRandom);
    } 

    private void OnTriggerEnter(Collider other)
    {
        //LayerMask.NameToLayer("Cell")
        if (other.gameObject.layer == 9 && !onCell)
        {            
            //esto recibe la info de la celda.
            currentCell = other.gameObject.GetComponent<Cell>();

            if (!currentCell.isBusy)
            {
                isTouchingDish = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            isTouchingDish = false;
            //currentCell = null;
        }
    }
}
