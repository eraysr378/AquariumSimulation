using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GrowthLevel
{
    Seed,
    Young,
    Mature,
    Rotten,
    Poisonous

}
public class Algae : MonoBehaviour
{
    [SerializeField] private Leaf leafPrefab;

    [SerializeField] private float growthRate;
    [SerializeField] private GrowthLevel growthLevel;
    [SerializeField] private Sand currentSandCell;
    [SerializeField] private AlgaeGrowthSO algaeGrowthSO;
    [SerializeField] private List<Prey> hiddenPreyList = new();
    private SpriteRenderer spriteRenderer;


    private float seedToYoungTime;
    private float youngToMatureTime;
    private float matureToRottenTime;
    private float rottenToPoisonousTime;
    private float poisonousToDeadTime;
    private float poisonPossibility;
    private float poisonSpreadTime;
    private float leafSpawnTime;

    private float leafSpawnerTimer;
    private float poisonTimer;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        currentSandCell = (Sand)GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        growthRate = 0.6f;


        poisonPossibility = AlgaeParameters.PoisonPossibility;
        poisonSpreadTime = AlgaeParameters.PoisonSpreadTime;
        seedToYoungTime = AlgaeParameters.SeedToYoungTime;
        youngToMatureTime = AlgaeParameters.YoungToMatureTime;
        matureToRottenTime = AlgaeParameters.MatureToRottenTime;
        rottenToPoisonousTime = AlgaeParameters.RottenToPoisonousTime;
        poisonousToDeadTime = AlgaeParameters.PoisonousToDeadTime;
        leafSpawnTime = AlgaeParameters.LeafSpawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(hiddenPreyList.Count > 0 && !IsTherePredatorAround() )
        {
            foreach(Prey prey in hiddenPreyList)
            {
                prey.GetOutOfAlgae(this);
            }
            hiddenPreyList = new List<Prey>();
        }
        switch (growthLevel)
        {
            case GrowthLevel.Seed:
                if (timer > seedToYoungTime)
                {
                    spriteRenderer.sprite = algaeGrowthSO.youngSprite;
                    growthLevel = GrowthLevel.Young;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate * 2;
                    transform.position += new Vector3(0, 0.1f, 0);
                }
                break;
            case GrowthLevel.Young:
                if (timer > youngToMatureTime)
                {
                    spriteRenderer.sprite = algaeGrowthSO.matureSprite;
                    growthLevel = GrowthLevel.Mature;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                    transform.position += new Vector3(0, 0.1f, 0);
                }
                break;
            case GrowthLevel.Mature:
                leafSpawnerTimer += Time.deltaTime;
                if (leafSpawnerTimer > leafSpawnTime)
                {
                    leafSpawnerTimer = 0;
                    SpawnLeaf();
                }
                if (timer > matureToRottenTime)
                {
                    growthLevel = GrowthLevel.Rotten;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                    transform.position += new Vector3(0, 0.1f, 0);
                }
                break;
            case GrowthLevel.Rotten:
                if (timer > rottenToPoisonousTime / 2)
                {
                    spriteRenderer.color = new Color32(255, 234, 0, 255);

                }
                if (timer > rottenToPoisonousTime)
                {
                    growthLevel = GrowthLevel.Poisonous;
                    spriteRenderer.color = new Color32(255, 145, 0, 255);

                    timer = 0;
                }
                break;
            case GrowthLevel.Poisonous:
                if (timer > poisonousToDeadTime)
                {
                    Destroy(gameObject);
                }
                else
                {
                    poisonTimer += Time.deltaTime;
                }
                // if algae is poisonous then there is a chance to make that cell poisonous
                if (poisonTimer > poisonSpreadTime)
                {
                    poisonTimer = 0;
                    if (poisonPossibility > Random.Range(0, 101))
                    {
                        currentSandCell.SpreadPoison();
                    }
                }
                break;
        }

    }
    private void SpawnLeaf()
    {
        Leaf leaf = Instantiate(leafPrefab, transform.position, Quaternion.identity);
        leaf.SetTargetWaterCell((Water)GridManager.GetCellAtPosition(new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 1), transform.position.z)));
    }
    public void EatenByCrab()
    {
        switch (growthLevel)
        {
            case GrowthLevel.Seed:
                break;
            case GrowthLevel.Young:
                break;
            case GrowthLevel.Mature:
                Leaf leaf = Instantiate(leafPrefab, new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 1), transform.position.z), Quaternion.identity);
                leaf.SetTargetWaterCell((Water)GridManager.GetCellAtPosition(new Vector3(transform.position.x - 1, Mathf.Floor(transform.position.y + 2), transform.position.z)));

                leaf = Instantiate(leafPrefab, new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 1), transform.position.z), Quaternion.identity);
                leaf.SetTargetWaterCell((Water)GridManager.GetCellAtPosition(new Vector3(transform.position.x + 1, Mathf.Floor(transform.position.y + 2), transform.position.z)));

                break;
            case GrowthLevel.Rotten:
                leaf = Instantiate(leafPrefab, new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 1), transform.position.z), Quaternion.identity);
                leaf.SetTargetWaterCell((Water)GridManager.GetCellAtPosition(new Vector3(transform.position.x - 1, Mathf.Floor(transform.position.y + 2), transform.position.z)));

                leaf = Instantiate(leafPrefab, new Vector3(transform.position.x, Mathf.Floor(transform.position.y + 1), transform.position.z), Quaternion.identity);
                leaf.SetTargetWaterCell((Water)GridManager.GetCellAtPosition(new Vector3(transform.position.x + 1, Mathf.Floor(transform.position.y + 2), transform.position.z)));

                break;
            case GrowthLevel.Poisonous:
                break;
            default:
                break;
        }
    }
    public GrowthLevel GetCurrentGrowthLevel()
    {
        return growthLevel;
    }
    public Leaf GetLeafPrefab()
    {
        return leafPrefab;
    }
    private void OnDestroy()
    {
        currentSandCell.RemoveCurrentAlgae();
        GameManager.Instance.DecreaseCurrentAlgaeAmount();
        foreach (Prey prey in hiddenPreyList)
        {
            if (prey != null)
                prey.GetOutOfAlgae(this);
        }

    }
    public void AddHiddenPrey(Prey prey)
    {
        hiddenPreyList.Add(prey);
    }
    public bool IsTherePredatorAround()
    {
        Water currentWaterCell = (Water)GridManager.GetCellAtPosition(new Vector2(Mathf.Round(currentSandCell.GetPosition().x), Mathf.Round(currentSandCell.GetPosition().y + 1)));
        List<Water> adjacentCells = currentWaterCell.GetAdjacentWaterCellList();
        if (currentWaterCell != null && currentWaterCell.GetPredatorExistencePossibility() <= 0)
        {
            foreach (Water cell in adjacentCells)
            {
                if (cell.GetPredatorExistencePossibility() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        return true;


    }
}
