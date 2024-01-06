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
    Escape
}
public class Prey : Fish
{
    [SerializeField] private PreyMood mood;
    [SerializeField] private Leaf targetLeaf;
    [SerializeField] private Direction moveDirection;

    private Vector3 targetPos;
    private float escapeTimer;
    private float escapeSpeed;
    private float defaultSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        BaseStart();
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);
        targetPos = GetCurrentCell().GetPosition();

        SetPreferredDepthMax(8);
        SetPreferredDepthMin(6);
        escapeSpeed = 2.5f;
        defaultSpeed = 1.5f;
    }
    private void Update()
    {
        BaseUpdate();
        if (GetHealthStatus() == HealthStatus.Dead)
        {
            spriteRenderer.color = Color.black;

            HandleDeadMovement();
            return;
        }
        hungerPoints -= Time.deltaTime;
        if (hungerPoints < 0)
        {
            SetHealthStatus(HealthStatus.Dead);
            GetCurrentCell().RemovePrey(this);
            return;
        }
        if (escapeTimer > 0)
        {
            escapeTimer -= Time.deltaTime;
            mood = PreyMood.Escape;
            SetSpeed(escapeSpeed);
        }
        else if (hungerPoints < 50)
        {
            mood = PreyMood.Hungry;
            SetSpeed(defaultSpeed);
        }
        else
        {
            mood = PreyMood.Calm;
            SetSpeed(defaultSpeed);
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
    private void SelectTargetCell()
    {
        if (adjacentWaterCellList.Count == 0)
        {
            return;
        }
        Color color = Color.white;
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

            switch (mood)
            {
                case PreyMood.Calm:
                    if (adjacentWaterCellList[random].GetPredatorExistencePossibility() > 0)
                    {
                        ActivateEscapeMode();
                        GetAdjacentWaterCells();
                        continue;
                    }
                    if (adjacentWaterCellList[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                        Debug.Log("Predator detected: " + adjacentWaterCellList[random].GetPredatorExistencePossibility());
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
                    color = Color.red;
                    if (GetCurrentCell().GetDepth() > 1)
                    {
                        if (adjacentWaterCellList[random].GetPredatorList().Count == 0 && adjacentWaterCellList[random].GetDepth() < GetCurrentCell().GetDepth())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
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
        spriteRenderer.color = color;

    }
    private void HandleMovement()
    {
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
            hungerPoints += 5;
        }
    }

    public void ActivateEscapeMode()
    {
        mood = PreyMood.Escape;
        escapeTimer = 7;

    }



}
