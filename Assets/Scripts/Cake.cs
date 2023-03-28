using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeCake
{
    Cupcake, Rosquilla, Canela
}

public enum AmountPiece
{
    Cupcake = 2, Rosquilla = 3, Canela = 4
}

public class Cake : MonoBehaviour
{
    //postre

    //public static Cake instance;

    //todo esto ya esta completo.
    public string nameDessert;
    public int amountPieces;
    public List<GameObject> pieceCake; //lista de porciones del postre 

    public void GetPieceCake()
    {

    }




}
