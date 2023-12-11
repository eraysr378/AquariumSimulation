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
    [SerializeField] private float poisonPossibility;
    [SerializeField] private AlgaeGrowthSO algaeGrowthSO;
    [SerializeField] private float poisonSpreadTime;
    private SpriteRenderer spriteRenderer;

    private float timer;
    private float poisonTimer;
    private float leafSpawnerTimer;
    // Start is called before the first frame update
    void Start()
    {
        currentSandCell = (Sand)GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        growthRate = 0.6f;
        poisonPossibility = 25f;
        poisonSpreadTime = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        switch (growthLevel)
        {
            case GrowthLevel.Seed:
                if (timer > algaeGrowthSO.seedTime)
                {
                    spriteRenderer.sprite = algaeGrowthSO.youngSprite;
                    growthLevel = GrowthLevel.Young;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate * 2;
                    transform.position += new Vector3(0, 0.1f, 0);
                }
                break;
            case GrowthLevel.Young:
                if (timer > algaeGrowthSO.youngTime)
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
                if (leafSpawnerTimer > 3)
                {
                    leafSpawnerTimer = 0;
                    SpawnLeaf();
                }
                if (timer > algaeGrowthSO.matureTime)
                {
                    growthLevel = GrowthLevel.Rotten;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                    transform.position += new Vector3(0, 0.1f, 0);
                }
                break;
            case GrowthLevel.Rotten:
                if (timer > algaeGrowthSO.rottenTime / 2)
                {
                    spriteRenderer.color = new Color32(255, 234, 0, 255);

                }
                if (timer > algaeGrowthSO.rottenTime)
                {
                    growthLevel = GrowthLevel.Poisonous;
                    spriteRenderer.color = new Color32(255, 145, 0, 255);

                    timer = 0;
                }
                break;
            case GrowthLevel.Poisonous:
                if (timer > algaeGrowthSO.poisonousTime)
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

    }
}
