using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid instance;

    [Header("Grid Construction")]
    //[SerializeField] List<Cell> _cells = new List<Cell>(); //conjunto de celdas
    [SerializeField] Cell[] cells; //conjunto de celdas
    public Cell cellPrefab; //celda individual
    public int height = 4; //x = columna
    public int width = 4; //y = fila

    //static int allCells 
    static int numBusyCell = 0;

    [Header("Padding")]
    [SerializeField] float paddingX = 1.9f; //2.425
    [SerializeField] float paddingY = 1.9f; //1.85
    
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

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateGrid(x, y, i++);
            }
        }
    }

    void CreateGrid(int x, int y, int i)
    {
        Cell cell = cells[i] = Instantiate<Cell>(cellPrefab);
        cell.transform.SetParent(transform, false);

        Vector3 position;
        position.x = cell.width + (paddingX * x);
        position.y = cell.height + (paddingY * y);
        position.z = 0f;
        cell.transform.localPosition = position;
        //cell.posCell = transform.position;

        //como centramos la grilla?

        cell.name = string.Format("Dish[{0}][{1}]", x, y);
    }

    //llamar cada vez que instanciamos un plato en la grilla.
    public void CheckBusyCell() 
    {
        //esto funciona, pero deberia ser mejor,
        //podrias crear una funcion con una variable
        //que a medida que se llene la celda, sume uno
        //si se vacia, que reste.
        //y cuando llega al cells.count >= variable .. llame al gameover..
        numBusyCell = 0;

        //if(cells.TrueForAll(x => x.transform.GetComponentInChildren<Cell>().isBusy))

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
