using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    //cant de piezas
    [SerializeField] int randomNum;

    //tipo de postre
    [SerializeField] Dessert[] dessert;

    private void Start()
    {
        //dessert = new Dessert[numAletorio];

        CreateDessert();

    }

    public void CreateDessert() //crea el postre.
    {
        //elije que postre quiere instanciar

        randomNum = UnityEngine.Random.Range(0, Enum.GetValues(typeof(typeDessert)).Length);

        //elegir un numero al azar
        

        //cuantas piezas

        //y lo instancia


        
    }


}
