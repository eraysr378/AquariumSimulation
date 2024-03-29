using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum HungerLevel
{
    Full,
    NotHungry,
    Hungry,
    Starving,
}
public enum HealthStatus
{
    Healthy,
    Sick,
    Dead
}
public enum Maturity
{
    Baby,
    Teen,
    Adult,
    Elderly
}
public class Fish : MonoBehaviour
{
    public event EventHandler OnHealthStatusChanged;
    [SerializeField] protected List<Water> adjacentWaterCellList;
    [SerializeField] protected List<Sand> adjacentSandCellList;
    [SerializeField] protected float hungerPoints;
    [SerializeField] protected float hungerCoefficient;
    [SerializeField] protected Direction moveDirection;

    [SerializeField] private float speed;
    [SerializeField] private Water currentCell;
    [SerializeField] private Water targetCell;
    [SerializeField] private float preferredDepthMin;
    [SerializeField] private float preferredDepthMax;
    [SerializeField] private float currentDepth;
    [SerializeField] private PoisonPossibilitySO poisonPossibilitySO;
    [SerializeField] private HungerLevel hungerLevel;
    [SerializeField] private HealthStatus healthStatus;

    protected SpriteRenderer spriteRenderer;



    protected void GetAdjacentWaterCells()
    {
        if (currentCell != null)
        {
            adjacentWaterCellList = currentCell.GetAdjacentWaterCellList();
        }
        else
        {
            adjacentWaterCellList = new List<Water>();
        }
    }
    protected void GetAdjacentSandCells()
    {
        if (currentCell != null)
        {
            adjacentSandCellList = currentCell.GetAdjacentSandCellList();
        }
        else
        {
            adjacentSandCellList = new List<Sand>();
        }
    }
    public bool IsHidden()
    {
        return !gameObject.activeSelf;
    }

    protected Water GetCurrentCell()
    {
        return currentCell;
    }
    protected void SetCurrentCell(Cell cell)
    {
        if (currentCell != null)
        {
            currentCell.RemoveFish(this);
        }
        currentCell = (Water)cell;
        currentCell.AddFish(this);
        currentDepth = currentCell.GetDepth();
        int random = UnityEngine.Random.Range(0, 101);
        switch (currentCell.GetQuality())
        {
            case Quality.SlightlyPoisoned:
                if (random < poisonPossibilitySO.slightlyPoisoned)
                {
                    SetHealthStatus(HealthStatus.Sick);
                    OnHealthStatusChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case Quality.Poisoned:
                if (random < poisonPossibilitySO.poisoned)
                {
                    SetHealthStatus(HealthStatus.Sick);
                    OnHealthStatusChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case Quality.SeverelyPoisoned:
                if (random < poisonPossibilitySO.severelyPoisoned)
                {
                    SetHealthStatus(HealthStatus.Sick);
                    OnHealthStatusChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }
    protected void SetHealthStatus(HealthStatus newHealthStatus)
    {
        healthStatus = newHealthStatus;
        OnHealthStatusChanged?.Invoke(this, EventArgs.Empty);
    }
    public HealthStatus GetHealthStatus()
    {
        return healthStatus;
    }
    protected Cell GetTargetCell()
    {
        return targetCell;
    }
    protected void SetTargetCell(Water cell)
    {
        targetCell = cell;
    }
    protected float GetCurrentDepth()
    {
        return currentDepth;
    }
    protected void SetCurrentDepth(float depth)
    {
        currentDepth = depth;
    }
    protected float GetPreferredDepthMin()
    {
        return preferredDepthMin;
    }
    protected void SetPreferredDepthMin(float depth)
    {
        preferredDepthMin = depth;
    }
    protected float GetPreferredDepthMax()
    {
        return preferredDepthMax;
    }
    protected void SetPreferredDepthMax(float depth)
    {
        preferredDepthMax = depth;
    }
    public float GetSpeed()
    {
        return speed;
    }
    protected void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void OnDestroy()
    {
        GetCurrentCell().RemoveFish(this);
    }

    public void SetHungePoints(float hungerPoints)
    {
        this.hungerPoints = hungerPoints;
    }

    public void SetMoveDirection(Direction direction)
    {
        moveDirection = direction;
    }

}
