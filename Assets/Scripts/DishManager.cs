using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager instance;

    [SerializeField] Dish dishPrefab;

    //lista total de platos instanciados.
    public List<Dish> dishes; 

    //posicion de platos de la parte inferior
    public Transform[] posDishes;
    //public int amountDish = 4;

    private void Awake()
    {
        instance = this;
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
            //instancias cada plato
            Dish dish = Instantiate<Dish>(dishPrefab);

            //los haces hijo para mejor orden
            dish.transform.SetParent(transform, false);
            
            //posicion del plato
            dish.transform.position = posDishes[i].position;
            dish.posInicial = dish.transform.position;

            //
            dish.CreatedCake();

            //guardas el postre
            dishes.Add(dish);
        }
    }
}
