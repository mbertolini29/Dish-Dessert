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
            if (TutorialManager.instance.ReturnNumTutorial() == 2)
            {
                //TutorialManager.instance.LoadTutorial();
            }

            //Una vez que ocupa la celda. Busca si las porciones de tortas coinciden.
            //CanSwipe();
            //FindObjectOfType<Dish>().CanSwipe();
            neighbors = GetAllNeighbors();

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

        if (TutorialManager.instance.tutorial)
        {
            //if(currentCell != null)
            //{
            //    if(currentCell.gameObject == TutorialManager.instance.targetCell1.gameObject)
            //    {
            //        TutorialManager.instance.numTutorial++;
            //        TutorialManager.instance.DesactiveHandle();
            //    }
            //}
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
                                    //chequea varias tipo de torta
                                    //CheckSeveralNeighbors(neighborDish, neighbor);

                                    //chequea vecinos que uno de ambos tenga una porcion de torta.
                                    //NeighborCheck(neighborDish);

                                    //desactivar el tutorial..

                                    if (this.cakeItemList.Count == 0 || neighborDish.cakeItemList.Count == 0)
                                    {
                                        // deberia salir..
                                    }

                                    //cant de piezas del objeto seleccionado y el vecino.
                                    int aux = this.cakeItemList[i]._pieceCount + neighborDish.cakeItemList[j]._pieceCount;

                                    if (this.cakeItemList.Count == 1 && neighborDish.cakeItemList.Count == 1)
                                    {
                                        //esto encuentra un vecino con un tipo de torta (1 vs 1)
                                        //FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, i, j);
                                        FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, aux, i, j);
                                    }
                                    else if (this.cakeItemList.Count == 1 && neighborDish.cakeItemList.Count >= 2)
                                    {
                                        //Del vecino al seleccionado.
                                        FindObjectOfType<Dish>().NeighborCheck(previousSelected, neighborDish, aux, i, j);
                                    }
                                    else if (this.cakeItemList.Count >= 2 && neighborDish.cakeItemList.Count == 1)
                                    {
                                        //Del vecino al seleccionado.
                                        FindObjectOfType<Dish>().NeighborCheck(neighborDish, previousSelected, aux, j, i);
                                    }

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
