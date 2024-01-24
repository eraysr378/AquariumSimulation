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
    public event EventHandler OnPredatorDied;
    [SerializeField] private PredatorMood predatorMood;
    [SerializeField] private float superiority;
    [SerializeField] private Prey targetFish;

    [SerializeField] private PredatorPreferredDepthSO preferredDepthsSO;
    //[SerializeField] private PredatorSpeedSO speedsSO;

    private float calmSpeed;
    private float predationSpeed;
    private float sickSpeed;
    private float healTimer;
    private Vector3 targetPos;
    private float neededTimeToHeal;
    private float hungerPointsToStopPredation;
    private float hungerPointsForActivePredation;
    private float hungerPointsForPassivePredation;
    private float preyFeedPoint;
    // Start is called before the first frame update
    private void Awake()
    {
        OnHealthStatusChanged += Predator_OnHealthStatusChanged;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    private void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);
        targetPos = GetCurrentCell().GetPosition();
        hungerPoints = 100;


        predationSpeed = PredatorParameters.PredationSpeed;
        calmSpeed = PredatorParameters.DefaultSpeed;
        sickSpeed = PredatorParameters.SickSpeed;
        hungerCoefficient = PredatorParameters.HungerCoefficient;
        neededTimeToHeal = PredatorParameters.NeededTimeToHeal;
        hungerPointsToStopPredation = PredatorParameters.HungerPointsToStopPredation;
        hungerPointsForActivePredation = PredatorParameters.HungerPointsToActivatePredation;
        hungerPointsForPassivePredation = hungerPointsForActivePredation * 1.1f;
        preyFeedPoint = PredatorParameters.PreyFeedPoint;

    }




    // Update is called once per frame
    private void Update()
    {
        if (GetHealthStatus() == HealthStatus.Dead)
        {
            spriteRenderer.color = Color.black;

            HandleDeadMovement();
            return;
        }
        if (GetHealthStatus() == HealthStatus.Sick)
        {
            healTimer += Time.deltaTime;
            if (healTimer > neededTimeToHeal)
            {
                SetMood(PredatorMood.Calm);
                SetHealthStatus(HealthStatus.Healthy);
                healTimer = 0;
            }
        }



        hungerPoints -= Time.deltaTime * hungerCoefficient;
        if (hungerPoints < 0)
        {
            OnPredatorDied?.Invoke(this, EventArgs.Empty);
            SetHealthStatus(HealthStatus.Dead);
            GetCurrentCell().RemovePredator(this);
            return;
        }
        if (GetHealthStatus() == HealthStatus.Healthy)
        {
            if (predatorMood == PredatorMood.ActivePredation)
            {
                if (hungerPoints > hungerPointsToStopPredation)
                {
                    SetMood(PredatorMood.Calm);
                    targetFish = null;
                }
                else
                {
                    float hungerRelatedSpeed = predationSpeed + 1 - hungerPoints / 100;
                    SetSpeed(hungerRelatedSpeed);
                }
            }
            else if (hungerPoints < hungerPointsForActivePredation)
            {
                SetMood(PredatorMood.ActivePredation);
            }
            else if (hungerPoints < hungerPointsForPassivePredation)
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
    private void Predator_OnHealthStatusChanged(object sender, EventArgs e)
    {
        SetMood(PredatorMood.Calm);
        if (GetHealthStatus() == HealthStatus.Sick)
        {
            spriteRenderer.color = Color.green;
            SetPreferredDepthMin(preferredDepthsSO.depthCalmMin);
            SetPreferredDepthMax(preferredDepthsSO.depthCalmMax);
            SetSpeed(sickSpeed);
        }
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
                SetPreferredDepthMin(preferredDepthsSO.depthCalmMin);
                SetPreferredDepthMax(preferredDepthsSO.depthCalmMax);
                SetSpeed(calmSpeed);
                break;
            case PredatorMood.PassivePredation:
                spriteRenderer.color = Color.gray;
                SetPreferredDepthMin(preferredDepthsSO.depthPassivePredationMin);
                SetPreferredDepthMax(preferredDepthsSO.depthPassivePredationMax);
                SetSpeed(calmSpeed);
                break;
            case PredatorMood.ActivePredation:
                spriteRenderer.color = Color.red;
                SetPreferredDepthMin(preferredDepthsSO.depthActivePredationMin);
                SetPreferredDepthMax(preferredDepthsSO.depthActivePredationMax);
                float hungerRelatedSpeed = predationSpeed + 1 - hungerPoints / 100;
                SetSpeed(hungerRelatedSpeed);
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
            return;
        }
        float directionChangePossibility = UnityEngine.Random.Range(0, 100);
        if (GetCurrentCell().IsEdgeCell() || directionChangePossibility < 1f)
        {
            moveDirection = moveDirection == Direction.Right ? Direction.Left : Direction.Right;
        }
        if (GetHealthStatus() == HealthStatus.Healthy && (GetMood() == PredatorMood.ActivePredation || GetMood() == PredatorMood.PassivePredation) && targetFish == null && GetCurrentCell().GetPreyList().Count > 0)
        {
            targetFish = GetCurrentCell().GetPreyList().First();
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
                        // if another prey is detected and it is slower (meaning older) than the current target, then target the slower fish
                        else if (targetFish != null && adjacentWaterCellList[random].GetPreyList().Count != 0)
                        {
                            foreach (Prey prey in adjacentWaterCellList[random].GetPreyList())
                            {
                                if (prey.GetDefaultSpeed() < targetFish.GetDefaultSpeed())
                                {
                                    targetFish = prey;
                                }
                            }
                        }
                        // if there is a cell where a fish has been on, compare the cell with the candidate cell
                        if (adjacentWaterCellList[random].GetPreyExistencePossibility() > candidateTargetCell.GetPreyExistencePossibility())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                        // if there is no possible fish, then make the predator patrol
                        else if (candidateTargetCell.GetPreyExistencePossibility() <= 0)
                        {
                            if (Cell.IsDepthBetween(candidateTargetCell.GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
                            {
                                if ((moveDirection == Direction.Right && candidateTargetCell.IsOnRightOf(GetCurrentCell())) || (moveDirection == Direction.Left && candidateTargetCell.IsOnLeftOf(GetCurrentCell())))
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

                            else if (!Cell.IsDepthBetween(candidateTargetCell.GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()))
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
            if (targetFish.IsHidden() || GetHealthStatus() == HealthStatus.Sick)
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
            Destroy(prey.gameObject);
            targetFish = null;
            hungerPoints += preyFeedPoint;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Fish prey = collision.gameObject.GetComponent<Fish>();
        if (prey != null && prey == targetFish)
        {
            Destroy(prey.gameObject);
            targetFish = null;
            hungerPoints += preyFeedPoint;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, scanRadius);
    }


}
