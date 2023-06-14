using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeItem : MonoBehaviour
{
    public List<GameObject> _allCake;
    public int _countCake; //cant de tortas.
    public string _nameCake;
    public int _numCake; //Num de postre.
    public int _pieceCount; //Cant de piezas del postre.
    public int _pieceCountCurrent; //Cant de piezas del postre.
    public int _modulesCount; //Cant de modulos que tiene una pieza.
    public int _filledPlace; //lugar lleno..
    public int _busyPlace;
}
