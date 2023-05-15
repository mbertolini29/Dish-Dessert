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

    IEnumerator FullDessert(DishSelect movePiece, int num)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject GO = cakePrefab[movePiece.cakeItemList[num]._numCake].fullPiece;
        GameObject fullPiece = Instantiate(cakePrefab[movePiece.cakeItemList[num]._numCake].fullPiece);
        fullPiece.transform.parent = movePiece.gameObject.transform;
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

    IEnumerator DestroyPieceDessert(DishSelect previousSelect)
    {
        yield return new WaitForSeconds(0.8f);
        //yield return new WaitForSecond(1f);
        int numChild = previousSelect.transform.childCount;
        for (int i = 0; i < numChild; i++)
        {
            //necesito algo que encuentre todos los hijos de un prefab..
            if (previousSelect.transform.GetChild(i).gameObject.tag == "Donut" ||
                previousSelect.transform.GetChild(i).gameObject.tag == "Apple")
            {
                //encuentra al hijo del plato
                GameObject piece = previousSelect.transform.GetChild(i).gameObject;
                StartCoroutine(NotScaleDish(piece,
                                            previousSelect.transform.GetChild(i).gameObject.transform.localScale,
                                            new Vector3(0f, 0f, 0f), 0.25f));

                //Destroy(piece, 1f); //destruye las porciones.
            }
        }
    }

    IEnumerator NotScaleDish(GameObject dish, Vector3 startingScale, Vector3 endingScale, float duration)
    {
        float currentTime = 0.0f;
        dish.transform.localPosition = new Vector3(dish.transform.localPosition.x, dish.transform.localPosition.y, -0.5f);

        do
        {
            if(dish == null)
            {
                break;
            }
            dish.transform.localScale = Vector3.Lerp(startingScale, endingScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;

        } while (currentTime <= duration);
    }

    void DestroyCakePiece(DishSelect destroyPiece, int j)
    {
        int numPieceCake = destroyPiece.cakeItemList[j]._allCake.Count - 1;

        //lo remueve de la lista de porcion de tortas del vecino.
        destroyPiece.cakeItemList[j]._allCake.RemoveAt(numPieceCake);
        destroyPiece.cakeItemList[j]._pieceCount--;

        destroyPiece.positionBusy[numPieceCake] = false;

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

    public void NeighborCheck(DishSelect previousSelect, DishSelect neighborDish, int i, int j)
    {
        //cant de piezas del objeto seleccionado y el vecino.
        int aux = previousSelect.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

        if (neighborDish.cakeItemList.Count == 1 && previousSelect.cakeItemList.Count == 1)
        {
            if (previousSelect.cakeItemList[i]._allCake.Count <= cakePrefab[previousSelect.cakeItemList[i]._numCake].piece.Count)
            {
                //esto funciona solo con uno a uno
                MovePiece(previousSelect, neighborDish, aux, i);
            }

            //una vez que se movio la porcion de torta, y se completo el plato
            if (previousSelect.cakeItemList[i]._allCake.Count >= cakePrefab[previousSelect.cakeItemList[i]._numCake].piece.Count)
            {
                //se escala el plato.. (la otra es escalar el postre nomas)
                StartCoroutine(ScaleDish(previousSelect.gameObject, startingScale, endingScale, 0.5f));

                //y volver el plato a la normalidad.
                StartCoroutine(ScaleDessert(previousSelect.gameObject, endingScale, startingScale, 0.5f));

                //ir destruyendo las porciones de torta,
                StartCoroutine(DestroyPieceDessert(previousSelect));

                //giras el plato
                //StartCoroutine(RotateDessert(0.5f));
                if (previousSelect.cakeItemList[i]._numCake == 1 || previousSelect.cakeItemList[i]._numCake == 3)
                {
                    //instanciar la pieza completa
                    StartCoroutine(FullDessert(previousSelect, i));
                }

                //particulas
                StartCoroutine(ParticleTime(previousSelect));

                //Liberar la celda
                CellRelease(previousSelect, neighborDish, previousSelect.gameObject, i);
            }
        }
        //para encontrar mas de un tipo de torta.
        //else if (neighborDish.cakeItemList.Count == 1 && this.cakeItemList.Count >= 2)
        //{
        //    //Desde el seleccionado al vecino.
        //    Dish dish = neighborDish;
        //    Dish destroyPiece = this.gameObject.GetComponent<Dish>();

        //    MovePiece(dish, destroyPiece, neighbor, aux, i, j);
        //}
        //else if (neighborDish.cakeItemList.Count >= 2 && this.cakeItemList.Count == 1)
        //{
        //    //Del vecino hacia la torta que pusiste
        //    Dish dish = this.gameObject.GetComponent<Dish>();
        //    Dish destroyPiece = neighborDish;

        //    MovePiece(dish, destroyPiece, this.gameObject, aux, j, i);
        //}
    }

    void CellRelease(DishSelect movePiece, DishSelect destroyPiece, GameObject gameobjectDish, int num) //liberar celda, si se completo..
    {
        if (movePiece.cakeItemList[num]._allCake.Count >= cakePrefab[movePiece.cakeItemList[num]._numCake].piece.Count)
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

    public void OnVibrate(int num)
    {
        Vibration.Vibrate(num);
    }

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

    void MovePiece(DishSelect movePiece, DishSelect destroyPiece, int aux, int num)
    {
        //llenar torta seleccionada.
        for (int k = movePiece.cakeItemList[num]._pieceCount; k < aux; k++)
        {
            //si la torta instanciada en el plato seleccionado, ya esta llena, salir.            
            if (movePiece.cakeItemList[num]._allCake.Count >= cakePrefab[movePiece.cakeItemList[num]._numCake].piece.Count)
            {
                //return;
                break;
            }

            if (!movePiece.positionBusy[k]) // && movePiece.positionBusy[k] != null)
            {
                //encuentra al hijo, siempre el 0 es el plato. luego la cantidad de platos que haya
                GameObject piece = destroyPiece.transform.GetChild(1).gameObject;

                //mueve la porcion de torta del plato a destruir, hacia el plato que deseas.
                piece.transform.parent = movePiece.gameObject.transform;

                StartCoroutine(MoveObject(piece,
                                          piece.transform.localPosition,
                                          cakePrefab[movePiece.cakeItemList[num]._numCake].posOriginal[k],
                                          0.35f));

                //sonido para cambiar de plato
                UIManager.instance.PlaySoundChangePiece();

                if (movePiece.cakeItemList[num]._numCake == 3)
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