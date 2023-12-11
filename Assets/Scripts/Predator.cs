using System;
using System.Collections;
using System.Linq;
using UnityEngine;
public enum Mood
{
    Calm,
    PassivePredation,
    ActivePredation,
    Reproduction
}

public class Predator : Fish
{
    [SerializeField] private Mood mood;
    [SerializeField] private float superiority;
    [SerializeField] private Fish targetFish;

    [SerializeField] private PredatorPreferredDepthSO preferredDepthsSO;
    [SerializeField] private PredatorSpeedSO speedsSO;

    [SerializeField] private bool isTargetCellSelectionStarted;
    private float healTimer;
    // Start is called before the first frame update
    private void Awake()
    {
        OnHealthStatusChanged += Predator_OnHealthStatusChanged;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Predator_OnHealthStatusChanged(object sender, EventArgs e)
    {
        SetMood(Mood.Calm);
        if (GetHealthStatus() == HealthStatus.Sick)
        {
            spriteRenderer.color = Color.green;
            SetPreferredDepthMin(preferredDepthsSO.depthCalmMin);
            SetPreferredDepthMax(preferredDepthsSO.depthCalmMax);
            SetSpeed(speedsSO.sickSpeed);
        }
    }

    private void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        hungerCoefficient = 2;
        hungerPoints = 100;
    }


    // Update is called once per frame
    private void Update()
    {
        if (GetHealthStatus() == HealthStatus.Dead)
        {
            spriteRenderer.color = Color.black;
            return;
        }
        if (GetHealthStatus() == HealthStatus.Sick)
        {
            healTimer += Time.deltaTime;
            if (healTimer > 10)
            {
                SetMood(Mood.Calm);
                SetHealthStatus(HealthStatus.Healthy);
                healTimer = 0;
            }
        }



        hungerPoints -= Time.deltaTime * hungerCoefficient;
        if (hungerPoints < 0)
        {
            SetHealthStatus(HealthStatus.Dead);
        }
        if (GetHealthStatus() == HealthStatus.Healthy)
        {
            if (hungerPoints < 20)
            {
                SetMood(Mood.ActivePredation);
            }
            else if (hungerPoints < 60)
            {
                SetMood(Mood.PassivePredation);
            }
            else
            {
                SetMood(Mood.Calm);
            }
        }


        HandleMovement();
    }

    public void SetMood(Mood newMood)
    {

        mood = newMood;
        switch (mood)
        {
            case Mood.Calm:
                spriteRenderer.color = Color.yellow;
                targetFish = null;
                SetPreferredDepthMin(preferredDepthsSO.depthCalmMin);
                SetPreferredDepthMax(preferredDepthsSO.depthCalmMax);
                SetSpeed(speedsSO.calmSpeed);
                break;
            case Mood.PassivePredation:
                spriteRenderer.color = Color.gray;
                SetPreferredDepthMin(preferredDepthsSO.depthPassivePredationMin);
                SetPreferredDepthMin(preferredDepthsSO.depthPassivePredationMax);
                SetSpeed(speedsSO.passivePredationSpeed);
                break;
            case Mood.ActivePredation:
                spriteRenderer.color = Color.red;
                SetPreferredDepthMin(preferredDepthsSO.depthActivePredationMin);
                SetPreferredDepthMin(preferredDepthsSO.depthActivePredationMax);
                SetSpeed(speedsSO.activePredationSpeed);
                break;
            case Mood.Reproduction:
                break;

            default:
                break;
        }
    }
    private IEnumerator SelectTargetCell()
    {
        if (adjacentWaterCellList.Count == 0)
        {
            isTargetCellSelectionStarted = false;
            yield break;
        }
        Water candidateTargetCell = adjacentWaterCellList[0];
        adjacentWaterCellList.RemoveAt(0);
        while (adjacentWaterCellList.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, adjacentWaterCellList.Count);
            if (GetHealthStatus() == HealthStatus.Sick)
            {
                if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                {
                    candidateTargetCell = adjacentWaterCellList[random];
                }
                else
                {
                    if (GetCurrentDepth() > GetPreferredDepthMax() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMax()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMax()))
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                    }
                    else if (GetCurrentDepth() < GetPreferredDepthMin() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMin()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMin()))
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                    }
                }
                adjacentWaterCellList.RemoveAt(random);
                yield return null;
            }
            else
            {
                switch (mood)
                {
                    case Mood.PassivePredation:
                    case Mood.Calm:
                        if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                        else
                        {
                            if (GetCurrentDepth() > GetPreferredDepthMax() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMax()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMax()))
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                            else if (GetCurrentDepth() < GetPreferredDepthMin() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMin()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMin()))
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }

                        adjacentWaterCellList.RemoveAt(random);
                        yield return null;
                        break;
                    case Mood.ActivePredation:
                        // if there is a prey around, target it
                        if (targetFish == null && adjacentWaterCellList[random].GetPreyList().Count != 0)
                        {
                            targetFish = adjacentWaterCellList[random].GetPreyList().First();
                        }
                        // if there is a cell where a fish has been on compare the cell with the candidate cell
                        if (adjacentWaterCellList[random].GetPreyExistencePossibility() > 0)
                        {
                            if (adjacentWaterCellList[random].GetPreyExistencePossibility() > candidateTargetCell.GetPreyExistencePossibility())
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }
                        // if there is no possible fish, then make the predator patrol
                        else if (candidateTargetCell.GetPreyExistencePossibility() <= 0)
                        {
                            if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                            else
                            {
                                if (GetCurrentDepth() > GetPreferredDepthMax() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMax()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMax()))
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                                else if (GetCurrentDepth() < GetPreferredDepthMin() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMin()) > Mathf.Abs(adjacentWaterCellList[random].GetDepth() - GetPreferredDepthMin()))
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                            }
                        }
                        adjacentWaterCellList.RemoveAt(random);
                        yield return null;
                        break;
                }
            }
        }
        SetTargetCell(candidateTargetCell);
    }

    private void HandleMovement()
    {
        float rotationSpeed = 10;
        if (targetFish != null)
        {
            Vector3 moveDir = (targetFish.transform.position - transform.position).normalized;
            transform.position += moveDir * GetSpeed() * Time.deltaTime;
            float angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
            {
                angle = 360 + angle;
            }
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
            return;
        }
        if (GetTargetCell() == null && !isTargetCellSelectionStarted)
        {
            isTargetCellSelectionStarted = true;
            GetAdjacentWaterCells();
            StartCoroutine(SelectTargetCell());
        }
        if (GetTargetCell() != null && (Mathf.Abs(transform.position.x - GetTargetCell().GetPosition().x) < 0.1f && Mathf.Abs(transform.position.y - GetTargetCell().GetPosition().y) < 0.1f))
        {
            SetCurrentCell(GetTargetCell());
            isTargetCellSelectionStarted = false;
            SetTargetCell(null);
        }
        if (GetTargetCell() != null)
        {
            Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
            transform.position += moveDir * GetSpeed() * Time.deltaTime;

            float angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
            {
                angle = 360 + angle;

            }
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
        }

    }
    public Mood GetMood()
    {
        return mood;
    }
    public float GetHungerPoints()
    {
        return hungerPoints;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish prey = collision.gameObject.GetComponent<Fish>();
        if (prey != null && prey == targetFish)
        {
            Destroy(prey.gameObject);
            targetFish = null;
            hungerPoints += 5;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, scanRadius);
    }


}
