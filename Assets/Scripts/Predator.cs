using System;
using System.Collections;
using System.Linq;
using UnityEngine;
public enum PredatorMood
{
    Calm,
    PassivePredation,
    ActivePredation,
    Reproduction
}
public enum Direction
{
    Right,
    Left
}

public class Predator : Fish
{
    [SerializeField] private PredatorMood predatorMood;
    [SerializeField] private float superiority;
    [SerializeField] private Fish targetFish;

    [SerializeField] private PredatorPreferredDepthSO preferredDepthsSO;
    [SerializeField] private PredatorSpeedSO speedsSO;

    [SerializeField] private bool isTargetCellSelectionStarted;
    [SerializeField] private Direction moveDirection;
    private float healTimer;
    private Vector3 targetPos;
    // Start is called before the first frame update
    private void Awake()
    {
        OnHealthStatusChanged += Predator_OnHealthStatusChanged;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Predator_OnHealthStatusChanged(object sender, EventArgs e)
    {
        SetMood(PredatorMood.Calm);
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
        BaseStart();
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);

        targetPos = GetCurrentCell().GetPosition();
        hungerCoefficient = 2;
        hungerPoints = 100;
    }




    // Update is called once per frame
    private void Update()
    {
        BaseUpdate();
        if (GetHealthStatus() == HealthStatus.Dead)
        {
            spriteRenderer.color = Color.black;

            HandleDeadMovement();
            return;
        }
        if (GetHealthStatus() == HealthStatus.Sick)
        {
            healTimer += Time.deltaTime;
            if (healTimer > 10)
            {
                SetMood(PredatorMood.Calm);
                SetHealthStatus(HealthStatus.Healthy);
                healTimer = 0;
            }
        }



        hungerPoints -= Time.deltaTime * hungerCoefficient;
        if (hungerPoints < 0)
        {
            SetHealthStatus(HealthStatus.Dead);
            GetCurrentCell().RemovePredator(this);
            return;
        }
        if (GetHealthStatus() == HealthStatus.Healthy)
        {
            if (predatorMood == PredatorMood.ActivePredation)
            {
                if (hungerPoints > 95)
                {
                    SetMood(PredatorMood.Calm);
                }
            }
            else if (hungerPoints < 70)
            {
                SetMood(PredatorMood.ActivePredation);
            }
            else if (hungerPoints < 75)
            {
                SetMood(PredatorMood.PassivePredation);
            }
            else
            {
                SetMood(PredatorMood.Calm);
            }
        }


        HandleMovement();
    }

    private void HandleDeadMovement()
    {
        Cell checkCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        if (checkCell != null && checkCell != GetCurrentCell())
        {
            SetCurrentCell(checkCell);
            SetTargetCell((Water)GridManager.GetCellAtPosition(new Vector2(GetCurrentCell().GetPosition().x, GetCurrentCell().GetPosition().y + 1)));
        }
        if (GetTargetCell() != null)
        {
            Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
            transform.position += moveDir * Time.deltaTime;
        }
    }
    public void SetMood(PredatorMood newMood)
    {

        predatorMood = newMood;
        switch (predatorMood)
        {
            case PredatorMood.Calm:
                spriteRenderer.color = Color.yellow;
                targetFish = null;
                SetPreferredDepthMin(preferredDepthsSO.depthCalmMin);
                SetPreferredDepthMax(preferredDepthsSO.depthCalmMax);
                SetSpeed(speedsSO.calmSpeed);
                break;
            case PredatorMood.PassivePredation:
                spriteRenderer.color = Color.gray;
                SetPreferredDepthMin(preferredDepthsSO.depthPassivePredationMin);
                SetPreferredDepthMax(preferredDepthsSO.depthPassivePredationMax);
                SetSpeed(speedsSO.passivePredationSpeed);
                break;
            case PredatorMood.ActivePredation:
                spriteRenderer.color = Color.red;
                SetPreferredDepthMin(preferredDepthsSO.depthActivePredationMin);
                SetPreferredDepthMax(preferredDepthsSO.depthActivePredationMax);
                SetSpeed(speedsSO.activePredationSpeed);
                break;
            case PredatorMood.Reproduction:
                break;

            default:
                break;
        }
    }
    private void SelectTargetCell()
    {
        if (adjacentWaterCellList.Count == 0)
        {
            isTargetCellSelectionStarted = false;
            return;
        }
        float directionChangePossibility = UnityEngine.Random.Range(0, 100);
        if (GetCurrentCell().IsEdgeCell() || directionChangePossibility < 1f)
        {
            moveDirection = moveDirection == Direction.Right ? Direction.Left : Direction.Right;
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

            }
            else
            {
                switch (predatorMood)
                {
                    case PredatorMood.PassivePredation:
                    case PredatorMood.Calm:
                        if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                        {
                            if (moveDirection == Direction.Right && adjacentWaterCellList[random].IsOnRightOf(GetCurrentCell()))
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                            else if (moveDirection == Direction.Left && adjacentWaterCellList[random].IsOnLeftOf(GetCurrentCell()))
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
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
                        break;
                    case PredatorMood.ActivePredation:
                        // if there is a prey around, target it
                        if (targetFish == null && adjacentWaterCellList[random].GetPreyList().Count != 0)
                        {
                            targetFish = adjacentWaterCellList[random].GetPreyList().First();
                        }
                        // if there is a cell where a fish has been on compare the cell with the candidate cell
                        if (adjacentWaterCellList[random].GetPreyExistencePossibility() > candidateTargetCell.GetPreyExistencePossibility())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];

                        }
                        // if there is no possible fish, then make the predator patrol
                        else if (candidateTargetCell.GetPreyExistencePossibility() <= 0)
                        {
                            if(Cell.IsDepthBetween(candidateTargetCell.GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                            {
                                if ((moveDirection == Direction.Right && candidateTargetCell.IsOnRightOf(GetCurrentCell())) ||( moveDirection == Direction.Left && candidateTargetCell.IsOnLeftOf(GetCurrentCell())))
                                {
                                    adjacentWaterCellList.RemoveAt(random);
                                    break;
                                }
                            }
                            if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                            {
                                if (moveDirection == Direction.Right && adjacentWaterCellList[random].IsOnRightOf(GetCurrentCell()))
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                                else if (moveDirection == Direction.Left && adjacentWaterCellList[random].IsOnLeftOf(GetCurrentCell()))
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                            }

                            else if(!Cell.IsDepthBetween(candidateTargetCell.GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
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
                        break;
                }
            }
        }
        if (predatorMood == PredatorMood.ActivePredation)
        {

            Debug.Log("prey detected: " + candidateTargetCell.GetPreyExistencePossibility());
            GetCurrentCell().PredatorPassedThrough();
        }
        SetTargetCell(candidateTargetCell);
    }

    private void HandleMovement()
    {
        float rotationSpeed = 5;
        Vector3 moveDir;
        float angle;


        if (targetFish != null)
        {
            if (targetFish.IsHidden())
            {
                targetFish = null;
                return;
            }
            Cell checkCurrentCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
            if (checkCurrentCell != null && checkCurrentCell != GetCurrentCell())
            {
                SetCurrentCell(checkCurrentCell);
                GetAdjacentWaterCells();
                SelectTargetCell();
            }

            moveDir = (targetFish.transform.position - transform.position).normalized;
            transform.position += moveDir * GetSpeed() * Time.deltaTime;
            angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
            {
                angle = 360 + angle;
            }
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
            return;
        }

        Cell checkCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        if (checkCell != null && GetTargetCell() == checkCell)
        {
            SetCurrentCell(checkCell);
            GetAdjacentWaterCells();
            SelectTargetCell();
        }

        if (GetTargetCell() != null && GetTargetCell() != GetCurrentCell())
        {
            targetPos = GetTargetCell().GetPosition();
        }
        moveDir = (targetPos - transform.position).normalized;
        transform.position += moveDir * GetSpeed() * Time.deltaTime;

        angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
        {
            angle = 360 + angle;

        }
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
    }
    public PredatorMood GetMood()
    {
        return predatorMood;
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
            Debug.Log("predator trigger enter");

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
