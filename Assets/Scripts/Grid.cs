using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
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

    void SavePosGrid(Vector3 pos, int x, int z, int i)
    {

    }
}
