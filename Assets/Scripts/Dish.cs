using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    //cant de piezas
    [SerializeField] int numCake;
    [SerializeField] int amountPiece; 

    //tipo de postre
    [SerializeField] Cake[] cakePrefab;

    //lista de postres a instanciar.
    //public List<GameObject> createdDesserts;

    public List<GameObject> createdCake;

    private void Start()
    {
        //CreateDessert();
    }

    public void CreatedCake() //crea el postre.
    {
        //elije el postre q quiere instanciar, al azar
        //UnityEngine.Random.Range(0, 2);
        numCake = UnityEngine.Random.Range(0, Enum.GetValues(typeof(typeCake)).Length);

        //cuantas piezas
        //numPiece = UnityEngine.Random.Range(0, Enum.GetValues(typeof(AmountPiece)).Length);
        switch (numCake)
        {
            case 0:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Cupcake);
                break;
            case 1:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Rosquilla);
                break;
            case 2:
                amountPiece = UnityEngine.Random.Range(1, (int)AmountPiece.Canela);
                break;
        }

        ////cada plato tiene que tener su lista de postres
        createdCake = new List<GameObject>();

        for (int i = 0; i < amountPiece; i++)
        {
            //instanciar cada postre en su plato



            //Cake cake = Instantiate(cakePrefab[numCake]);

            GameObject pieceCake = Instantiate<GameObject>(cakePrefab[numCake].pieceCake[i]);

            pieceCake.transform.SetParent(transform, false);
            pieceCake.transform.position = new Vector3(transform.position.x,
                                                       pieceCake.transform.position.y, 
                                                       transform.position.z);
            createdCake.Add(pieceCake);
        }


        ////y lo instancia, por cada plato tal postre.
        //foreach (var item in DishManager.instance.dishes)
        //{
        //    //obtener el postre
        //    //Dessert postre1 = item.dessertPrefab[item.numDessert];

        //    //cada plato tiene que tener su lista de postres
        //    createdDesserts = new List<GameObject>();

        //    //podria hacer una lista de postres.
        //    for (int i = 0; i < item.amountPiece; i++)
        //    {

        //        //instanciar cada postre en su plato
        //        //Dessert dessert = Instantiate<Dessert>(item.dessertPrefab[item.numDessert].desserts[i]);
        //        GameObject dessert = item.dessertPrefab[item.numDessert].desserts[i];

        //        this.transform.SetParent(transform, false);
        //        //dessert.transform.position = 

        //        Instantiate(dessert);

        //        createdDesserts.Add(dessert);



        //        //Dish dish = Instantiate<Dish>(dishPrefab);

        //        ////en su posicion, y los haces hijo para mejor orden
        //        //dish.transform.SetParent(transform, false);
        //        //dish.transform.position = posDishes[i].position;

        //        //postre1.desserts[i]                    
        //    }


        //obtenes la cant de porciones

        //instanciar el postre en su plato correspondiente




    }


}
