using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum PreyMood
{
    Calm,
    Hungry,
    Escape,
    Reproduction
}
public class Prey : Fish
{
    public static float AlivePreyCount;
    public event EventHandler OnMaturityChanged;
    [SerializeField] private PreyMood mood;
    [SerializeField] private Leaf targetLeaf;
    [SerializeField] private PreyEgg preyEggPrefab;
    [SerializeField] private FloatingBar floatingBar;
    [SerializeField] private Maturity maturity;
    [SerializeField] private float scalingAmount;
    [SerializeField] private bool canLayEgg;
    [SerializeField] private Water predatorCell;
    private float babyTimer = 10;
    private float teenTimer = 10;
    private float adultTimer = 30;
    private float ageTimer;
    private Vector3 targetPos;
    private float escapeTimer;
    private float escapeSpeed;
    private float defaultSpeed;
    private bool isLayingEgg;
    private float neededTimeToLayEgg;
    private float hungerLimit;
    private float neededTimeToLayEggIncraseAmount;
    private float canLayEggTimer;
    private float eggLayingDuration;
    private float leafFeedPoint;
    private float escapeTime;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        OnMaturityChanged += Prey_OnMaturityChanged;
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);
        targetPos = GetCurrentCell().GetPosition();

        SetPreferredDepthMax(8);
        SetPreferredDepthMin(6);
        escapeSpeed = PreyParameters.EscapingSpeed;
        defaultSpeed = PreyParameters.DefaultSpeed;
        neededTimeToLayEgg = PreyParameters.NeededTimeToLayEgg;
        neededTimeToLayEggIncraseAmount = PreyParameters.NeededTimeToLayEggIncreaseAmount;
        hungerLimit = PreyParameters.HungerLimit;
        eggLayingDuration = PreyParameters.EggLayingDuration;
        leafFeedPoint = PreyParameters.LeafFeedPoint;
        escapeTime = PreyParameters.EscapeTime;
    }

    private void Prey_OnMaturityChanged(object sender, EventArgs e)
    {
        transform.localScale *= scalingAmount;

        switch (GetMaturity())
        {
            case Maturity.Teen:
                escapeSpeed *= 0.9f;
                defaultSpeed *= 0.9f;
                break;
            case Maturity.Adult:
                escapeSpeed *= 0.9f;
                defaultSpeed *= 0.9f;
                break;
            case Maturity.Elderly:
                escapeSpeed *= 0.9f;
                defaultSpeed *= 0.9f;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (GetHealthStatus() == HealthStatus.Dead)
        {
            spriteRenderer.color = Color.black;

            HandleDeadMovement();
            return;
        }
        if (!CanLayEgg() && GetMaturity() >= Maturity.Adult && mood != PreyMood.Escape)
        {
            canLayEggTimer += Time.deltaTime;
            if (canLayEggTimer > neededTimeToLayEgg)
            {
                SetCanLayEgg(true);
                canLayEggTimer = 0;
            }
        }
        hungerPoints -= Time.deltaTime;
        if (hungerPoints < 0)
        {
            SetHealthStatus(HealthStatus.Dead);
            GetCurrentCell().RemovePrey(this);
            GameManager.Instance.DecreaseAlivePreyCount();
            return;
        }
        if (escapeTimer > 0)
        {
            escapeTimer -= Time.deltaTime;
            spriteRenderer.color = Color.red;
            mood = PreyMood.Escape;
            float speed = Mathf.Lerp(GetSpeed(), escapeSpeed, Time.deltaTime);
            SetSpeed(speed);
        }
        else if (hungerPoints < hungerLimit && mood != PreyMood.Reproduction)
        {
            spriteRenderer.color = Color.yellow;
            mood = PreyMood.Hungry;
            SetSpeed(defaultSpeed);
        }
        else if (CanLayEgg() || isLayingEgg)
        {
            spriteRenderer.color = Color.green;
            mood = PreyMood.Reproduction;
            SetSpeed(defaultSpeed);
            if (!isLayingEgg)
            {
                if (GetCurrentCell().GetDepth() == 1)
                {
                    LayEgg();
                }
            }

        }
        else
        {
            spriteRenderer.color = Color.white;
            mood = PreyMood.Calm;
            SetSpeed(defaultSpeed);
        }
        CheckMaturity();
        HandleMovement();
    }
    private void CheckMaturity()
    {
        ageTimer += Time.deltaTime;
        if (maturity == Maturity.Baby && ageTimer > babyTimer)
        {
            ageTimer = 0;
            maturity = Maturity.Teen;
            OnMaturityChanged?.Invoke(this, EventArgs.Empty);
        }
        if (maturity == Maturity.Teen && ageTimer > teenTimer)
        {
            ageTimer = 0;
            maturity = Maturity.Adult;
            canLayEgg = true;
            OnMaturityChanged?.Invoke(this, EventArgs.Empty);
        }
        if (maturity == Maturity.Adult && ageTimer > adultTimer)
        {
            ageTimer = 0;
            maturity = Maturity.Elderly;
            OnMaturityChanged?.Invoke(this, EventArgs.Empty);
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
    private void SelectTargetCell()
    {
        if (adjacentWaterCellList.Count == 0)
        {
            return;
        }
        if (GetCurrentCell().IsEdgeCell())
        {
            moveDirection = moveDirection == Direction.Right ? Direction.Left : Direction.Right;
        }
        if (targetLeaf == null && GetCurrentCell().GetLeafList().Count > 0)
        {
            targetLeaf = GetCurrentCell().GetLeafList().First();
        }
        Water candidateTargetCell = adjacentWaterCellList[0];
        adjacentWaterCellList.RemoveAt(0);
        while (adjacentWaterCellList.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, adjacentWaterCellList.Count);

            switch (mood)
            {
                case PreyMood.Calm:
                    if (adjacentWaterCellList[random].GetPredatorExistencePossibility() > 0)
                    {
                        ActivateEscapeMode(adjacentWaterCellList[random]);
                        GetAdjacentWaterCells();
                        continue;
                    }
                    if (adjacentWaterCellList[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                    }
                    else if (candidateTargetCell.GetPredatorExistencePossibility() == 0)
                    {
                        if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()) && adjacentWaterCellList[random].GetPredatorExistencePossibility() == 0)
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
                        else if (adjacentWaterCellList[random].GetPredatorExistencePossibility() == 0)
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
                case PreyMood.Hungry:
                    if (targetLeaf == null && adjacentWaterCellList[random].GetLeafList().Count != 0)
                    {
                        targetLeaf = adjacentWaterCellList[random].GetLeafList().First();
                    }
                    if (adjacentWaterCellList[random].GetLeafExistencePossiblity() > 0)
                    {
                        if (adjacentWaterCellList[random].GetLeafExistencePossiblity() > candidateTargetCell.GetLeafExistencePossiblity())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                    }
                    else if (candidateTargetCell.GetLeafExistencePossiblity() <= 0)
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
                case PreyMood.Escape:
                    Water localPredatorCell = predatorCell;
                    if (GetCurrentCell().GetDepth() > 1)
                    {
                        if (Mathf.Abs(candidateTargetCell.GetPosition().x - localPredatorCell.GetPosition().x) == 2 && candidateTargetCell.GetDepth() < GetCurrentCell().GetDepth())
                        {
                            adjacentWaterCellList.RemoveAt(random);
                            break;
                        }
                        else if (Mathf.Abs(adjacentWaterCellList[random].GetPosition().x - predatorCell.GetPosition().x) == 2 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                            adjacentWaterCellList.RemoveAt(random);
                            break;
                        }
                        else if (GetCurrentCell().IsEdgeCell())
                        {
                            if (Mathf.Abs(candidateTargetCell.GetPosition().x - predatorCell.GetPosition().x) == 1 && candidateTargetCell.GetDepth() < GetCurrentCell().GetDepth())
                            {
                                adjacentWaterCellList.RemoveAt(random);
                                break;
                            }
                            else if (Mathf.Abs(adjacentWaterCellList[random].GetPosition().x - predatorCell.GetPosition().x) == 1 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }
                        else if (adjacentWaterCellList[random].GetPredatorList().Count == 0 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                        {
                            if (candidateTargetCell.GetPredatorList().Count == 0 && candidateTargetCell.GetDepth() < GetCurrentCell().GetDepth())
                            {
                                if (adjacentWaterCellList[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                            }
                            else
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }

                        /*
                        if (adjacentWaterCellList[random].GetPredatorList().Count == 0 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                        {
                            if (candidateTargetCell.GetPredatorList().Count == 0 && candidateTargetCell.GetDepth() < GetCurrentCell().GetDepth())
                            {
                                if (Mathf.Abs(adjacentWaterCellList[random].GetPosition().x - predatorCell.GetPosition().x) == 2)
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                                else
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                            }
                            else
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }
                        else
                        {

                        }*/
                    }
                    else
                    {

                        if (moveDirection == Direction.Right && adjacentWaterCellList[random].GetDepth() == 1 && adjacentWaterCellList[random].IsOnRightOf(GetCurrentCell()))
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                        else if (moveDirection == Direction.Left && adjacentWaterCellList[random].GetDepth() == 1 && adjacentWaterCellList[random].IsOnLeftOf(GetCurrentCell()))
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                    }
                    adjacentWaterCellList.RemoveAt(random);
                    break;
                case PreyMood.Reproduction:
                    if (GetCurrentCell().GetDepth() > 1)
                    {
                        if (adjacentWaterCellList[random].GetPredatorList().Count == 0 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                        {
                            if (candidateTargetCell.GetPredatorList().Count == 0 && candidateTargetCell.GetDepth() < GetCurrentCell().GetDepth())
                            {
                                if (candidateTargetCell.GetPredatorExistencePossibility() > adjacentWaterCellList[random].GetPredatorExistencePossibility())
                                {
                                    candidateTargetCell = adjacentWaterCellList[random];
                                }
                            }
                            else
                            {
                                candidateTargetCell = adjacentWaterCellList[random];
                            }
                        }
                    }
                    else
                    {
                        if (moveDirection == Direction.Right && adjacentWaterCellList[random].GetDepth() == 1 && adjacentWaterCellList[random].IsOnRightOf(GetCurrentCell()))
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                        else if (moveDirection == Direction.Left && adjacentWaterCellList[random].GetDepth() == 1 && adjacentWaterCellList[random].IsOnLeftOf(GetCurrentCell()))
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                        }
                    }
                    adjacentWaterCellList.RemoveAt(random);

                    break;
                default:
                    break;
            }
        }
        if (mood == PreyMood.Hungry)
        {
            GetCurrentCell().PreyPassedThrough();
        }
        SetTargetCell(candidateTargetCell);

    }
    private void LayEgg()
    {
        neededTimeToLayEgg += neededTimeToLayEggIncraseAmount; // on each lay, increase the needed time to lay another egg to balance the population growth
        SetCanLayEgg(false);
        isLayingEgg = true;
        Invoke("SpawnEgg", eggLayingDuration);
        floatingBar.FillBar(eggLayingDuration);
    }
    private void SpawnEgg()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.05f, 0);
        PreyEgg egg = Instantiate(preyEggPrefab, spawnPos, Quaternion.identity);
        egg.SetCurrentCell(GetCurrentCell());
        isLayingEgg = false;
    }
    private void HandleMovement()
    {
        if (isLayingEgg)
        {
            return;
        }
        float rotationSpeed = 10f;
        Vector3 moveDir;
        float angle;
        if (targetLeaf != null)
        {
            Cell checkCurrentCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
            if (checkCurrentCell != null && checkCurrentCell != GetCurrentCell())
            {
                SetCurrentCell(checkCurrentCell);
                GetAdjacentWaterCells();
                SelectTargetCell();
            }
            moveDir = (targetLeaf.transform.position - transform.position).normalized;
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

        // if there is algae to hide in, then hide in it
        if (mood == PreyMood.Escape && GetCurrentDepth() == 1)
        {
            Sand sand = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y - 1))) as Sand;
            if (sand != null && sand.GetCurrentAlgae() != null && sand.GetCurrentAlgae().GetCurrentGrowthLevel() >= GrowthLevel.Mature)
            {
                HideIn(sand.GetCurrentAlgae());
            }

        }
    }
    private void HideIn(Algae algae)
    {
        algae.AddHiddenPrey(this);
        gameObject.SetActive(false);
        transform.position = new Vector3(9999, 9999, 0);
        GetCurrentCell().RemoveFish(this);
    }
    public void GetOutOfAlgae(Algae algae)
    {
        transform.position = algae.transform.position;
        gameObject.SetActive(true);
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);
        targetPos = GetCurrentCell().GetPosition();
        escapeTimer = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Leaf leaf = collision.gameObject.GetComponent<Leaf>();
        if (leaf != null && leaf == targetLeaf)
        {
            Destroy(leaf.gameObject);
            targetLeaf = null;
            hungerPoints += leafFeedPoint;
        }
    }
    private void OnDestroy()
    {
        if (GetHealthStatus() != HealthStatus.Dead)
        {
            GameManager.Instance.DecreaseAlivePreyCount();
        }
        GetCurrentCell().RemoveFish(this);

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Leaf leaf = collision.gameObject.GetComponent<Leaf>();
        if (leaf != null && leaf == targetLeaf)
        {
            Destroy(leaf.gameObject);
            targetLeaf = null;
            hungerPoints += leafFeedPoint;
        }
    }

    public void ActivateEscapeMode(Water predatorCell)
    {
        this.predatorCell = predatorCell;
        // extra speed increase will be applied only when it is first encounter with the predator
        if (mood != PreyMood.Escape)
        {
            SetSpeed(escapeSpeed + 2f);
        }
        // make it select the safer direction when the prey is at the bottom of the aquarium
        if (GetCurrentCell().GetDepth() > 1)
        {
            Water cellOnLeft = GridManager.GetCellOnLeft(GetCurrentCell()) as Water;
            Water cellOnRight = GridManager.GetCellOnRight(GetCurrentCell()) as Water;
            if (cellOnLeft != null && cellOnRight != null)
            {
                if (cellOnLeft.GetPredatorList().Count != 0)
                {
                    moveDirection = Direction.Right;
                }
                else if (cellOnRight.GetPredatorList().Count != 0)
                {
                    moveDirection = Direction.Left;
                }
                else
                {
                    if (cellOnLeft.GetPredatorExistencePossibility() > cellOnRight.GetPredatorExistencePossibility())
                    {
                        moveDirection = Direction.Right;
                    }
                    else
                    {
                        moveDirection = Direction.Left;
                    }
                }

            }
            else if (cellOnLeft != null)
            {

                moveDirection = Direction.Left;
            }
            else if (cellOnRight != null)
            {
                moveDirection = Direction.Right;
            }
            else
            {
                Debug.LogWarning("Both left and right cells are null !!!");
            }
        }
        mood = PreyMood.Escape;
        GetAdjacentWaterCells();
        SelectTargetCell();
        escapeTimer = escapeTime;
    }
    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }
    public void SetCanLayEgg(bool val)
    {
        canLayEgg = val;
    }
    public bool CanLayEgg()
    {
        return canLayEgg;
    }
    public Maturity GetMaturity()
    {
        return maturity;
    }


}
