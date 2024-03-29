using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public enum Quality
{
    Healthy,
    SlightlyPoisoned,
    Poisoned,
    SeverelyPoisoned

}
public class Cell : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Vector3 position;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


  
    public List<Water> GetAdjacentWaterCellList()
    {
        List<Water> adjacentWaterCellList = new List<Water>();

        Cell right = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y));
        if (right != null && right is Water)
            adjacentWaterCellList.Add((Water)right);
        Cell left = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y));
        if (left != null && left is Water)
            adjacentWaterCellList.Add((Water)left);
        Cell up = GridManager.GetCellAtPosition(new Vector2(position.x, position.y + 1));
        if (up != null && up is Water)
            adjacentWaterCellList.Add((Water)up);
        Cell down = GridManager.GetCellAtPosition(new Vector2(position.x, position.y - 1));
        if (down != null && down is Water)
            adjacentWaterCellList.Add((Water)down);
        Cell downRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y - 1));
        if (downRight != null && downRight is Water)
            adjacentWaterCellList.Add((Water)downRight);
        Cell downLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y - 1));
        if (downLeft != null && downLeft is Water)
            adjacentWaterCellList.Add((Water)downLeft);
        Cell upRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y + 1));
        if (upRight != null && upRight is Water)
            adjacentWaterCellList.Add((Water)upRight);
        Cell upLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y + 1));
        if (upLeft != null && upLeft is Water)
            adjacentWaterCellList.Add((Water)upLeft);

        return adjacentWaterCellList;
    }

    public List<Sand> GetAdjacentSandCellList()
    {
        List<Sand> adjacentSandCellList = new List<Sand>();

        Cell right = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y));
        if (right != null && right is Sand)
            adjacentSandCellList.Add((Sand)right);
        Cell left = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y));
        if (left != null && left is Sand)
            adjacentSandCellList.Add((Sand)left);
        Cell up = GridManager.GetCellAtPosition(new Vector2(position.x, position.y + 1));
        if (up != null && up is Sand)
            adjacentSandCellList.Add((Sand)up);
        Cell down = GridManager.GetCellAtPosition(new Vector2(position.x, position.y - 1));
        if (down != null && down is Sand)
            adjacentSandCellList.Add((Sand)down);
        Cell downRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y - 1));
        if (downRight != null && downRight is Sand)
            adjacentSandCellList.Add((Sand)downRight);
        Cell downLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y - 1));
        if (downLeft != null && downLeft is Sand)
            adjacentSandCellList.Add((Sand)downLeft);
        Cell upRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y + 1));
        if (upRight != null && upRight is Sand)
            adjacentSandCellList.Add((Sand)upRight);
        Cell upLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y + 1));
        if (upLeft != null && upLeft is Sand)
            adjacentSandCellList.Add((Sand)upLeft);

        return adjacentSandCellList;
    }
    public List<Cell> GetAdjacentCellList()
    {
        List<Cell> adjacentCellList = new List<Cell>();

        Cell right = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y));
        if (right != null)
            adjacentCellList.Add(right);
        Cell left = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y));
        if (left != null)
            adjacentCellList.Add(left);
        Cell up = GridManager.GetCellAtPosition(new Vector2(position.x, position.y + 1));
        if (up != null)
            adjacentCellList.Add(up);
        Cell down = GridManager.GetCellAtPosition(new Vector2(position.x, position.y - 1));
        if (down != null)
            adjacentCellList.Add(down);
        Cell downRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y - 1));
        if (downRight != null)
            adjacentCellList.Add(downRight);
        Cell downLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y - 1));
        if (downLeft != null)
            adjacentCellList.Add(downLeft);
        Cell upRight = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y + 1));
        if (upRight != null)
            adjacentCellList.Add(upRight);
        Cell upLeft = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y + 1));
        if (upLeft != null)
            adjacentCellList.Add(upLeft);

        return adjacentCellList;
    }

    public float GetDepth()
    {
        return position.y;
    }
    public void SetPosition(int x, int y)
    {
        position = new Vector3(x, y, 0);
    }
    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    static public bool IsDepthBetween(float depth, float min, float max)
    {
        if (depth >= min && depth <= max)
            return true;
        return false;
    }
    public bool IsOnLeftOf(Cell cell)
    {
        if (this.position.x < cell.position.x) return true;
        return false;
    }
    public bool IsOnRightOf(Cell cell)
    {
        if (this.position.x > cell.position.x) return true;
        return false;
    }
    public bool IsEdgeCell()
    {
        if(this.position.x == GridManager.Instance.GetWidth() - 1 || this.position.x == 0)
        {
            return true;
        }
        return false;
    }
   
}

