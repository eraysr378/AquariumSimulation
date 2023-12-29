using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Cell
{
    [SerializeField] private CellColorsSO cellColorsSO;
    [SerializeField] private Quality quality;
    [SerializeField] private List<Prey> preyList = new List<Prey>();
    [SerializeField] private List<Predator> predatorList = new List<Predator>();
    [SerializeField] private List<Leaf> leafList = new List<Leaf>();
    [SerializeField] private float preyExistencePossibility;
    [SerializeField] private float predatorExistencePossibility;
    [SerializeField] private float leafExistencePossibility;
    private float poisonTimer;

    private void Update()
    {
        float color = 255 - preyExistencePossibility * 10;
        if(color < 0)
        {
            color = 0;
        }
        SetColor(new Color32(0, 0, (byte)(color), 255));
        poisonTimer += Time.deltaTime;

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
        if (leafExistencePossibility >= leafList.Count)
        {
            leafExistencePossibility -= Time.deltaTime;
            if (leafExistencePossibility < 0)
                leafExistencePossibility = 0;
        }
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
        foreach (Water water in GetAdjacentWaterCellList())
        {
            water.preyExistencePossibility += 1;
        }
        preyExistencePossibility += 2;
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
       
        foreach (Water water in GetAdjacentWaterCellList())
        {
            water.predatorExistencePossibility += 2;
        }
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
    public void AddLeaf(Leaf leaf)
    {
        leafExistencePossibility += 5;
        leafList.Add(leaf);
    }
    public void RemoveLeaf(Leaf leaf)
    {
        leafList.Remove(leaf);
    }
    public List<Leaf> GetLeafList()
    {
        return leafList;
    }
    public float GetLeafExistencePossiblity()
    {
        return leafExistencePossibility;
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
                ChangeColor(cellColorsSO.slightlyPoisonedColor);
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SlightlyPoisoned:
                ChangeColor(cellColorsSO.poisonedColor);
                quality = Quality.Poisoned;
                break;
            case Quality.Poisoned:
                ChangeColor(cellColorsSO.severelyPoisonedColor);
                quality = Quality.SeverelyPoisoned;
                break;
        }

    }
    public void SpreadPoisonAcrossAdjacentCells()
    {
        List<Water> adjacentWaterCellList = GetAdjacentWaterCellList();
        List<Water> poisonedWaterCellList = new List<Water>();
        foreach (Water cell in adjacentWaterCellList)
        {
            if ((int)cell.GetQuality() < (int)GetQuality())
            {
                poisonedWaterCellList.Add(cell);
            }
        }
        foreach (Water cell in poisonedWaterCellList)
        {
            cell.IncreasePoisonInvolved();
        }
    }
    public void DecreasePoisonInvolved()
    {
        switch (quality)
        {
            case Quality.SlightlyPoisoned:
                ChangeColor(cellColorsSO.defaultColor);
                quality = Quality.Healthy;
                break;
            case Quality.Poisoned:
                ChangeColor(cellColorsSO.slightlyPoisonedColor);
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SeverelyPoisoned:
                ChangeColor(cellColorsSO.poisonedColor);
                quality = Quality.Poisoned;
                break;
        }
    }
    public Quality GetQuality()
    {
        return quality;
    }
    public void PredatorPassedThrough()
    {
        preyExistencePossibility = 0;
    }
    public void PreyPassedThrough()
    {
        leafExistencePossibility = 0;
    }


}
