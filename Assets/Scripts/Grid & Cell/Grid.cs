using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid instance;

    [Header("Grid Construction")]
    [SerializeField] Cell[] cells; //conjunto de celdas
    public Cell cellPrefab; //celda individual
    public int width = 4; //x = fila
    public int height = 4; //z = columna

    [Header("Padding")]
    [SerializeField] float paddingX = 2.1f;
    [SerializeField] float paddingZ = 1.6f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //se crea la grilla de 4 * 4
        cells = new Cell[width * height];

        //se rellena la grilla
        int i = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateGrid(x, z, i++);
            }
        }
    }

    void CreateGrid(int x, int z, int i)
    {
        Cell cell = cells[i] = Instantiate<Cell>(cellPrefab);
        cell.transform.SetParent(transform, false);

        Vector3 position;
        position.x = (cell.width + (paddingX * x));
        position.y = 0f;
        position.z = (cell.height + (paddingZ * z));
        cell.transform.localPosition = position;

        //como centramos la grilla?

        cell.name = string.Format("Dish[{0}][{1}]", x, z);
    }

    //llamar cada vez que instanciamos un plato en la grilla.
    public void CheckBusyCell() 
    {
        //esto funciona, pero deberia ser mejor,
        //podrias crear una funcion con una variable
        //que a medida que se llene la celda, sume uno
        //si se vacia, que reste.
        //y cuando llega al cells.count >= variable .. llame al gameover..

        int numBusyCell = 0;

        foreach (var item in cells)
        {
            if(item.transform.GetComponentInChildren<Cell>().isBusy)
            {
                numBusyCell++;
            }
        }

        if(numBusyCell >= (width*height))
        {
            //llamar al gameOver. 
            UIManager.instance.GameoverScreen();
        }
    }
}
