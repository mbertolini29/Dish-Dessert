using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    [Header("Particle")]
    public GameObject particle;
    public GameObject particleWave;
    public GameObject particleSmoke;

    [Header("Material")] //para cambiar a apple
    public Mesh mesh;
    
    [Header("Estrella Puntuación")] //para cambiar a apple
    public GameObject estrella;
    public GameObject background;

    //se scala a mayor
    Vector3 startingScale = new Vector3(1f, 1f, 1f);
    Vector3 endingScale = new Vector3(1.1f, 1.1f, 1.1f);

    public GameObject[] dishConection;
    GameObject connection;
    public bool isNeighbor = false;

    //cada plato tiene 5 posiciones mixta, si el plato solo tiene apple, cambia a 6, sino 4 o 2
    public int positionCount = 5;
    public bool[] positionBusy;

    //es por aca.. chequear las posiciones ocupadas.
    //y en base a eso, activar las animaciones, y guardar las nuevas posiciones ocupadas

    public List<CakeItem> cakeItemList = new List<CakeItem>();
    CakeItem cakeItem;

    //[Header("Lista de postres instanciados.")]
    public List<GameObject> createdCake; //la lista de postres instanciado en cada plato.

    //[Header("Postres individuales.")]
    int numCake; //numDessert
    int pieceCount;

    [Header("Tipos de postres")]
    //Todos los tipos de postres
    public Cake[] cakePrefab;

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
    static bool destroyDish = false;

    [Header("Celda donde esta el plato.")]
    public Cell currentCell;

    //public Action OnCell;

    [Header("Colisiones.")]
    public GameObject[] hitObject;
    public Vector3 collision = Vector3.zero;
    public LayerMask layerToHit;

    private void Awake()
    {
       // particle = GetComponent<ParticleSystem>();       

        //ponemos las 5 posiciones ocupada.
        positionBusy = new bool[positionCount];

        particle = GameObject.Find("Particle");
        particleWave = GameObject.Find("Wake");
        particleSmoke = GameObject.Find("Smoke");

        //Instantiate(background);
    }

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
            //transform.position = new Vector3(transform.position.x, transform.position.y,  transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
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

        if (this.isTouchingDish && !this.onCell && !currentCell.isBusy)
        {
            //cant de platos intanciados.
            FindObjectOfType<DishManager>().AmountDish--;

            //this.OnCell?.Invoke();
            this.onCell = true; //plato esta en la celda.

            UIManager.instance.PlaySoundDish();

            //ocupas la celda donde se instancio el plato.
            currentCell.isBusy = true;

            transform.position = new Vector3(0f, 0f, 0.2f);
            transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            //haces hijo de la celda al plato.
            transform.SetParent(currentCell.gameObject.transform, false);

            //particulas
            particleWave.transform.parent = this.gameObject.transform;
            particleWave.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, particleWave.transform.position.z);
            particleWave.GetComponent<ParticleSystem>().Play();

            GameObject background2 = GameObject.Find("Background 2");
            particleWave.transform.parent = background2.transform;

            particle.transform.parent = this.gameObject.transform;
            particle.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, particle.transform.position.z);
            particle.GetComponent<ParticleSystem>().Play();

            background2 = GameObject.Find("Background 2");
            particle.transform.parent = background2.transform;

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

    int sideNum = 0;
    //poder intercambiar
    void CanSwipe()
    {
        //si hay vecinos que tengan la misma porcion de torta, se puede intercambiar        
        //return GetAllNeighbors().Contains(previousSelected.gameObject);
        neighbors = GetAllNeighbors();

        foreach (GameObject item in neighbors)
        {
            if(item != null)
            {
                neighborDish = item.gameObject.GetComponent<Dish>();
                previousSelected = this.gameObject.GetComponent<Dish>();

                int num = 0;

                foreach (var itemCake in neighborDish.cakeItemList)
                {
                    if(itemCake._numCake == this.cakeItemList[num]._numCake)
                    {
                        DishConnection(sideNum);

                        //cantidad de piezas entre cada torta.
                        int aux = itemCake._pieceCount + this.cakeItemList[num]._pieceCount;

                        //chequea que los vecinos sean iguales
                        //if (neighborDish.cakeItemList.Count == 1 && this.cakeItemList.Count == 1)
                            //si es asi, cual de los dos tiene mas porciones.
                            //y para ese plato vas

                        if (this.cakeItemList[num]._allCake.Count <= cakePrefab[this.cakeItemList[num]._numCake].piece.Count)
                        {
                            //esto funciona solo con uno a uno
                            MovePiece(previousSelected, neighborDish, this.gameObject, aux, num); 
                        }

                        //una vez que se movio la porcion de torta, y se completo el plato
                        if (this.cakeItemList[num]._allCake.Count >= cakePrefab[this.cakeItemList[num]._numCake].piece.Count)
                        {
                            //se escala el plato.. (la otra es escalar el postre nomas)
                            StartCoroutine(ScaleDish(previousSelected.gameObject, startingScale, endingScale, 0.5f));

                            //y volver el plato a la normalidad.
                            StartCoroutine(ScaleDessert(previousSelected.gameObject, endingScale, startingScale, 0.5f));

                            //ir destruyendo las porciones de torta,
                            StartCoroutine(DestroyPieceDessert());

                            //giras el plato
                            //StartCoroutine(RotateDessert(0.5f));
                            if (this.cakeItemList[num]._numCake == 1 || this.cakeItemList[num]._numCake == 3)
                            {
                                //instanciar la pieza completa
                                StartCoroutine(FullDessert(previousSelected, num));
                            }

                            //particulas
                            StartCoroutine(ParticleTime());

                            //
                            CellRelease(previousSelected, neighborDish, this.gameObject, num);
                        }
                    }
                    num++;
                }
            }
            sideNum++;
        }
    }

    void CellRelease(Dish movePiece, Dish destroyPiece, GameObject gameobjectDish, int num) //liberar celda, si se completo..
    {
        if (movePiece.cakeItemList[num]._allCake.Count >= cakePrefab[movePiece.cakeItemList[num]._numCake].piece.Count)
        {
            //sumar puntos!
            GameManager.instance.Score += ReturnScore(movePiece.cakeItemList[num]._numCake);

            //sonido de torta completa.
            UIManager.instance.PlaySoundFullCake();

            //destruis el plato, una vez que se quedo sin porciones.
            if (movePiece.cakeItemList.Count <= 1)
            {
                //liberas la celda seleccionada, si es que se completo.
                gameobjectDish.transform.parent.GetComponent<Cell>().isBusy = false;

                //faltaria que aparezca la estrella, y 
                //GameObject estrellaVisual = Instantiate(estrella);
                ////estrellaVisual.transform.SetParent(gameobjectDish.transform, false);
                //estrellaVisual.transform.parent = gameobjectDish.transform;
                //estrellaVisual.transform.localPosition = estrella.transform.localPosition;


                StartCoroutine(EstrellaTime(gameobjectDish, 15f));

                Destroy(movePiece.gameObject, 2f);
            }
            else //si en un plato, se completo la torta, aunque tenga otro vecino. hay que destruir esa torta completa
            {
                //lo remueve de la lista de porcion de tortas del vecino.
                movePiece.cakeItemList.RemoveAt(num);
            }
        }
    }

    IEnumerator EstrellaTime(GameObject gameobjectDish, float duration)
    {
        yield return new WaitForSeconds(0.5f);
        //faltaria que aparezca la estrella, y 
        GameObject estrellaVisual = Instantiate(estrella);
        //estrellaVisual.transform.SetParent(gameobjectDish.transform, false);
        estrellaVisual.transform.parent = gameobjectDish.transform;
        estrellaVisual.transform.localPosition = estrella.transform.localPosition;

        //GameObject background2 = Instantiate(background);
        GameObject background2 = GameObject.Find("Background 2");
        estrellaVisual.transform.parent = background2.transform;

        yield return new WaitForSeconds(0.5f);
        float currentTime = 0.0f;
        do
        {
            estrellaVisual.transform.position = Vector3.Lerp(estrellaVisual.transform.position,
                                                                new Vector3(1.741f, 8f, -5.4f),
                                                                currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
    }

    IEnumerator ParticleTime()
    {
        yield return new WaitForSeconds(0.5f);
        particleWave.transform.parent = this.gameObject.transform;
        particleWave.transform.position = new Vector3(this.gameObject.transform.position.x,
                                                      this.gameObject.transform.position.y,
                                                      particleWave.transform.position.z);
        particleWave.GetComponent<ParticleSystem>().Play();

        GameObject background2 = GameObject.Find("Background 2");
        particleWave.transform.parent = background2.transform;

        particleSmoke.transform.parent = this.gameObject.transform;
        particleSmoke.transform.position = new Vector3(this.gameObject.transform.position.x,
                                                       this.gameObject.transform.position.y,
                                                       particle.transform.position.z);
        particleSmoke.GetComponent<ParticleSystem>().Play();

        background2 = GameObject.Find("Background 2");
        particleSmoke.transform.parent = background2.transform;
    }

    IEnumerator FullDessert(Dish movePiece, int num)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject GO = cakePrefab[movePiece.cakeItemList[num]._numCake].fullPiece;
        GameObject fullPiece = Instantiate(cakePrefab[movePiece.cakeItemList[num]._numCake].fullPiece);
        fullPiece.transform.parent = this.gameObject.transform;
        fullPiece.transform.localPosition = GO.transform.localPosition;
        fullPiece.transform.localScale = GO.transform.localScale;
    }

    IEnumerator ScaleDessert(GameObject dish, Vector3 startingScale, Vector3 endingScale, float duration)
    {
        yield return new WaitForSeconds(0.5f);
        float currentTime = 0.0f;
        do
        {
            dish.transform.localScale = Vector3.Lerp(startingScale, endingScale, currentTime / duration);
            currentTime += Time.deltaTime;
            //yield return null;
        } while (currentTime <= duration);
    }

    IEnumerator DestroyPieceDessert()
    {
        yield return new WaitForSeconds(0.8f);
        //yield return new WaitForSecond(1f);
        int numChild = previousSelected.transform.childCount;
        for (int i = 0; i < numChild; i++)
        {
            //necesito algo que encuentre todos los hijos de un prefab..
            if (previousSelected.transform.GetChild(i).gameObject.tag == "Donut" ||
                previousSelected.transform.GetChild(i).gameObject.tag == "Apple")
            {
                //encuentra al hijo del plato
                GameObject piece = previousSelected.transform.GetChild(i).gameObject;
                StartCoroutine(NotScaleDish(piece, 
                                            previousSelected.transform.GetChild(i).gameObject.transform.localScale,
                                            new Vector3(0f, 0f, 0f), 0.25f));
                Destroy(piece); //destruye las porciones.
            }
        }
    }
    IEnumerator NotScaleDish(GameObject dish, Vector3 startingScale, Vector3 endingScale, float duration)
    {
        float currentTime = 0.0f;
        dish.transform.localPosition = new Vector3(dish.transform.localPosition.x, dish.transform.localPosition.y, -0.5f);

        do
        {
            dish.transform.localScale = Vector3.Lerp(startingScale, endingScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
    }

    void DestroyCakePiece(Dish destroyPiece, int j)
    {
        int numPieceCake = destroyPiece.cakeItemList[j]._allCake.Count - 1;

        //lo remueve de la lista de porcion de tortas del vecino.
        destroyPiece.cakeItemList[j]._allCake.RemoveAt(numPieceCake);
        destroyPiece.cakeItemList[j]._pieceCount--;

        if (destroyPiece.cakeItemList[j]._allCake.Count < 1)
        {
            destroyPiece.cakeItemList.RemoveAt(j);
        }

        //esto destruye al vecino si se queda sin porcion.
        if (destroyPiece.cakeItemList.Count == 0)
        {
            //liberas la celda del vecino. si se quedo sin porciones.
            destroyPiece.transform.parent.GetComponent<Cell>().isBusy = false;

            Vector3 startingScale = new Vector3(1.3f, 1.3f, 1.3f);
            Vector3 endingScale = new Vector3(0f, 0f, 1.3f);
            //destruye el plato que se quedo sin porcion, porque se movio.
            StartCoroutine(ScaleDish(destroyPiece.gameObject, startingScale, endingScale, 0.5f));
            Destroy(destroyPiece.gameObject, 0.9f);
        }
    }


    IEnumerator ScaleRotateDish(GameObject dish, Vector3 startingScale, Vector3 endingScale, float duration)
    {
        Vector3 rotation = transform.localEulerAngles;
        float currentTime = 0.0f;

        do
        {
            rotation.z += Time.deltaTime * 360f;
            transform.localEulerAngles = rotation;
            dish.transform.localScale = Vector3.Lerp(startingScale, endingScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
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
                numScore = (int)PuntuacionPiece.Donut;
                return numScore;
            case 2:
                numScore = (int)PuntuacionPiece.Cinnamon;
                return numScore;
            case 3:
                numScore = (int)PuntuacionPiece.Apple;
                return numScore;
        }
        return numScore;
    }

    //vecinos
    Vector3[] adjecentDirections = new Vector3[]
    {
        Vector3.up, 
        Vector3.down, 
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

        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(ray);

        RaycastHit hit;
        float maxDistance = 2.5f;
        Physics.Raycast(ray, out hit, maxDistance, layerToHit);

        if (hit.collider != null)
        {
            //vecino que encontraste 
            neighborDish = hit.collider.gameObject.GetComponent<Dish>();
            GameObject neighbor = hit.collider.gameObject; //esto para poder eliminarlo. 

            if (neighborDish.currentCell != this.currentCell)
            {
                //chequea varias tipo de torta
                //CheckSeveralNeighbors(neighborDish, neighbor);

                //chequea vecinos que uno de ambos tenga una porcion de torta.
                //NeighborCheck(neighborDish, neighbor);

                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    void DishConnection(int num)
    {
        connection = Instantiate(dishConection[num]);
        connection.transform.parent = this.gameObject.transform;
        connection.transform.localPosition = dishConection[num].transform.position;
        connection.transform.localRotation = dishConection[num].transform.localRotation;
        Destroy(connection.gameObject, 0.5f);
    }

    //void CheckSeveralNeighbors(Dish neighborDish, GameObject neighbor)
    //{
    //    for (int i = 0; i < this.cakeItemList.Count; i++)
    //    {
    //        for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
    //        {
    //            //chequeas si el vecinoo tiene un porcion igual.
    //            if (this.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
    //            {
    //                if (neighborDish.cakeItemList.Count == 0 || this.cakeItemList.Count == 0)
    //                {
    //                    return; //si uno de los 2 esta vacio. No seguir.
    //                }

    //                //cant de piezas del objeto seleccionado y el vecino.
    //                int aux = this.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

    //                if (neighborDish.cakeItemList.Count >= 2 && this.cakeItemList.Count >= 2)
    //                {
    //                    //ir del vecino hacia la torta que pusiste
    //                    Dish dish = this.gameObject.GetComponent<Dish>();
    //                    Dish destroyPiece = neighborDish;
                        
    //                    MovePiece(destroyPiece, dish, neighbor, aux, i, j);
    //                }
    //            }
    //        }
    //    }
    //}

    //void NeighborCheck(Dish neighborDish, GameObject neighbor)
    //{
    //    //primero te chequeas al seleccionado.
    //    for (int i = 0; i < this.cakeItemList.Count; i++)
    //    {
    //        for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
    //        {
    //            //chequeas si el vecinoo tiene un porcion igual.
    //            if (this.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
    //            {
    //                isNeighbor = true;

    //                if(neighborDish.cakeItemList.Count == 0 || this.cakeItemList.Count == 0)
    //                {
    //                    return; //si uno de los 2 esta vacio. No seguir.
    //                }

    //                //cant de piezas del objeto seleccionado y el vecino.
    //                int aux = this.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

    //                if (neighborDish.cakeItemList.Count == 1 && this.cakeItemList.Count == 1)
    //                {
    //                    //Del vecino hacia la torta que pusiste
    //                    Dish dish = this.gameObject.GetComponent<Dish>();
    //                    Dish destroyPiece = neighborDish;

    //                    MovePiece(dish, destroyPiece, this.gameObject, aux, i, j);
    //                }
    //                else if (neighborDish.cakeItemList.Count == 1 && this.cakeItemList.Count >= 2)
    //                { 
    //                    //Desde el seleccionado al vecino.
    //                    Dish dish = neighborDish;
    //                    Dish destroyPiece = this.gameObject.GetComponent<Dish>();

    //                    MovePiece(dish, destroyPiece, neighbor, aux, i, j);                               
    //                }
    //                else if(neighborDish.cakeItemList.Count >= 2 && this.cakeItemList.Count == 1)
    //                {
    //                    //Del vecino hacia la torta que pusiste
    //                    Dish dish = this.gameObject.GetComponent<Dish>();
    //                    Dish destroyPiece = neighborDish;
                        
    //                    MovePiece(dish, destroyPiece, this.gameObject, aux, j, i);
    //                }
    //            }
    //        }
    //    }   
    //}

    IEnumerator MoveObject(GameObject piece, Vector3 currentPos, Vector3 targetPos, float duration)
    {
        piece.transform.position = Vector3.zero;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            piece.transform.localPosition = Vector3.Lerp(currentPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator RotateObject(GameObject gameObjectToMove, Quaternion currentRot, Quaternion newRot, float duration)
    {
        //gameObjectToMove.transform.localRotation = Quaternion.Euler(Vector3.zero);

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            gameObjectToMove.transform.localRotation = Quaternion.Slerp(currentRot, newRot, counter / duration);
            yield return null;
        }
    }

    void MovePiece(Dish movePiece, Dish destroyPiece, GameObject gameobjectDish, int aux, int num)
    {
        //llenar torta seleccionada.
        for (int k = movePiece.cakeItemList[num]._pieceCount; k < aux; k++)
        {
            if (!movePiece.positionBusy[k])
            {
                //encuentra al hijo, siempre el 0 es el plato. luego la cantidad de platos que haya
                GameObject piece = destroyPiece.transform.GetChild(1).gameObject;

                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                piece.transform.parent = movePiece.gameObject.transform;

                StartCoroutine(MoveObject(piece,
                                          piece.transform.localPosition,
                                          cakePrefab[movePiece.cakeItemList[num]._numCake].posOriginal[k],
                                          0.35f));

                if(movePiece.cakeItemList[num]._numCake == 3)
                {
                    piece.gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
                }

                //todo lo de rotar para apple
                if (movePiece.cakeItemList[num]._numCake == 1 || movePiece.cakeItemList[num]._numCake == 3)
                {
                    Vector3 rot1 = new Vector3(-180, 0, 0);
                    Quaternion quat1 = Quaternion.Euler(rot1);

                    Vector3 rot2 = cakePrefab[movePiece.cakeItemList[num]._numCake].rotOriginal[k];
                    Quaternion quat2 = Quaternion.Euler(rot2);

                    //cuando es apple tengo que rotarla.
                    StartCoroutine(RotateObject(piece, quat1, quat2, 0.5f));
                }

                //esto lo agrega a la lista de torta correspondiente. 
                movePiece.cakeItemList[num]._allCake.Add(piece);

                //si le instancias, tenes que sumarle.
                movePiece.cakeItemList[num]._pieceCount++;

                movePiece.positionBusy[k] = true;
            }

            //libera la celda, si la porcion de torta se completa.
            //CellRelease(movePiece, destroyPiece, gameobjectDish, num); //release=liberar        

            //destruir la porcion de torta movida.
            DestroyCakePiece(destroyPiece, num);

            //si la torta instanciada en el plato seleccionado, ya esta llena, salir.
            if (movePiece.cakeItemList[num]._allCake.Count >= cakePrefab[movePiece.cakeItemList[num]._numCake].piece.Count)
            {
                //return;
                break;
            }
        }
    }   

    IEnumerator ScaleDish(GameObject dish, Vector3 startingScale, Vector3 endingScale, float duration)
    {
        float currentTime = 0.0f;

        do
        {
            dish.transform.localScale = Vector3.Lerp(startingScale, endingScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
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
        //Debug.Log(other.gameObject);

        if (other.gameObject.layer == 10)
        {
            isTouchingDish = false;
            //currentCell = null;
        }
    }
}