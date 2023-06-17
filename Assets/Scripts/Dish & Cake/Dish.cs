using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    bool tutorialGame;

    [Header("Particle")]
    public GameObject particle;
    public GameObject particleWave;
    public GameObject particleSmoke;

    [Header("Material")] //para cambiar a apple
    public Mesh mesh;

    [Header("Estrella Puntuación")] //para cambiar a apple
    public Estrella star;    

    //se scala a mayor
    Vector3 startingScale = new Vector3(1f, 1f, 1f);
    Vector3 endingScale = new Vector3(1.1f, 1.1f, 1.1f);

    //para la union de los platos.
    public GameObject[] dishConection;
    GameObject connection;
    public bool isNeighbor = false;

    [Header("Tipos de postres")]
    public Cake[] cakePrefab;

    private void Awake()
    {
        particle = GameObject.Find("Particle");
        particleWave = GameObject.Find("Wake");
        particleSmoke = GameObject.Find("Smoke");
    }

    IEnumerator ParticleTime(DishSelect previousSelect)
    {
        yield return new WaitForSeconds(0.5f);
        particleWave.transform.parent = previousSelect.gameObject.transform;
        particleWave.transform.position = new Vector3(previousSelect.gameObject.transform.position.x,
                                                      previousSelect.gameObject.transform.position.y,
                                                      particleWave.transform.position.z);
        particleWave.GetComponent<ParticleSystem>().Play();

        GameObject background2 = GameObject.Find("Background 2");
        particleWave.transform.parent = background2.transform;

        particleSmoke.transform.parent = previousSelect.gameObject.transform;
        particleSmoke.transform.position = new Vector3(previousSelect.gameObject.transform.position.x,
                                                       previousSelect.gameObject.transform.position.y,
                                                       particle.transform.position.z);
        particleSmoke.GetComponent<ParticleSystem>().Play();

        background2 = GameObject.Find("Background 2");
        particleSmoke.transform.parent = background2.transform;
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

    //esto funcionaba, pero no active las conexiones de los platos todavía
    void DishConnection(int num) 
    {
        connection = Instantiate(dishConection[num]);
        connection.transform.parent = this.gameObject.transform;
        connection.transform.localPosition = dishConection[num].transform.position;
        connection.transform.localRotation = dishConection[num].transform.localRotation;
        Destroy(connection.gameObject, 0.5f);
    }

    //cuando haya 2 tipos de torta en cada plato, usaría esta funcion.
    public string CheckSeveralNeighbors(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        string namePiece = " ";

        if (previousSelect.cakeItemList[i]._pieceCount <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
        {
            if (aux >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
            {
                aux = cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount;
            }

            for (int k = previousSelect.cakeItemList[i]._pieceCount; k < aux; k++)
            {
                //esto funciona, pero mueve todas las porciones se mueven juntas, 
                //y en cada porcion, estaría bueno que haya una pausa
                //

                //mover y acomodar los 2 tipos de postres, 2 vs 2    
                namePiece = MoveSortDessert(previousSelect, neighborDish, aux, i, j);
            }
        }
        
        return namePiece;
    }

    public int NumSameNeighbor(string namePiece)
    {
        int numPiece = 0;
        switch (namePiece)
        {
            case "Cupcake":
                numPiece = 0;
                return numPiece;
            case "Donut":
                numPiece = 1;
                return numPiece;
            case "Cinnamon":
                numPiece = 2;
                return numPiece;
        }
        return numPiece;
    }

    /*void*/ string MoveSortDessert(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        //encontras el nombre del seleccionado, que tiene un vecino igual
        string namePiece = previousSelect.cakeItemList[i]._allCake[0].tag; //sera siempre cero??

        int numChildNeighbor = neighborDish.transform.childCount;
        for (int n = 0; n < numChildNeighbor; n++)
        {
            if (neighborDish.transform.GetChild(n).gameObject.transform.childCount > 0)
            {
                //que pasa cuando ninguno es igual? o solo uno?

                //Aca encontras a la porcion de torta que es igual en el vecino .
                if (neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).tag == namePiece)  
                {
                    //ejemplo, el vecino tiene una dona tmb => el cual deberias mover al seleccionado!
                    GameObject neighborPiece = neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).gameObject;

                    //esa dona del seleccionado, tiene que ir al vecino.
                    int numChildSelected = previousSelect.transform.childCount;
                    for (int ns = 0; ns < numChildSelected; ns++)
                    {
                        if (previousSelect.transform.GetChild(ns).gameObject.transform.childCount > 0)
                        {
                            //del seleccionado, buscas el distinto... => el cual vas a mover al vecino!
                            if (previousSelect.transform.GetChild(ns).gameObject.transform.GetChild(0).tag != namePiece) // => EN EL MOMENTO QUE YA INTERCAMBIASTE UNA PIEZA, ACA NO ESTARIA QUEDANDO NINGUN CONTRARIO/DISTINTO, Y POR LO TANTO, NO VA A A ESTAR NUNCA.         
                            {
                                //vecino que hay que MOVER.

                                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                                neighborPiece.transform.parent = previousSelect.transform.GetChild(ns).transform;

                                //este pone la posicion en cero.
                                StartCoroutine(MoveObject(neighborPiece, neighborPiece.transform.localPosition, Vector3.zero, 0.35f));

                                //tengo que poner la rotacion de Z en cero
                                Quaternion quat1 = neighborPiece.transform.localRotation;

                                Vector3 rot2 = new Vector3(neighborPiece.transform.localRotation.z, 0, 0); //creo que solo z
                                Quaternion quat2 = Quaternion.Euler(rot2);

                                StartCoroutine(RotateObject(neighborPiece, quat1, quat2, 0.5f));

                                //---- ESTO FUNCIONA. PASANDO DEL VECINO. AL SELECCIONADO.  

                                //---- COMO HAGO QUE LA SEGUNDA VUELTA. PASE DEL SELECIONADO. AL VECINO.

                                //esto puede servir, si le sacas un hijo, que no busque por demas..
                                //numChildSelected = neighborDish.transform.childCount;

                                //sonido para cambiar de plato
                                UIManager.instance.PlaySoundChangePiece();

                                //esto lo agrega a la lista de torta correspondiente. 
                                previousSelect.cakeItemList[i]._allCake.Add(neighborPiece);

                                //si le instancias, tenes que sumarle.
                                previousSelect.cakeItemList[i]._pieceCount++;
                                //previousSelect.cakeItemList[i]._busyPlace++;
                                //previousSelect.cakeItemList[i]._modulesCount++;

                                //destruir la porcion de torta movida.
                                DestroyCakePiece(previousSelect, neighborDish, i, j);
                            }
                        }
                    }
                }

            }
        }

        return namePiece;
    }

    //-------para mover piezas no iguales---------------
    public void DifferentNeighborCheck(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        //esto controla que no vayan mas del plato correspondiente.
        if (previousSelect.cakeItemList[i]._pieceCount <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
        {
            //el aux puede ser que a veces sea mayor a la cantidad maxima de piezas de esa torta?
            // aux > cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount ???

            if (aux >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
            {
                aux = cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount;
            }

            for (int k = previousSelect.cakeItemList[i]._pieceCount; k < aux; k++)
            {
                //esto funciona solo con uno a uno
                DifferentMovePiece(previousSelect, neighborDish, aux, i, j);
            }
        }

        //una vez que se movio la porcion de torta, y se completo el plato
        if (previousSelect.cakeItemList[i]._pieceCount >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
        {
            for (int c = 0; c < previousSelect.cakeItemList[i]._allCake.Count; c++)
            {
                if(c+1 < previousSelect.cakeItemList[i]._allCake.Count)
                {
                    if (previousSelect.cakeItemList[i]._allCake[c].tag == previousSelect.cakeItemList[i]._allCake[c + 1].tag)
                    {
                        //se escala el plato.. (la otra es escalar el postre nomas)
                        StartCoroutine(ScaleDish(previousSelect.gameObject, startingScale, endingScale, 0.5f));

                        //y volver el plato a la normalidad.
                        StartCoroutine(ScaleDessert(previousSelect.gameObject, endingScale, startingScale, 0.5f));

                        //particulas
                        StartCoroutine(ParticleTime(previousSelect));

                        //Liberar la celda
                        CellRelease(previousSelect, neighborDish, previousSelect.gameObject, i);
                    }
                }
            }
        }
    }

    void DifferentMovePiece(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        //encontrar al vecino. (ACA , FALLA A VECES......
        string namePiece = previousSelect.cakeItemList[i]._allCake[0].tag; //sera siempre cero??

        //bool movePiece = false;
        int numChild = neighborDish.transform.childCount;
        for (int n = 0; n < numChild; n++)
        {
            //IGNORAR LA PORCION DE TORTA QUE SE MOVIO.

            //POR ESO, UN VECINO VA A TENER MAS DE UN HIJO. 

            //ESE HIJO, ES EL QUE HAY QUE MOVER.
            if (neighborDish.transform.GetChild(n).gameObject.transform.childCount > 1)
            {
                //necesito algo que encuentre todos los hijos de un prefab..
                if (neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).tag != namePiece) // && !movePiece) 
                {
                    //encuentra al hijo del plato
                    GameObject neighborPiece = neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).gameObject;

                    bool busied = false;
                    int cantOcupados = 0;
                    int ocupados = 0;

                    //un plato tiene 8 lugares.
                    while (ocupados < previousSelect.cakeItemList[i]._busyPlace)
                    {
                        if (!previousSelect.positionBusy[cantOcupados])
                        {
                            previousSelect.positionBusy[cantOcupados] = true;

                            //si le ocupas lo lugares, tenes que sumarle el module, y busyplace
                            //previousSelect.cakeItemList[i]._busyPlace++;
                            //previousSelect.cakeItemList[i]._modulesCount++;

                            if (!busied)
                            {
                                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                                neighborPiece.transform.parent = previousSelect.transform.GetChild(cantOcupados + 1).transform;
                                busied = true;
                            }

                            ocupados++;
                        }
                        cantOcupados++;

                        if (cantOcupados >= 8)
                        {
                            break;
                        }
                    }

                    //previousSelect.cakeItemList[i]._busyPlace += ocupados;
                    //previousSelect.cakeItemList[i]._modulesCount += ocupados;

                    //movePiece = true;

                    //este pone la posicion en cero.
                    StartCoroutine(MoveObject(neighborPiece,
                                              neighborPiece.transform.localPosition,
                                              Vector3.zero,
                                              0.35f));

                    //tengo que poner la rotacion de Z en cero
                    Quaternion quat1 = neighborPiece.transform.localRotation;

                    Vector3 rot2 = new Vector3(neighborPiece.transform.localRotation.z, 0, 0); //creo que solo z
                    Quaternion quat2 = Quaternion.Euler(rot2);

                    StartCoroutine(RotateObject(neighborPiece, quat1, quat2, 0.5f));

                    //esto puede servir, si le sacas un hijo, que no busque por demas..
                    numChild = neighborDish.transform.childCount;

                    //sonido para cambiar de plato
                    UIManager.instance.PlaySoundChangePiece();

                    //esto lo agrega a la lista de torta correspondiente. 
                    previousSelect.cakeItemList[i]._allCake.Add(neighborPiece);

                    //si le instancias, tenes que sumarle.
                    previousSelect.cakeItemList[i]._pieceCount++;
                    //previousSelect.cakeItemList[i]._busyPlace++;
                    //previousSelect.cakeItemList[i]._modulesCount++;

                    //Habria que sumarle el busy place, y el module count??

                    //previousSelect.positionBusy[k] = true;

                    //break; //esto funciona pero para mi es xq tiene un solo hijo. con 2 se romperia.

                }
            }
        }

        //destruir la porcion de torta movida.
        DestroyCakePiece(previousSelect, neighborDish, i, j);
    }

    //----------------------

    public void NeighborCheck(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        //esto controla que no vayan mas del plato correspondiente.
        if (previousSelect.cakeItemList[i]._pieceCount <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
        {
            //el aux puede ser que a veces sea mayor a la cantidad maxima de piezas de esa torta?
            // aux > cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount ???

            if (aux >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
            {
                aux = cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount;
            }

            for (int k = previousSelect.cakeItemList[i]._pieceCount; k < aux; k++)
            {
                //esto funciona, pero mueve todas las porciones se mueven juntas, 
                //y en cada porcion, estaría bueno que haya una pausa
                
                //esto funciona solo con uno a uno
                MovePiece(previousSelect, neighborDish, aux, i, j);
            }
        }

        //entonces, aca hay que agregar una pausa.
        //
        //sacaralo de aca seguro. xq a veces funciona bien y otras veces no.

        //una vez que se movio la porcion de torta, y se completo el plato
        if (previousSelect.cakeItemList[i]._pieceCount >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piecesCount)
        {
            //se escala el plato.. (la otra es escalar el postre nomas)
            StartCoroutine(ScaleDish(previousSelect.gameObject, startingScale, endingScale, 0.5f));

            //y volver el plato a la normalidad.
            StartCoroutine(ScaleDessert(previousSelect.gameObject, endingScale, startingScale, 0.5f));

            //ir destruyendo las porciones de torta,
            //StartCoroutine(DestroyPieceDessert(previousSelect));

            //giras el plato
            //StartCoroutine(RotateDessert(0.5f));
            //if (previousSelect.cakeItemList[i]._numCake == 1 || previousSelect.cakeItemList[i]._numCake == 3)
            //{
            //    //instanciar la pieza completa
            //    StartCoroutine(FullDessert(previousSelect, i));
            //}

            //particulas
            StartCoroutine(ParticleTime(previousSelect));

            //Liberar la celda
            CellRelease(previousSelect, neighborDish, previousSelect.gameObject, i);
        }
    }


    //    //cant de piezas del objeto seleccionado y el vecino.
    //    int _aux = previousSelect.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

    //    if (neighborDish.cakeItemList.Count == 1 && previousSelect.cakeItemList.Count == 1)
    //    {

    //    }
    //    else if (neighborDish.cakeItemList.Count == 1 && previousSelect.cakeItemList.Count >= 2) //para encontrar mas de un tipo de torta.
    //    {
    //        //Desde el seleccionado al vecino.
    //        if (previousSelect.cakeItemList[i]._allCake.Count <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piece.Count)
    //        {
    //            //esto funciona solo con uno a uno
    //            MovePiece(neighborDish, previousSelect, aux, i);
    //        }
    //    }
    //    else if (neighborDish.cakeItemList.Count >= 2 && previousSelect.cakeItemList.Count == 1)
    //    {
    //        //Del vecino hacia la torta que pusiste
    //        if (previousSelect.cakeItemList[i]._allCake.Count <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piece.Count)
    //        {
    //            //esto funciona solo con uno a uno
    //            MovePiece(previousSelect, neighborDish, aux, i);
    //        }
    //    }
    //}

    void CellRelease(DishSelect movePiece, DishSelect destroyPiece, GameObject gameobjectDish, int num) //liberar celda, si se completo..
    {
        if (movePiece.cakeItemList[num]._allCake.Count >= cakePrefab[movePiece.cakeItemList[num]._numCake].piecesCount)
        {
            //destruis el plato, una vez que se quedo sin porciones.
            if (movePiece.cakeItemList.Count <= 1)
            {
                //liberas la celda seleccionada, si es que se completo.                
                gameobjectDish.transform.parent.GetComponent<Cell>().isBusy = false;

                //sonido de torta completa.
                UIManager.instance.PlaySoundFullCake();

                //una vez completo elimina el plato.
                Destroy(movePiece.gameObject, 1f);

                //funcion para saber, cuantos platos de cada tipo esta destruyendo??
                //como evitar el tutorial ? 

                //la primer vuelta, siempre sigue en tutorial..
                //por lo tanto, usamos ese booleano.
                //para que la proxima vuelta entre..
                if (!TutorialManager.instance.tutorialGame)
                {
                    //empieza a sumar la cantidad de postres 
                    AmountTypesDessert(movePiece);

                    //tenes que informarle al usuario que actividad realizar..
                    if (GameManager.instance.NumLevel == 0)
                    {
                        GameManager.instance.NumLevel++;
                    }
                }

                //si todavia sigue en el tutoria, 
                if (GameManager.instance.NumLevel == 0)
                {
                    TutorialManager.instance.tutorialGame = false;
                }

                //Vibra el celular.
                OnVibrate(110);

                //movimiento de la estrella
                star = FindObjectOfType<Estrella>();
                star.StartStarMove(gameobjectDish.transform.position, () =>
                {
                    //sumar puntos!
                    GameManager.instance.Score += ReturnScore(movePiece.cakeItemList[num]._numCake);

                });
            }
            else //si en un plato, se completo la torta, aunque tenga otro vecino. hay que destruir esa torta completa
            {
                //lo remueve de la lista de porcion de tortas del vecino.
                movePiece.cakeItemList.RemoveAt(num);
            }
        }
    }

    void AmountTypesDessert(DishSelect movePiece)
    {
        //encontrar que tipo de torta se completo.
        //y subar uno
        //if (TutorialManager.instance.tutorial) //no se porque, pero en este entra al tutorial.

        //evitar el primer nivel, etapa,
        string nameDessert = movePiece.cakeItemList[0]._allCake[0].gameObject.tag;

        switch (nameDessert)
        {
            case "Cupcake":
                //sumas uno
                GameManager.instance.numCupcakes++;
                GameManager.instance.CheckLevels();
                break;
            case "Donut":
                GameManager.instance.numDonut++;
                GameManager.instance.CheckLevels();
                break;
        }

    }

    public void OnVibrate(int num)
    {
        Vibration.Vibrate(num);
    }

    IEnumerator MoveObject(GameObject piece, Vector3 currentPos, Vector3 targetPos, float duration)
    {
        //piece.transform.position = Vector3.zero;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            piece.transform.localPosition = Vector3.Lerp(currentPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //timeElapsed = 1f;
        //chequear que toda la posicion esten en cero?

        //antes de seguir, chequear que todo este en cero.
        //A segurar que quede en cero la posicion
        piece.transform.position = Vector3.zero;
        piece.transform.localPosition = Vector3.zero;
        //neighborDish.transform.localRotation = Quaternion.Euler(Vector3.zero);


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

        //gameObjectToMove.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void MovePiece(DishSelect previousSelect, DishSelect neighborDish, int aux, int i, int j)
    {
        //encontrar al vecino. (ACA , FALLA A VECES......
        string namePiece = previousSelect.cakeItemList[i]._allCake[0].tag; //sera siempre cero??

        //bool movePiece = false;
        int numChild = neighborDish.transform.childCount;
        for (int n = 0; n < numChild; n++)
        {
            if (neighborDish.transform.GetChild(n).gameObject.transform.childCount > 0)
            {
                //necesito algo que encuentre todos los hijos de un prefab..
                if (neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).tag == namePiece) // && !movePiece) 
                {
                    //encuentra al hijo del plato
                    GameObject neighborPiece = neighborDish.transform.GetChild(n).gameObject.transform.GetChild(0).gameObject;

                    bool busied = false;
                    int cantOcupados = 0;
                    int ocupados = 0;

                    //un plato tiene 8 lugares.
                    while (ocupados < previousSelect.cakeItemList[i]._busyPlace)
                    {
                        if (!previousSelect.positionBusy[cantOcupados])
                        {
                            previousSelect.positionBusy[cantOcupados] = true;

                            //si le ocupas lo lugares, tenes que sumarle el module, y busyplace
                            //previousSelect.cakeItemList[i]._busyPlace++;
                            //previousSelect.cakeItemList[i]._modulesCount++;

                            if (!busied)
                            {
                                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                                neighborPiece.transform.parent = previousSelect.transform.GetChild(cantOcupados + 1).transform;
                                busied = true;
                            }

                            ocupados++;
                        }
                        cantOcupados++;

                        if (cantOcupados >= 8)
                        {
                            break;
                        }
                    }

                    //previousSelect.cakeItemList[i]._busyPlace += ocupados;
                    //previousSelect.cakeItemList[i]._modulesCount += ocupados;

                    //movePiece = true;

                    //este pone la posicion en cero.
                    StartCoroutine(MoveObject(neighborPiece,
                                              neighborPiece.transform.localPosition,
                                              Vector3.zero,
                                              0.35f));

                    //if(numTorta == 2)
                    //{
                    //    neighborPiece.transform.localPosition = new Vector3(0f, 0f, 0.4f);
                    //}

                    //rotacion.
                    Quaternion quat1 = neighborPiece.transform.localRotation;

                    int numTorta = NumSameNeighbor(namePiece);
                    Quaternion quat2 = Quaternion.Euler(neighborDish.cakePrefab[numTorta].rotation);

                    StartCoroutine(RotateObject(neighborPiece, quat1, quat2, 0.5f));

                    //esto puede servir, si le sacas un hijo, que no busque por demas..
                    numChild = neighborDish.transform.childCount;

                    //sonido para cambiar de plato
                    UIManager.instance.PlaySoundChangePiece();

                    //esto lo agrega a la lista de torta correspondiente. 
                    previousSelect.cakeItemList[i]._allCake.Add(neighborPiece);

                    //si le instancias, tenes que sumarle.
                    previousSelect.cakeItemList[i]._pieceCount++;
                    //previousSelect.cakeItemList[i]._busyPlace++;
                    //previousSelect.cakeItemList[i]._modulesCount++;

                    //Habria que sumarle el busy place, y el module count??

                    //previousSelect.positionBusy[k] = true;

                    //break; //esto funciona pero para mi es xq tiene un solo hijo. con 2 se romperia.

                }
            }
        }

        //destruir la porcion de torta movida.
        DestroyCakePiece(previousSelect, neighborDish, i, j);
    }

    void DestroyCakePiece(DishSelect moveSelect, DishSelect destroyPiece, int i, int j)
    {
        int numPieceCake = destroyPiece.cakeItemList[j]._allCake.Count - 1;

        //lo remueve de la lista de porcion de tortas del vecino.
        destroyPiece.cakeItemList[j]._allCake.RemoveAt(numPieceCake);
        destroyPiece.cakeItemList[j]._pieceCount--;

        //int numPlace = destroyPiece.cakeItemList[j]._filledPlace + destroyPiece.cakeItemList[j]._busyPlace;
        for (int d = destroyPiece.cakeItemList[j]._filledPlace; d < destroyPiece.cakeItemList[j]._busyPlace; d++)
        {
            destroyPiece.positionBusy[d] = false;
        }

        //elimina la pieza de la lista.
        if (destroyPiece.cakeItemList[j]._allCake.Count < 1)
        {
            if (j + 1 < destroyPiece.cakeItemList.Count)
            {
                //pero antes de destruirla. Hay que vaciar los lugar.
                destroyPiece.cakeItemList[j + 1]._busyPlace -= destroyPiece.cakeItemList[j]._busyPlace;
                //destroyPiece.cakeItemList[j + 1]._filledPlace = destroyPiece.cakeItemList[j + 1]._busyPlace;
            }

            //destruye la torta que se movio.
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
        else
        {
            //acomodar la porcion de torta en la posicion 0 
            for (int m = 0; m < destroyPiece.positionBusy.Length; m++)
            {
                //var aux = destroyPiece.positionBusy[m+1];
                if (m + 1 < 8)
                {
                    if (!destroyPiece.positionBusy[m] && destroyPiece.positionBusy[m + 1])
                    {
                        //mover a la posicion 0
                        MoveToZeroPosition(destroyPiece, 0); //por ahora sera siempre cero ==> j ??

                        //acomodar la porcion de torta.
                        //SortPieces(destroyPiece, j);
                    }
                }
            }
        }
    }

    //acomodar la porcion de torta en la posicion 0
    void MoveToZeroPosition(DishSelect destroyPiece, int j)
    {
        //por el momento, j siempre deberia ser 0

        //encontrar al vecino.
        string namePiece = destroyPiece.cakeItemList[j]._allCake[0].tag; //_allcake => es siempre cero?
                                                                         //bool movePiece = false;
        int numChild = destroyPiece.transform.childCount;
        for (int n = 0; n < numChild; n++)
        {
            if (destroyPiece.transform.GetChild(n).gameObject.transform.childCount > 0)
            {
                //necesito algo que encuentre todos los hijos de un prefab..
                if (destroyPiece.transform.GetChild(n).gameObject.transform.GetChild(0).tag == namePiece) // && !movePiece) 
                {
                    //encuentra al hijo del plato
                    GameObject neighborPiece = destroyPiece.transform.GetChild(n).gameObject.transform.GetChild(0).gameObject;

                    bool busied = false;
                    int cantOcupados = 0;
                    int ocupados = 0;

                    //falta desocupar todo.
                    for (int b = 0; b < destroyPiece.positionBusy.Length; b++)
                    {
                        destroyPiece.positionBusy[b] = false;
                    }

                    //Esto ocupa los nuevos lugares.
                    while (ocupados < destroyPiece.cakeItemList[j]._busyPlace)
                    {
                        if (!destroyPiece.positionBusy[cantOcupados])
                        {
                            destroyPiece.positionBusy[cantOcupados] = true;

                            if (!busied)
                            {
                                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                                neighborPiece.transform.parent = destroyPiece.transform.GetChild(cantOcupados + 1).transform;
                                busied = true;
                            }

                            ocupados++;
                        }
                        cantOcupados++;

                        if (cantOcupados >= 8)
                        {
                            break;
                        }
                    }

                    //destroyPiece.cakeItemList[j]._busyPlace = ocupados;
                    //destroyPiece.cakeItemList[j]._modulesCount = ocupados;

                    //movePiece = true;

                    //este pone la posicion en cero.
                    StartCoroutine(MoveObject(neighborPiece,
                                              neighborPiece.transform.localPosition,
                                              Vector3.zero,
                                              0.35f));

                    //tengo que poner la rotacion de Z en cero
                    //Quaternion quat1 = neighborPiece.transform.localRotation

                    //Vector3 rot2 = new Vector3(neighborPiece.transform.localRotation.z, 0, 0); //creo que solo z
                    //Quaternion quat2 = Quaternion.Euler(rot2);

                    //if (numTorta == 2)
                    //{
                    //    neighborPiece.transform.localPosition = new Vector3(0f, 0f, 0.4f);
                    //}

                    //rotacion.
                    Quaternion quat1 = neighborPiece.transform.localRotation;

                    int numTorta = NumSameNeighbor(namePiece);
                    Quaternion quat2 = Quaternion.Euler(destroyPiece.cakePrefab[numTorta].rotation);

                    StartCoroutine(RotateObject(neighborPiece, quat1, quat2, 0.5f));
                }
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
}