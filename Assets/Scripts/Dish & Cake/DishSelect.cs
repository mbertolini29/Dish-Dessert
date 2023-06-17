using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishSelect : MonoBehaviour
{
    //static Dish selected = null;
    static DishSelect previousSelected = null;
    static DishSelect neighborDish = null;

    [Header("Lista de vecinos.")]
    public List<GameObject> neighbors;

    List<GameObject> sameNeighbors;

    //cada plato tiene 5 posiciones mixta,
    //si el plato solo tiene apple, cambia a 6, sino 4 o 2
    //
    // 3 posiciones para iniciar el plato, xq mas no entran.
    public GameObject[] posRotPiece;
    public int positionCount = 8; 
    public bool[] positionBusy;
    public Vector3[] posOriginal;
    public Vector3[] rotOriginal;

    [Header("Tipos de postres")]
    public Cake[] cakePrefab;

    public List<CakeItem> cakeItemList = new List<CakeItem>();

    [Header("Colisiones")]
    public LayerMask layerToHit;
    Ray ray;
    [SerializeField] RaycastHit[] hits;

    Vector3 mousePos;

    [Header("Drag & Drop")]
    public Vector3 posInicial = new Vector3();  
    public bool isSelected = false;
    public bool isTouchingDish = false;
    public bool onCell = false;

    [Header("Celda donde esta el plato.")]
    public Cell currentCell;

    [Header("Particle")]
    public GameObject particle;
    public GameObject particleWave;
    public GameObject particleSmoke;

    [Header("Time Click")]
    public float timer;
    public float timeBetweenClicking = 1f;
    public bool canClick = true;

    private void Awake()
    {
        particle = GameObject.Find("Particle");
        particleWave = GameObject.Find("Wake");
        particleSmoke = GameObject.Find("Smoke");

        positionBusy = new bool[positionCount];
        

    }

    //private void Update()
    //{
    //    if (!canClick)
    //    {
    //        timer += Time.deltaTime;
    //        if(timer > timeBetweenClicking)
    //        {
    //            canClick = true;
    //            timer = 0;
    //        }
    //    }        
    //}

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos();

        UIManager.instance.PlaySoundDish();

        if (!onCell)
        {
            OnVibrate(55);
        }
    }

    public void OnVibrate(int num)
    {
        Vibration.Vibrate(num);
    }

    Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDrag()
    {
        SelectDish();

        if (!onCell) // && canClick)
        {
            //esto mueve el plato.
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);
            //transform.position = new Vector3(transform.position.x, transform.position.y,  transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1.5f);
        }
    }

    void SelectDish()
    {
        isSelected = true;
        //selected = gameObject.GetComponent<Dish>();
        previousSelected = gameObject.GetComponent<DishSelect>();
    }

    void DeselectDish()
    {
        isSelected = false;
        //previousSelected = null;
    }


    List<GameObject> CheckMoreOneEqualNeighbor(List<GameObject> neighbors)
    {
        //lista para chequear si tenes mas de un vecino igual... y si no salir? como salgo de aca, para que funcione la otra?
        sameNeighbors = new List<GameObject>();

        //esto chequea que haya mas de un vecino..
        foreach (GameObject item in neighbors)
        {
            if (item != null)
            {
                neighborDish = item.gameObject.GetComponent<DishSelect>();
                previousSelected = this.gameObject.GetComponent<DishSelect>();

                for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                {
                    for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
                    {
                        if (this.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                        {
                            if (this.cakeItemList[i]._numCake != 0)
                            {
                                //sumar a una lista de vecino.. 
                                sameNeighbors.Add(item);

                                i = previousSelected.cakeItemList.Count;
                                j = neighborDish.cakeItemList.Count;

                                //chequear los vecino.. y devolver cual tiene menos cant. ?
                            }
                        }
                    }
                }
            }                
        }

        return sameNeighbors;
    }

    IEnumerator WaitMoveDessert()
    {
        yield return new WaitForSeconds(0.75f);

        CheckDessert();
    }

    void CheckDessert()
    {
        neighbors = GetAllNeighbors();

        //chequear si hay vecinos todavia.
        foreach (GameObject item in neighbors)
        {
            if (item != null)
            {
                neighborDish = item.gameObject.GetComponent<DishSelect>();
                previousSelected = this.gameObject.GetComponent<DishSelect>();

                if (previousSelected.cakeItemList.Count == 0 || neighborDish.cakeItemList.Count == 0)
                {
                    // deberia salir..
                }

                for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                {
                    for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
                    {
                        //chequeas si el vecinoo tiene un porcion igual.
                        if (previousSelected.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                        {
                            //cant de piezas del objeto seleccionado y el vecino.
                            int aux = previousSelected.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

                            // si hay 2 vecino..
                            if (previousSelected.cakeItemList.Count == 2 && neighborDish.cakeItemList.Count == 2)
                            {

                            }

                            if (previousSelected.cakeItemList.Count == 1 && neighborDish.cakeItemList.Count == 1)
                            {
                                //si es uno a uno, siempre va hacia el plato que pusiste.

                                if (previousSelected.cakeItemList[i]._pieceCount > neighborDish.cakeItemList[j]._pieceCount)
                                {
                                    //del vecino al seleccionado
                                    FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, aux, i, j);
                                }
                                else
                                {
                                    //del seleccionado al vecino.
                                    FindObjectOfType<Dish>().NeighborCheck(neighborDish, previousSelected, aux, j, i);
                                }

                                //esto encuentra un vecino con un tipo de torta (1 vs 1)
                                //FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, i, j);
                            }
                            else if (previousSelected.cakeItemList.Count == 1 && neighborDish.cakeItemList.Count >= 2)
                            {
                                //Del vecino al seleccionado.
                                FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, aux, i, j);
                            }
                            else if (previousSelected.cakeItemList.Count >= 2 && neighborDish.cakeItemList.Count == 1)
                            {
                                //del seleccionado al vecino.
                                FindObjectOfType<Dish>().NeighborCheck(neighborDish, previousSelected, aux, j, i);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {
        DeselectDish();

        if (this.isTouchingDish && !this.onCell && !currentCell.isBusy)
        {
            //para bloquear unos segundos y no pueda agarrar el plato.
            //canClick = false;

            //cant de platos intanciados.
            FindObjectOfType<DishManager>().AmountDish--;

            //this.OnCell?.Invoke();
            this.onCell = true; //plato esta en la celda.

            //ocupas la celda donde se instancio el plato.
            currentCell.isBusy = true;

            transform.position = new Vector3(0f, 0f, 0.2f);
            transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            //haces hijo de la celda al plato.
            transform.SetParent(currentCell.gameObject.transform, false);

            UIManager.instance.PlaySoundDish();
            
            //particulas para instanciarlo.
            DishParticles();

            //desactivar tutorial.
            //if (TutorialManager.instance.ReturnNumTutorial() == 2)
            //{
            //    //TutorialManager.instance.LoadTutorial();
            //}

            //Una vez que ocupa la celda. Busca si las porciones de tortas coinciden.
            
            //este busca los vecino, si hay un vecino del mismo tipo de postre,
            //lo agrega a la lista de objetos "neighbors"
            neighbors = GetAllNeighbors();

            //si hay mas de un vecino igual, te devuelve una lista de vecinos..
            sameNeighbors = CheckMoreOneEqualNeighbor(neighbors);

            if (sameNeighbors != null && sameNeighbors.Count > 1)
            {
                //chequear cual vecino tiene 2 o mas tipos de porciones.
                //elegir ese vecino, y traerlo al medio.
                CheckNeighborMoreDessert(sameNeighbors);

                //vuelve a buscar vecino..
                neighbors = GetAllNeighbors();
                //si hay mas de un vecino. hace lo mismo.
                sameNeighbors = CheckMoreOneEqualNeighbor(neighbors);
                //if (sameNeighbors != null && sameNeighbors.Count > 1)
                //{
                //    CheckNeighborMoreDessert(sameNeighbors);
                //}
                //else //chequea si hay algun vecino..
                //{
                //    //antes de moverlo, tengo que acomodarlo en el plato.
                //    StartCoroutine(WaitMoveDessert());
                //}
            }
            else //si hay menos de 1 vecino, hace esto.
            {
                //chequear si hay vecinos todavia.
                //foreach (GameObject item in neighbors)
                //{
                //    if (item != null)
                //    {
                //        neighborDish = item.gameObject.GetComponent<DishSelect>();
                //        previousSelected = this.gameObject.GetComponent<DishSelect>();
                //    }
                //}

                //chequear si hay vecinos todavia.
                GameObject _item = neighbors.Find(x => x != null);

                if (_item != null)
                {
                    neighborDish = _item.gameObject.GetComponent<DishSelect>();
                    previousSelected = this.gameObject.GetComponent<DishSelect>();

                    if (previousSelected.cakeItemList.Count == 0 || neighborDish.cakeItemList.Count == 0)
                    {
                        //deberia salir, xq no hay vecinos. 
                    }

                    // si hay 2 vecino, con 2 porciones cada uno..
                    if (previousSelected.cakeItemList.Count == 2 && neighborDish.cakeItemList.Count == 2)
                    {
                        //INTERCAMBIA AL VECINO HACIA EL SELECCIONADO.
                        string nameSame = Check2Piece(previousSelected, neighborDish);

                        int numSame = FindObjectOfType<Dish>().NumSameNeighbor(nameSame);

                        //QUEDAN EL SELECCIONADO CON 2, Y EL VECINO CON 1
                        //AUNQUE SEA IGUAL O DISTINTA.
                        //comparando cual es igual a la porcion que se encuentra sola.
                        for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                        {
                            for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
                            {
                                //ESTO FUNCIONA PARA CUANDO, TENES 2 VECINOS, Y AMBOS TIENEN PIEZA IGUALES.
                                if (previousSelected.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                                {
                                    MovePieceNeighbor(previousSelected, neighborDish, i, j);
                                    break;
                                }
                            }
                        }

                        //PERO SI NO SON IGUALES, COMO PASO. UNO AL OTRO LADO ??
                        for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                        {
                            for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
                            {
                                //PARECE QUE ENTRA SIEMPRE AL VECINO..
                                if (previousSelected.cakeItemList[i]._numCake != numSame)
                                {
                                    MovePieceDifferentNeighbor(previousSelected, neighborDish, i, j);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //ESTO CHEQUEA VECINO 1 VS 1
                        // 2 VS 1 
                        CheckDessert();
                    }
                }


                //si no pasa nada de esto. chequea vecinos nomas.
                //CheckDessert();
            }

            //una vez que el vecino que tenia menos, esta en el plato seleccionado.
            //chequear de nuevo, el seleccionado o el vecino tiene mas porciones, 
            //y el que tiene menos, va hacia el que tiene mas.


            //desp de buscar coincidencia, si todos los platos estan ocupados. GameOver
            //Grid.instance.CheckBusyCell();
        }
        else if(!this.onCell)
        {
            //sonido para cambiar de plato
            UIManager.instance.PlaySoundBusyPos();

            //sino vuelve a la posicion inicial.
            transform.position = posInicial;
        }

        //if (TutorialManager.instance.tutorial)
        //{
        //    //if(currentCell != null)
        //    //{
        //    //    if(currentCell.gameObject == TutorialManager.instance.targetCell1.gameObject)
        //    //    {
        //    //        TutorialManager.instance.numTutorial++;
        //    //        TutorialManager.instance.DesactiveHandle();
        //    //    }
        //    //}
        //}
    }

    void MovePieceDifferentNeighbor(DishSelect previousSelected, DishSelect neighborDish, int i, int j)
    {
        //cant de piezas del objeto seleccionado y el vecino.
        int aux = previousSelected.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

        //ACA SIEMPRE ENTRA EN EL VECINO, XQ ES EL QUE SE QUEDO SIN UNA PIEZA.
        if (previousSelected.cakeItemList.Count >= 2 && neighborDish.cakeItemList.Count == 1)
        {
            //del seleccionado al vecino.
            FindObjectOfType<Dish>().DifferentNeighborCheck(neighborDish, previousSelected, aux, j, i);

            //ESTO ACOMODA LA POSICIONES DEL SELECIONADO.
            int numModule = previousSelected.cakeItemList[0]._pieceCount *
                            previousSelected.cakeItemList[0]._modulesCount;

            for (int pb = 0; pb < 8; pb++)
            {
                previousSelected.positionBusy[pb] = false;
            }

            //una vez finalizado el cambio. 
            for (int p = 0; p < numModule; p++)
            {
                previousSelected.positionBusy[p] = true;
            }

            //FALTA ACOMODAR LAS POSICIONES DEL VECINO.. POR ESO SUMAMOS LA CANT DE PIEZAS Q HAY
            numModule = 0; //ESTO TIENE Q SER UNA FUNCION.
            for (int ac = 0; ac < neighborDish.cakeItemList[0]._allCake.Count; ac++)
            {
                if(neighborDish.cakeItemList[0]._allCake[ac].tag == "Cupcake")
                {
                    numModule += 4;
                }
                else if (neighborDish.cakeItemList[0]._allCake[ac].tag == "Donut")
                {
                    numModule += 2;
                }
                else if (neighborDish.cakeItemList[0]._allCake[ac].tag == "Cinnamon")
                {
                    numModule += 1;
                }
            }

            //ESTO ACOMODA LA POSICIONES DEL SELECIONADO.
            for (int pb = 0; pb < 8; pb++)
            {
                neighborDish.positionBusy[pb] = false;
            }

            //una vez finalizado el cambio. 
            for (int p = 0; p < numModule; p++)
            {
                neighborDish.positionBusy[p] = true;
            }
        }
    }

    void MovePieceNeighbor(DishSelect previousSelected, DishSelect neighborDish, int i, int j)
    {
        //cant de piezas del objeto seleccionado y el vecino.
        int aux = previousSelected.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

        if (previousSelected.cakeItemList.Count == 1 && neighborDish.cakeItemList.Count >= 2)
        {
            //Del vecino al seleccionado.
            FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, aux, i, j);

            //una vez finalizado el cambio. 

            int numModule = neighborDish.cakeItemList[0]._pieceCount *
                            neighborDish.cakeItemList[0]._modulesCount;

            for (int pb = 0; pb < 8; pb++)
            {
                neighborDish.positionBusy[pb] = false;
            }

            //una vez finalizado el cambio. 
            for (int p = 0; p < numModule; p++)
            {
                neighborDish.positionBusy[p] = true;
            }

            //previousSelected.cakeItemList[0]._busyPlace = previousSelected.cakeItemList[0]._pieceCount;
        }
        else if (previousSelected.cakeItemList.Count >= 2 && neighborDish.cakeItemList.Count == 1)
        {
            //del seleccionado al vecino.
            FindObjectOfType<Dish>().NeighborCheck(neighborDish, previousSelected, aux, j, i);

            int numModule = previousSelected.cakeItemList[0]._pieceCount *
                            previousSelected.cakeItemList[0]._modulesCount;

            for (int pb = 0; pb < 8; pb++)
            {
                previousSelected.positionBusy[pb] = false;
            }

            //una vez finalizado el cambio. 
            for (int p = 0; p < numModule; p++)
            {
                previousSelected.positionBusy[p] = true;
            }

            //previousSelected.cakeItemList[0]._busyPlace = previousSelected.cakeItemList[0]._pieceCount;
        }
    }


    string Check2Piece(DishSelect previousSelected, DishSelect neighborDish)
    {
        string namePiece = " "; 

        for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
        {
            for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
            {
                //chequeas si el vecinoo tiene un porcion igual.
                if (previousSelected.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                {
                    //cant de piezas del objeto seleccionado y el vecino.
                    int aux = previousSelected.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

                    //con una funcion deberia hacer todo. ordenarlo, y acomodarlo.

                    //del vecino al seleccionado
                    namePiece = FindObjectOfType<Dish>().CheckSeveralNeighbors(previousSelected, neighborDish, aux, i, j);

                    i = previousSelected.cakeItemList.Count;
                    j = neighborDish.cakeItemList.Count;
                }
            }
        }

        return namePiece;
    }

    void CheckNeighborMoreDessert(List<GameObject> sameNeighbors)
    {
        for (int v = 0; v < sameNeighbors.Count; v++)
        {
            if (v + 1 < sameNeighbors.Count)
            {
                DishSelect neighbordDish1 = sameNeighbors[v].gameObject.GetComponent<DishSelect>();
                DishSelect neighbordDish2 = sameNeighbors[v + 1].gameObject.GetComponent<DishSelect>();
                previousSelected = this.gameObject.GetComponent<DishSelect>();

                //el que tiene mas de un tipo de vecino
                if (neighbordDish1.cakeItemList.Count >= neighbordDish2.cakeItemList.Count)
                {
                    //se mueve del vecino al seleccionado.
                    WhichMorePiece(previousSelected, neighbordDish1);

                    //Chequear el orden.

                    SortPieces(previousSelected, neighbordDish1);

                    //break;
                }
                else if(neighbordDish1.cakeItemList.Count < neighbordDish2.cakeItemList.Count)
                {
                    //se mueve del vecino al seleccionado.
                    WhichMorePiece(previousSelected, neighbordDish2);

                    //Chequear el orden.
                    SortPieces(previousSelected, neighbordDish2);

                    //break;
                }
                //else //sino significa que tienen la misma cantidad de porciones.
                //{
                //    //esto funciona. si hay solo 2 vecinos.
                //    CheckMore(sameNeighbors);
                //}
            }
        }
    }

    void WhichMorePiece(DishSelect previousSelected, DishSelect neighbordDish)
    {
        for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
        {
            for (int j = 0; j < neighbordDish.cakeItemList.Count; j++)
            {
                //chequeas cual porcion es igual.
                if (previousSelected.cakeItemList[i]._numCake == neighbordDish.cakeItemList[j]._numCake)
                {
                    //cant de piezas del objeto seleccionado y el vecino menor
                    int aux = previousSelected.cakeItemList[i]._pieceCount +
                                 neighbordDish.cakeItemList[j]._pieceCount;

                    //del vecino al seleccionado.
                    FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighbordDish, aux, i, j);

                    //una vez que se movio, tiene que salir xq ya no tiene las mismas piezas la lista.
                    //break;
                }
            }
        }
    }

    void SortPieces(DishSelect previousSelected, DishSelect neighbordDish)
    {
        //si o si, el plato seleccionado,
        //tiene que tener mas de un tipo de torta, para tener que acomodar el orden.
        if(previousSelected.cakeItemList.Count > 1)
        {
            string namePiece = " ";

            //el primer hijo del plato seleccionado, se ignora, porque es el plato,
            if (previousSelected.transform.GetChild(1).gameObject.transform.childCount > 0)
            {
                //este nombre es el tipo de porcion, que esta primero.
                namePiece = previousSelected.transform.GetChild(1).gameObject.transform.GetChild(0).tag;

                //y ahora necesito saber en que lista esta.
                for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                {
                    //buscas cual es el contrario.

                    //como se que existe contrario ?
                    if (previousSelected.cakeItemList[i]._allCake[0].tag != namePiece)
                    {
                        int _numChild = previousSelected.transform.childCount;
                        for (int n = 2; n < _numChild; n++) //recorres y buscas, si hay un vecino igual ?
                        {
                            //el primer hijo, se ignora, porque es el plato,
                            if (previousSelected.transform.GetChild(n).gameObject.transform.childCount > 0)
                            {
                                //chequeas si el hijo siguiente. es distinto
                                if (previousSelected.transform.GetChild(n).gameObject.transform.GetChild(0).tag != namePiece)
                                {
                                    //si encontras un vecino distinto.

                                    //seguis recorriendo a partir de ese numero. a ver si hay un vecino igual al primero?
                                    for (int m = n+1; m < _numChild; m++) //recorres y buscas, si hay un vecino igual ?
                                    {
                                        //el primer hijo, se ignora, porque es el plato,
                                        if (previousSelected.transform.GetChild(m).gameObject.transform.childCount > 0)
                                        {
                                            //chequeas si el hijo siguiente. es distinto
                                            if (previousSelected.transform.GetChild(m).gameObject.transform.GetChild(0).tag == namePiece)
                                            {
                                                //si encontras un vecino igual. es porque esta desacomodado.
                                                //entonces, realizas el cambio.

                                                //primero paso distinto.
                                                SortNeighborNow(n, m);

                                                //y luego muevo al igual al lado?
                                                SortNeighborNow(m, n);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void SortNeighborNow(int n, int m)
    {
        //vecino que hay que MOVER.
        GameObject neighbor1 = previousSelected.transform.GetChild(n).gameObject.transform.GetChild(0).gameObject;

        //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
        neighbor1.transform.parent = previousSelected.transform.GetChild(m).transform;

        //este pone la posicion en cero.
        StartCoroutine(MoveObject(neighbor1, neighbor1.transform.localPosition, Vector3.zero, 0.35f));

        //tengo que poner la rotacion de Z en cero
        Quaternion quat1 = neighbor1.transform.localRotation;

        Vector3 rot2 = new Vector3(neighbor1.transform.localRotation.z, 0, 0); //creo que solo z
        Quaternion quat2 = Quaternion.Euler(rot2);

        StartCoroutine(RotateObject(neighbor1, quat1, quat2, 0.5f));
    }

    IEnumerator MoveObject(GameObject piece, Vector3 currentPos, Vector3 targetPos, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            piece.transform.localPosition = Vector3.Lerp(currentPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //antes de seguir, chequear que todo este en cero.
        //A segurar que quede en cero la posicion
        piece.transform.position = Vector3.zero;
        piece.transform.localPosition = Vector3.zero;
    }

    IEnumerator RotateObject(GameObject gameObjectToMove, Quaternion currentRot, Quaternion newRot, float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            gameObjectToMove.transform.localRotation = Quaternion.Slerp(currentRot, newRot, counter / duration);
            yield return null;
        }

        gameObjectToMove.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void CheckMore(List<GameObject> sameNeighbors)
    {
        //aca tengo que controlar cual de los 2 vecino tiene mas porciones,
        //y mover ese vecino, al seleccionado
        for (int v = 0; v < sameNeighbors.Count; v++)
        {
            if (v + 1 < sameNeighbors.Count)
            {
                DishSelect neighbordDish1 = sameNeighbors[v].gameObject.GetComponent<DishSelect>();
                DishSelect neighbordDish2 = sameNeighbors[v + 1].gameObject.GetComponent<DishSelect>();

                previousSelected = this.gameObject.GetComponent<DishSelect>();

                for (int i = 0; i < neighbordDish1.cakeItemList.Count; i++)
                {
                    for (int j = 0; j < neighbordDish2.cakeItemList.Count; j++)
                    {
                        //chequeas si el vecinoo tiene un porcion igual.
                        if (neighbordDish1.cakeItemList[i]._numCake == neighbordDish2.cakeItemList[j]._numCake)
                        {                              
                            //el que es menor, lo moves del vecino al seleccionado.
                            if (neighbordDish1.cakeItemList[i]._pieceCount <= neighbordDish2.cakeItemList[j]._pieceCount)
                            {
                                for (int k = 0; k < previousSelected.cakeItemList.Count; k++)
                                {
                                    if (previousSelected.cakeItemList[k]._numCake == neighbordDish1.cakeItemList[i]._numCake)
                                    {
                                        //cant de piezas del objeto seleccionado y el vecino menor
                                        int aux = previousSelected.cakeItemList[k]._pieceCount +
                                                    neighbordDish1.cakeItemList[i]._pieceCount;

                                        //del seleccionado al vecino
                                        FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighbordDish1, aux, k, i);

                                        //una vez que se movio, tiene que salir xq ya no tiene las mismas piezas la lista.
                                        break;
                                        //continue;
                                    }
                                }
                            }
                            else 
                            {

                            }
                        }
                    }
                }
            }
        }
    }

    #region Neighbors

    //vecinos
    Vector3[] adjecentDirections = new Vector3[]
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    public List<GameObject> GetAllNeighbors()
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
        //Ray ray;
        ray = new Ray(previousSelected.transform.position, direction);      
        //Debug.DrawRay(ray.origin, ray.direction * 10);
        //Gizmos.DrawRay(ray);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);

        foreach (RaycastHit obj in hits)
        {
            if (obj.transform.tag == "Dish" && obj.distance < 2f)
            {
                //vecino que encontraste 
                neighborDish = obj.collider.gameObject.GetComponent<DishSelect>();
                //previousSelected = this.gameObject.GetComponent<DishSelect>();

                if (neighborDish.currentCell != null)
                {
                    if (neighborDish.currentCell != this.currentCell)
                    {
                        for (int i = 0; i < previousSelected.cakeItemList.Count; i++)
                        {
                            for (int j = 0; j < neighborDish.cakeItemList.Count; j++)
                            {
                                //chequeas si el vecinoo tiene un porcion igual.
                                if (this.cakeItemList[i]._numCake == neighborDish.cakeItemList[j]._numCake)
                                {
                                    return obj.collider.gameObject;
                                }
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    #endregion

    void DishParticles()
    {
        //particula onda
        particleWave.transform.parent = this.gameObject.transform;
        particleWave.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, particleWave.transform.position.z);
        particleWave.GetComponent<ParticleSystem>().Play();

        GameObject background2 = GameObject.Find("Background 2");
        particleWave.transform.parent = background2.transform;

        //particula
        particle.transform.parent = this.gameObject.transform;
        particle.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, particle.transform.position.z);
        particle.GetComponent<ParticleSystem>().Play();

        background2 = GameObject.Find("Background 2");
        particle.transform.parent = background2.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        //LayerMask.NameToLayer("Cell")
        if (other.gameObject.layer == 9 && !onCell)
        {
            //para que el plato tmb tenga la info.
            //FindObjectOfType<Dish>().CurrentCell(other.gameObject.GetComponent<Cell>());
            
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
