using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager instance;

    //Dish dish;
    [SerializeField] DishSelect dishPrefab;

    //lista total de platos instanciados.
    public List<DishSelect> dishes;

    //es buena que las tortas que se pueden instanciar, esten aca.
    //cake prefab ?

    //posicion de platos de la parte inferior
    public Transform[] posDishes;

    int dishCount = 2;

    [SerializeField] int amountDish = 0;

    public int AmountDish
    {
        get { return amountDish; }
        set
        {
            amountDish = value;

            if(amountDish <= 0)
            {
                //ver como agregar el siguiente paso..
                //agregar 3 platos.. y donas junto con cupcake..
                //if(dishCount <= 4)
                //{
                //    dishCount++;
                //}

                CreateDish();
            }
        }            
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(sharedInstance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //tutorial

        //arrancar con 2 platos, y 2 cupcake.
        //CreateDishTutorial();

        //FindObjectOfType<TutorialManager>().LoadTutorial();

        //tenes que instanciar 4 platos
        CreateDish();    
    }

    void CreateDishTutorial()
    {
        //solo 2 platos para arrancar..
        for (int i = 1; i < 3; i++)
        {
            amountDish++;

            //instancias cada plato
            DishSelect dish = Instantiate<DishSelect>(dishPrefab);

            dish.name = string.Format("Dish {0}", i);

            //sonido de intanciar los platos en la mesa.
            UIManager.instance.PlaySoundInstance();

            //animacion de entrada del plato.
            StartCoroutine(MoveDish(dish.gameObject,
                                    dish.gameObject.transform.position,
                                    posDishes[i].position,
                                    0.5f));

            //los haces hijo para mejor orden
            dish.transform.SetParent(transform, false);

            //posicion del plato
            dish.posInicial = posDishes[i].position;

            Cake cake = new Cake();
            cake.CreateTutorial();

            //guardas el plato con el postre.
            dishes.Add(dish);
        }
    }

    void CreateDish()
    {
        //for (int i = 0; i < dishCount; i++)
        for (int i = 0; i < posDishes.Length; i++)
        {
            amountDish++;

            //instancias cada plato
            DishSelect dish = Instantiate<DishSelect>(dishPrefab);

            //sonido de intanciar los platos en la mesa.
            UIManager.instance.PlaySoundInstance();

            //animacion de entrada del plato.
            StartCoroutine(MoveDish(dish.gameObject,
                                    dish.gameObject.transform.position,
                                    posDishes[i].position,
                                    0.5f));

            //sonido.            

            //los haces hijo para mejor orden
            dish.transform.SetParent(transform, false);

            //posicion del plato
            //dish.transform.position = posDishes[i].position;
            //dish.posInicial = dish.transform.position;
            dish.posInicial = posDishes[i].position; 

            //
            //dish.CreatedCake();

            Cake cake = new Cake();
            cake.Create();

            //FindObjectOfType<Cake>().Create();

            //guardas el plato con el postre.
            dishes.Add(dish);
        }
    }

    IEnumerator MoveDish(GameObject dish, Vector3 currentPos, Vector3 targetPos, float duration)
    {
        yield return new WaitForSeconds(0.5f);

        //dish.transform.position = Vector3.zero;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            dish.transform.position = Vector3.Lerp(currentPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }
}

