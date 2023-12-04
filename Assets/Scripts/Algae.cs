using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GrowthLevel
{
    Seed,
    Young,
    Healthy,
    Rotten,
    Poisonous

}
public class Algae : MonoBehaviour
{
    [SerializeField] private float growthRate;
    [SerializeField] private GrowthLevel growthLevel;
    [SerializeField] private Cell currentCell;
    [SerializeField] private float poisonPossibility;

    private SpriteRenderer spriteRenderer;

    private float timer;
    private float poisonTimer;
    // Start is called before the first frame update
    void Start()
    {
        currentCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        spriteRenderer = GetComponent<SpriteRenderer>();
        growthRate = 0.5f;
        poisonPossibility = 25f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        switch (growthLevel)
        {
            case GrowthLevel.Seed:
                if (timer > 2)
                {
                    growthLevel = GrowthLevel.Young;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                }
                break;
            case GrowthLevel.Young:
                if (timer > 2)
                {
                    growthLevel = GrowthLevel.Healthy;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                }
                break;
            case GrowthLevel.Healthy:
                if (timer > 2)
                {
                    growthLevel = GrowthLevel.Rotten;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                }
                break;
            case GrowthLevel.Rotten:
                if (timer > 2)
                {
                    growthLevel = GrowthLevel.Poisonous;
                    timer = 0;
                    transform.localScale += transform.localScale * growthRate;
                }
                break;
            case GrowthLevel.Poisonous:
                if (timer > 30)
                {
                    currentCell.RemoveCurrentAlgae();
                    GameManager.Instance.DecreaseCurrentAlgaeAmount();
                    Destroy(gameObject);
                }
                else
                {
                    poisonTimer += Time.deltaTime;
                }
                // if algae is poisonous then there is a chance to make that cell poisonous
                if(poisonTimer > 1)
                {
                    poisonTimer = 0;
                    if(poisonPossibility > Random.Range(0, 101))
                    {
                        currentCell.IncreasePoisonInvolved();
                    }
                }
                break;
        }

    }
}
