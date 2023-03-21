using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmountPiece
{
    Cupcake = 2, Rosquilla = 3, Canela = 4
}

public enum typeDessert
{
    Cupcake, Rosquilla, Canela
}

public class Dessert : MonoBehaviour
{
    //postre

    public string nameDessert;
    public int amountPieces;
    public List<GameObject> desserts; //lista de porciones del postre

    public void GetDesserts()
    {

    }




}
