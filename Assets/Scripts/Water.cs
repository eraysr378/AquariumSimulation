using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Cell
{
    public enum TestCellColor
    {
        PreyExistence,
        PredatorExistence,
        LeafExistence,
        DeadFishExistence,
        Poisonous
    }
    public static TestCellColor CellColor;
    [SerializeField] private CellColorsSO cellColorsSO;
    [SerializeField] private Quality quality;
    [SerializeField] private List<Prey> preyList = new List<Prey>();
    [SerializeField] private List<Predator> predatorList = new List<Predator>();
    [SerializeField] private List<Leaf> leafList = new List<Leaf>();
    [SerializeField] private List<Fish> deadFishList = new List<Fish>();
    [SerializeField] private float preyExistencePossibility;
    [SerializeField] private float predatorExistencePossibility;
    [SerializeField] private float leafExistencePossibility;
    [SerializeField] private float deadFishExistencePossibility;
    [SerializeField] private float poisonTimer;

    private void Update()
    {
        TestShowCellColor();
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
        if (deadFishExistencePossibility >= deadFishList.Count)
        {
            deadFishExistencePossibility -= Time.deltaTime;
            if (deadFishExistencePossibility < 0)
                deadFishExistencePossibility = 0;
        }
    }

    public void RemoveFish(Fish fish)
    {
        if (fish.GetHealthStatus() == HealthStatus.Dead)
        {
            RemoveDeadFish(fish);
            return;
        }
        else if (fish is Prey)
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
        if (fish.GetHealthStatus() == HealthStatus.Dead)
        {
            AddDeadFish(fish);
            return;
        }
        else if (fish is Prey)
        {
            AddPrey((Prey)fish);
        }
        else if (fish is Predator)
        {
            AddPredator((Predator)fish);
        }
    }
    public void RemoveDeadFish(Fish fish)
    {
        deadFishList.Remove(fish);
    }
    public void AddDeadFish(Fish fish)
    {
        deadFishExistencePossibility += 2f;
        deadFishList.Add(fish);
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
        preyExistencePossibility += 1.75f;
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
        if (predator.GetMood() == PredatorMood.ActivePredation || predator.GetMood() == PredatorMood.PassivePredation)
        {
            foreach (Water water in GetAdjacentWaterCellList())
            {
                water.predatorExistencePossibility += 1;
                foreach (Prey prey in water.GetPreyList())
                {
                    prey.ActivateEscapeMode();
                }
            }
            foreach (Prey prey in GetPreyList())
            {
                prey.ActivateEscapeMode();
            }
            predatorExistencePossibility += 1.75f;
        }

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
    public List<Fish> GetDeadFishList()
    {
        return deadFishList;
    }
    public float GetLeafExistencePossiblity()
    {
        return leafExistencePossibility;
    }
    public float GetDeadFishExistencePossibility()
    {
        return deadFishExistencePossibility;
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
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SlightlyPoisoned:
                quality = Quality.Poisoned;
                break;
            case Quality.Poisoned:
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
    public void SetQuality(Quality quality)
    {
        this.quality = quality;
    }
    public void ResetPoisonTimer()
    {
        poisonTimer = 0;
    }
    public void DecreasePoisonInvolved()
    {
        switch (quality)
        {
            case Quality.SlightlyPoisoned:
                quality = Quality.Healthy;
                break;
            case Quality.Poisoned:
                quality = Quality.SlightlyPoisoned;
                break;
            case Quality.SeverelyPoisoned:
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
    public void ScavengerPassedThrough()
    {
        leafExistencePossibility = 0;
        deadFishExistencePossibility = 0;
    }
    static public void ShowPreyExistence()
    {
        CellColor = TestCellColor.PreyExistence;
    }
    static public void ShowPredatorExistence()
    {
        CellColor = TestCellColor.PredatorExistence;
    }
    static public void ShowLeafExistence()
    {
        CellColor = TestCellColor.LeafExistence;
    }
    static public void ShowPoisonousness()
    {
        CellColor = TestCellColor.Poisonous;
    }
    static public void ShowDeadFishExistence()
    {
        CellColor = TestCellColor.DeadFishExistence;
    }

    private void TestShowCellColor()
    {
        float color;
        switch (CellColor)
        {
            case TestCellColor.PreyExistence:
                color = 255 - preyExistencePossibility * 10;
                if (color < 0)
                {
                    color = 0;
                }
                SetColor(new Color32(0, (byte)(color), 255, 255));
                break;
            case TestCellColor.PredatorExistence:
                color = 255 - predatorExistencePossibility * 10;
                if (color < 0)
                {
                    color = 0;
                }
                SetColor(new Color32(0, (byte)(color), 255, 255));
                break;
            case TestCellColor.LeafExistence:
                color = 255 - leafExistencePossibility * 10;
                if (color < 0)
                {
                    color = 0;
                }
                SetColor(new Color32(0, (byte)(color), 255, 255));
                break;
            case TestCellColor.Poisonous:
                switch (quality)
                {
                    case Quality.Healthy:
                        SetColor(cellColorsSO.defaultColor);
                        break;
                    case Quality.SlightlyPoisoned:
                        SetColor(cellColorsSO.slightlyPoisonedColor);
                        break;
                    case Quality.Poisoned:
                        SetColor(cellColorsSO.poisonedColor);
                        break;
                    case Quality.SeverelyPoisoned:
                        SetColor(cellColorsSO.severelyPoisonedColor);
                        break;
                }
                break;
            case TestCellColor.DeadFishExistence:
                color = 255 - deadFishExistencePossibility * 10;
                if (color < 0)
                {
                    color = 0;
                }
                SetColor(new Color32(0, (byte)(color), 255, 255));
                break;
            default:
                break;
        }
    }
}
