using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager instance;

    [SerializeField] Dish dishPrefab;

    //Dish dish;

    //lista total de platos instanciados.
    public List<Dish> dishes;

    //es buena que las tortas que se pueden instanciar, esten aca.
    //cake prefab ?

    //posicion de platos de la parte inferior
    public Transform[] posDishes;

    [SerializeField] int amountDish = 0;

    public int AmountDish
    {
        get { return amountDish; }
        set
        {
            amountDish = value;

            if(amountDish <= 0)
            {
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
        //tenes que instanciar 4 platos
        CreateDish();    
    }

    void CreateDish()
    {
        for (int i = 0; i < posDishes.Length; i++)
        {
            amountDish++;

            //instancias cada plato
            Dish dish = Instantiate<Dish>(dishPrefab);


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

