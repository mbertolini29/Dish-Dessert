using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CakeItem
{
    public List<GameObject> _allCake;
    public int _countCake; //cant de tortas.
    public int _numCake; //Num de postre.
    public int _pieceCount; //Cant de piezas del postre.
}

public class Dish : MonoBehaviour
{
    public List<CakeItem> cakeItemList = new List<CakeItem>();
    CakeItem cakeItem;

    //[Header("Lista de postres instanciados.")]
    List<GameObject> createdCake; //la lista de postres instanciado en cada plato.

    //[Header("Postres individuales.")]
    //Num de postre
    int numCake;
    //Cant de piezas del postre
    int pieceCount;

    [Header("Tipos de postres.")]
    //Todos los tipos de postres
    [SerializeField] Cake[] cakePrefab;

    [Header("Lista de vecinos.")] 
    public List<GameObject> neighbors;

    [Header("Drag & Drop")]
    public Vector3 posInicial = new Vector3();
    public bool isSelected = false;
    public bool isTouchingDish = false;
    public bool onCell = false;

    static Dish previousSelected = null;
    static Dish neighborsPrefab = null;

    static Dish neighborDish = null;
    Vector3 mousePos;

    [Header("Celda donde esta el plato.")]
    public Cell currentCell;

    //public Action OnCell;

    [Header("Colisiones.")]
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

    void DestroyCakePieceNeighbor(GameObject selectedPrefab, int i)
    {
        int numPieceCake = this.cakeItemList[i]._allCake.Count - 1;

        //destruye la porcion de la torta del vecino
        GameObject npc = this.cakeItemList[i]._allCake[numPieceCake].transform.gameObject;
        Destroy(npc, 0.5f);

        //lo remueve de la lista de porcion de tortas del vecino.
        this.cakeItemList[i]._allCake.RemoveAt(numPieceCake);
        this.cakeItemList[i]._pieceCount--;

        //esto destruye al vecino si se queda sin porcion.
        if (this.cakeItemList[i]._allCake.Count == 0)
        {
            //liberas la celda del vecino. si se quedo sin porciones.
            this.transform.parent.GetComponent<Cell>().isBusy = false;
            Destroy(selectedPrefab, 0.25f);
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

        RaycastHit hit;
        float maxDistance = 1.5f;
        Physics.Raycast(ray, out hit, maxDistance, layerToHit);

        if(hit.collider != null)
        {
            //porque aca tmb, podria identificar las conexiones con un tag
            //hit.collider.tag == "Magdalena"

            //vecino que encontraste //0.1
            neighborDish = hit.collider.gameObject.GetComponent<Dish>();
            GameObject neighbor = hit.collider.gameObject; //esto para poder eliminarlo. 

            NeighborCheck(neighborDish, neighbor);

            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    void NeighborCheck(Dish neighborDish, GameObject neighbor)
    {
        ////cuantas torta tiene el vecino y el plato que dejaste.
        //int neighborCount = neighborDish.cakeItemList.Count;
        //int cakeCount = this.cakeItemList.Count; //el plato seleccionado y dejado.

        //primero te chequeas al seleccionado.
        for (int i = 0; i < this.cakeItemList.Count; i++)
        {
            for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
            {
                //chequeas si el vecinoo tiene un porcion igual.
                if (this.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                {
                    if(neighborDish.cakeItemList.Count == 0 || this.cakeItemList.Count == 0)
                    {
                        return; //si uno de los 2 esta vacio. No seguir.
                    }

                    //cant de piezas del objeto seleccionado y el vecino.
                    int aux = this.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

                    if (neighborDish.cakeItemList.Count == 1) //mover la porcion de torta desde el seleccionado al vecino.
                    {
                        Dish dish = neighborDish;
                        Dish destroyPiece = this.gameObject.GetComponent<Dish>();
                        //funcion que mueva la pieza del vecino.
                        MovePiece(dish, destroyPiece, neighbor, aux, i, j);                               
                    }
                    else if(this.cakeItemList.Count == 1) //ir del vecino hacia la torta que pusiste
                    {
                        Dish dish = this.gameObject.GetComponent<Dish>();
                        Dish destroyPiece = neighborDish;

                        //funcion que mueve la pieza del plato que se mueve.
                        MovePiece(dish, destroyPiece, this.gameObject, aux, i, j);
                    }
                }
            }
        }   
    }

    void MovePiece(Dish movePiece, Dish destroyPiece, GameObject neighbor, int aux, int i, int j)
    {
        //para intanciarle al vecino, las porciones de torta del seleccionado.
        if (movePiece.cakeItemList[j]._allCake.Count <= cakePrefab[movePiece.cakeItemList[j]._numCake].piece.Count)
        {
            //llenar torta seleccionada.
            for (int k = movePiece.cakeItemList[j]._pieceCount; k < aux; k++)
            {
                //instanciar porcion de torta del vecino al seleccionado.
                GameObject pieceCake = Instantiate<GameObject>(cakePrefab[movePiece.cakeItemList[j]._numCake].piece[k]);
                pieceCake.transform.SetParent(movePiece.transform, false);
                pieceCake.transform.position = new Vector3(movePiece.transform.position.x, 
                                                           pieceCake.transform.position.y,
                                                           movePiece.transform.position.z);
                movePiece.cakeItemList[j]._allCake.Add(pieceCake);

                //si le instancias, tenes que sumarle.
                movePiece.cakeItemList[j]._pieceCount++;

                //libera la celda, si la porcion de torta se completa.
                CellRelease(movePiece, j); //release=liberar                                

                //destruir la porcion de torta movida.
                DestroyCakePiece(destroyPiece, neighbor, i);

                //si la torta instanciada en el plato seleccionado, ya esta llena, salir.
                if (movePiece.cakeItemList[j]._allCake.Count >= cakePrefab[movePiece.cakeItemList[j]._numCake].piece.Count)
                {
                    return;
                }
            }
        }
    }

    void CellRelease(Dish movePiece, int i) //liberar celda, si se completo..
    {
        if (movePiece.cakeItemList[i]._allCake.Count >= cakePrefab[movePiece.cakeItemList[i]._numCake].piece.Count)
        {
            //sumar puntos!
            GameManager.instance.Score += ReturnScore(movePiece.cakeItemList[i]._numCake);

            //sonido de torta completa.
            UIManager.instance.PlaySoundFullCake();

            //liberas la celda seleccionada, si es que se completo.
            this.transform.parent.GetComponent<Cell>().isBusy = false;

            //destruis el plato, una vez que se quedo sin porciones.
            //Destroy(this.cakeItemList[j]._allCake[j], 1.0f);
            Destroy(movePiece.gameObject, 1.0f);
        }
    }

    void DestroyCakePiece(Dish destroyPiece, GameObject neighbor, int j)
    {
        int numPieceCake = destroyPiece.cakeItemList[j]._allCake.Count - 1;

        //destruye la porcion de la torta del vecino
        GameObject npc = destroyPiece.cakeItemList[j]._allCake[numPieceCake].transform.gameObject;
        Destroy(npc, 0.5f);

        //lo remueve de la lista de porcion de tortas del vecino.
        destroyPiece.cakeItemList[j]._allCake.RemoveAt(numPieceCake);
        //destroyPiece.cakeItemList[j]._pieceCount--;
        destroyPiece.cakeItemList.RemoveAt(j);

        //esto destruye al vecino si se queda sin porcion.
        if (destroyPiece.cakeItemList.Count == 0)
        {
            //liberas la celda del vecino. si se quedo sin porciones.
            destroyPiece.transform.parent.GetComponent<Cell>().isBusy = false;
            Destroy(neighbor, 0.5f);
        }
    }

    //------------
    //1° cosa que se crea.
    public void CreatedCake() //crea el postre.
    {
        //lista de porciones de torta.
        createdCake = new List<GameObject>();
        cakeItem = new CakeItem();

        //Elije el postre q quiere instanciar, al azar
        //UnityEngine.Random.Range(0, 2);
        numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TypeCake)).Length);

        //Cuantas piezas
        //numPiece = UnityEngine.Random.Range(0, Enum.GetValues(typeof(AmountPiece)).Length);
        switch (numCake)
        {
            case 0:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Rosquilla);
                break;
            case 2:
                pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Canela);
                break;
        }

        for (int i = 0; i < pieceCount; i++)
        {
            //instanciar porcion de torta del vecino al seleccionado.
            InstantiateCakePiece(i, numCake);
        }

        //guardas la torta
        cakeItem._allCake = createdCake;
        cakeItem._numCake = numCake;
        cakeItem._pieceCount = pieceCount;
        cakeItemList.Add(cakeItem);

        //lista de porciones de torta.
        createdCake = new List<GameObject>();
        cakeItem = new CakeItem();

        //lanzas un numero random con posibilidad de que no salga torta.
        int numRandom = UnityEngine.Random.Range(-1, Enum.GetValues(typeof(TypeCake)).Length);

        if (numRandom != -1 && numRandom != numCake)
        {
            numCake = numRandom;
            switch (numRandom)
            {
                case 0:
                    pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                    break;
                case 1:
                    pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Rosquilla);
                    break;
                case 2:
                    pieceCount = UnityEngine.Random.Range(1, (int)AmountPiece.Canela);
                    break;
            }

            for (int i = 0; i < pieceCount; i++)
            {
                //instanciar porcion de torta del vecino al seleccionado.
                InstantiateCakePiece(i, numCake);
                
                //ver como acomodar desp.
                //createdCake[i].transform.rotation = Quaternion.Euler(transform.rotation.x, -90f, transform.rotation.z);
            }

            //guardas otra torta en el plato.
            //allDishCake.Add(createdCake);

            //guardas la torta
            cakeItem._allCake = createdCake;
            cakeItem._numCake = numCake;
            cakeItem._pieceCount = pieceCount;
            cakeItemList.Add(cakeItem);
        }

        //Debug.Log(numRandom);
    }

    void InstantiateCakePiece(int i, int numCake)
    {
        //instanciar cada postre en su plato
        GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].piece[i]);
        pieceCake.transform.SetParent(transform, false);
        pieceCake.transform.position = new Vector3(transform.position.x,
                                                    pieceCake.transform.position.y,
                                                    transform.position.z);

        createdCake.Add(pieceCake);
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