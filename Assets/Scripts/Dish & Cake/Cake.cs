using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeCake
{
    Cupcake, Rosquilla, Canela
}

public enum AmountPiece
{
    Cupcake = 2, Rosquilla = 3, Canela = 4
}

public enum PuntuacionPiece
{
    Cupcake = 100, Rosquilla = 150, Canela = 200
}

public class Cake : MonoBehaviour
{
    //postre

    //public static Cake instance;

    //todo esto ya esta completo.
    public string nameCake;
    public int pieceCount;
    public List<GameObject> piece; //lista de porciones del postre 

    public void GetPieceCake()
    {

    }




}
