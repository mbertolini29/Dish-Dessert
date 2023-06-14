using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NameCake
{
    Cupcake, Donut, Cinnamon
}

public enum TypeCake
{
    //Cupcake = 2, Donut = 3, Cinnamon = 4, Bagel = 5, Apple = 6/*, Rainbow = 7*/
    Cupcake = 2, Donut = 3, Cinnamon = 4/*, Bagel = 5, Apple = 6, Rainbow = 7*/
}

public enum AmountPiece
{
    //aca hay que poner el num maximo de porciones por torta que quieras que salgan.
    //cinnon diria que salgan 2 porciones y apple 3
    //Cupcake = 2, Donut = 2, Cinnamon = 4, Bagel = 4, Apple = 8, Rainbow = 8
    Cupcake = 2, Donut = 4, Cinnamon = 8 //, Bagel = 4, Apple = 8, Rainbow = 8
}

public enum PuntuacionPiece
{
    Cupcake = 100, Donut = 125, Cinnamon = 150, Bagel = 175, Apple = 200, Rainbow = 225
}

public class PieceCake : MonoBehaviour
{
    private void Start()
    {

    } 

}
