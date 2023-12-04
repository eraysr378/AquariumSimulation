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
    [SerializeField] private List<Prey> preyList = new List<Prey>();
    [SerializeField] private List<Predator> predatorList = new List<Predator>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float preyExistencePossibility;
    [SerializeField] private float predatorExistencePossibility;
    [SerializeField] private float algaeEmergencePossibility;
    [SerializeField] private Transform algaePrefab;
    [SerializeField] private Quality quality;
    [SerializeField] private Algae currentAlgae;


    private Vector3 position;
    private float timer;
    private float poisonTimer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
    }
    private void Update()
    {
        timer += Time.deltaTime;
        poisonTimer += Time.deltaTime;
        if (timer > 1 && currentAlgae == null)
        {
            TrySpawningAlgae();
            timer = 0;
        }
        if (poisonTimer > 5)
        {
            DecreasePoisonInvolved();
            poisonTimer = 0;
        }
        if (preyExistencePossibility > preyList.Count)
        {
            preyExistencePossibility -= Time.deltaTime;
            if (preyExistencePossibility < 0)
                preyExistencePossibility = 0;
        }
        if (predatorExistencePossibility > predatorList.Count)
        {
            predatorExistencePossibility -= Time.deltaTime;
            if (predatorExistencePossibility < 0)
                predatorExistencePossibility = 0;
        }
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

    private void TrySpawningAlgae()
    {
        bool spawnable = true;

        Cell leftCell = GridManager.GetCellAtPosition(new Vector2(position.x - 1, position.y));
        Cell rightCell = GridManager.GetCellAtPosition(new Vector2(position.x + 1, position.y));
        if (leftCell != null && leftCell.currentAlgae != null)
        {
            spawnable = false;
        }
        if (rightCell != null && rightCell.currentAlgae != null)
        {
            spawnable = false;
        }
        if (spawnable)
        {
            if (UnityEngine.Random.Range(0, 101) < algaeEmergencePossibility && GameManager.Instance.IsAlgaeSpawnable())
            {
                currentAlgae = Instantiate(algaePrefab, transform.position, Quaternion.identity).GetComponent<Algae>();
                GameManager.Instance.IncreaseCurrentAlgaeAmount();
            }
        }

    }
    public void SetPosition(int x, int y)
    {
        position = new Vector3(x, y, 0);
        if (y == 0)
        {
            algaeEmergencePossibility = 2;
        }
    }
    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
    public float GetDepth()
    {
        return position.y;
    }
    public void RemoveFish(Fish fish)
    {
        if (fish is Prey)
        {
            RemovePrey((Prey)fish);
        }
        else if (fish is Predator)
        {
            RemovePredator((Predator)fish);
        }
    }
    public void AddFish(Fish fish)
    {
        if (fish is Prey)
        {
            AddPrey((Prey)fish);
        }
        else if (fish is Predator)
        {
            AddPredator((Predator)fish);
        }
    }
    public void RemovePrey(Prey prey)
    {
        preyList.Remove(prey);
    }
    public void AddPrey(Prey prey)
    {
        preyExistencePossibility += 1;
        preyList.Add(prey);
    }
    public List<Prey> GetPreyList()
    {
        return preyList;
    }
    public float GetPreyExistencePossibility()
    {
        return preyExistencePossibility;
    }
    public void RemovePredator(Predator predator)
    {
        predatorList.Remove(predator);
    }
    public void AddPredator(Predator predator)
    {
        predatorExistencePossibility += 3;
        predatorList.Add(predator);
    }
    public List<Predator> GetPredatorList()
    {
        return predatorList;
    }
    public float GetPredatorExistencePossibility()
    {
        return predatorExistencePossibility;
    }
 
    private void ChangeColor(Color32 color)
    {
        spriteRenderer.color = color;
    }
    public void IncreasePoisonInvolved()
    {
        poisonTimer = 0;
        if (quality != Quality.Healthy)
        {
            SpreadPoisonAcrossAdjacentCells();

        }
        switch (quality)
        {
            case Quality.Healthy:
                ChangeColor(Color.yellow);
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SlightlyPoisoned:
                ChangeColor(Color.gray);
                quality = Quality.Poisoned;
                break;
            case Quality.Poisoned:
                ChangeColor(Color.red);
                quality = Quality.SeverelyPoisoned;
                break;
        }

    }
    public void DecreasePoisonInvolved()
    {
        switch (quality)
        {
            case Quality.SlightlyPoisoned:
                ChangeColor(new Color32(0, 209, 255, 255));
                quality = Quality.Healthy;
                break;
            case Quality.Poisoned:
                ChangeColor(Color.yellow);
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SeverelyPoisoned:
                ChangeColor(Color.gray);
                quality = Quality.Poisoned;
                break;
        }
    }
    public void SpreadPoisonAcrossAdjacentCells()
    {
        List<Cell> adjacentCellList = GetAdjacentCellList();
        List<Cell> poisonedCellList = new List<Cell>();
        foreach (Cell cell in adjacentCellList)
        {
            if ((int)cell.GetQuality() < (int)GetQuality())
            {
                poisonedCellList.Add(cell);
            }
        }
        foreach (Cell cell in poisonedCellList)
        {
            cell.IncreasePoisonInvolved();

        }
    }
    public Quality GetQuality()
    {
        return quality;
    }
    public void RemoveCurrentAlgae()
    {
        currentAlgae = null;
    }
    static public bool IsDepthBetween(float depth, float min, float max)
    {
        if (depth >= min && depth <= max)
            return true;
        return false;

    }
}

