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
public enum Direction
{
    Right,
    Left
}

public class Predator : Fish
{
    [SerializeField] private Mood mood;
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
            if (mood == Mood.ActivePredation)
            {
                if (hungerPoints > 95)
                {
                    SetMood(Mood.Calm);
                }
            }
            else if (hungerPoints < 70)
            {
                SetMood(Mood.ActivePredation);
            }
            else if (hungerPoints < 75)
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

    private void HandleDeadMovement()
    {
        Cell checkCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        SetCurrentCell(checkCell);
        SetTargetCell((Water)GridManager.GetCellAtPosition(new Vector2(GetCurrentCell().GetPosition().x, GetCurrentCell().GetPosition().y + 1)));
        if (GetTargetCell() != null)
        {
            Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
            transform.position += moveDir * speedsSO.sickSpeed * Time.deltaTime;
        }

        //if (GetTargetCell() == null)
        //{
        //    SetTargetCell((Water)GridManager.GetCellAtPosition(new Vector2(GetCurrentCell().GetPosition().x, GetCurrentCell().GetPosition().y + 1)));
        //}
        //if (GetTargetCell() != null && (Mathf.Abs(transform.position.x - GetTargetCell().GetPosition().x) < 0.1f && Mathf.Abs(transform.position.y - GetTargetCell().GetPosition().y) < 0.1f))
        //{
        //    SetCurrentCell(GetTargetCell());
        //    SetTargetCell(null);
        //}
        //if (GetTargetCell() != null)
        //{
        //    Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
        //    transform.position += moveDir * speedsSO.sickSpeed * Time.deltaTime;
        //}
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
                SetPreferredDepthMax(preferredDepthsSO.depthPassivePredationMax);
                SetSpeed(speedsSO.passivePredationSpeed);
                break;
            case Mood.ActivePredation:
                spriteRenderer.color = Color.red;
                SetPreferredDepthMin(preferredDepthsSO.depthActivePredationMin);
                SetPreferredDepthMax(preferredDepthsSO.depthActivePredationMax);
                SetSpeed(speedsSO.activePredationSpeed);
                break;
            case Mood.Reproduction:
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
                switch (mood)
                {
                    case Mood.PassivePredation:
                    case Mood.Calm:
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
                    case Mood.ActivePredation:
                        // if there is a prey around, target it
                        if (targetFish == null && adjacentWaterCellList[random].GetPreyList().Count != 0)
                        {
                            targetFish = adjacentWaterCellList[random].GetPreyList().First();
                        }
                        // if there is a cell where a fish has been on compare the cell with the candidate cell
                        if (adjacentWaterCellList[random].GetPreyExistencePossibility() > candidateTargetCell.GetPreyExistencePossibility())
                        {
                            Debug.Log("hheyy");
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
        if (mood == Mood.ActivePredation)
        {

            Debug.Log("prey detected: " + candidateTargetCell.GetPreyExistencePossibility());
            candidateTargetCell.PredatorPassedThrough();
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
        //if (checkCell != null && GetTargetCell() != checkCell && GetCurrentCell() != checkCell)
        //{
        //    SetCurrentCell(checkCell);
        //    GetAdjacentWaterCells();
        //    SelectTargetCell();
        //}

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

        //if (GetTargetCell() != null)
        //{
        //    Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
        //    transform.position += moveDir * GetSpeed() * Time.deltaTime;

        //    float angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        //    if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
        //    {
        //        angle = 360 + angle;

        //    }
        //    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
        //}

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
