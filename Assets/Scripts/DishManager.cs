using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishManager : MonoBehaviour
{
    public static DishManager instance;

    [SerializeField] Dish dishPrefab;

    public List<Dish> dishes;
    public Transform[] posDishes;
    //public int amountDish = 4;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //tenes que instanciar 4 platos
        //dish = new Dish();

        for (int i = 0; i < posDishes.Length; i++)
        {
            //instancias cada plato
            Dish dish = Instantiate<Dish>(dishPrefab);

            //en su posicion, y los haces hijo para mejor orden
            dish.transform.SetParent(transform, false);
            dish.transform.position = posDishes[i].position;

            dish.CreatedCake();

            //guardas el postre
            dishes.Add(dish);
        }
        
        //en sus posiciones correctas.

        //esos platos tiene que elegir, que tipo de postre quieren

        //y cuando elegis el tipo de postre, tenes que elegir la cant de porciones.

        //luego instanciarlo

        //en orden a los platos, 1, 2, 3, 4

        //luego cada plato, tiene su drag and drog, para mover, y empezar a jugar.

        // cada celda de la grilla tiene su posicion, y su lugar para que el plato quede.
    }
}
