using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public static Dictionary<Vector2, Cell> Grid;
    [SerializeField] private int width, height;

    [SerializeField] private Cell waterPrefab;
    [SerializeField] private Cell sandPrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform parent;
    private void Awake()
    {
        Instance = this;

        GenerateGrid();
    }
    private void GenerateGrid()
    {

        Grid = new Dictionary<Vector2, Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell spawnedCell;
                if( y == 0)
                {
                    spawnedCell = Instantiate(sandPrefab, new Vector3(x, y), Quaternion.identity);
                }
                else
                {
                    spawnedCell = Instantiate(waterPrefab, new Vector3(x, y), Quaternion.identity);
                }


                spawnedCell.name = $"Tile {x} {y}";
                spawnedCell.transform.SetParent(parent, false);
                spawnedCell.SetPosition(x, y);
                Grid[new Vector2(x, y)] = spawnedCell;
            }

        }
        //cam.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }
    public static Cell GetCellAtPosition(Vector2 pos)
    {
        if (Grid.TryGetValue(pos, out Cell cell))
        {
            return cell;
        }
        else
        {
            return null;
        }
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }

    static public Cell GetCellOnLeft(Cell cell)
    {
        return GetCellAtPosition(new Vector2(cell.GetPosition().x - 1, cell.GetPosition().y));

    }
    static public Cell GetCellOnRight(Cell cell)
    {
        return GetCellAtPosition(new Vector2(cell.GetPosition().x + 1, cell.GetPosition().y));

    }
}
