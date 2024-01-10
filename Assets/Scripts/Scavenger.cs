using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scavenger : Fish
{
    private enum ScavengerMood
    {
        Calm,
        Hungry
    }
    [SerializeField] private Fish targetDeadFish;
    [SerializeField] private Leaf targetLeaf;
    [SerializeField] private ScavengerMood scavengerMood;
    private Vector3 targetPos;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetTargetCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water);
        targetPos = GetCurrentCell().GetPosition();
        SetPreferredDepthMin(GridManager.Instance.GetHeight() - 2);
        SetPreferredDepthMax(GridManager.Instance.GetHeight() - 1);
    }

    // Update is called once per frame
    void Update()
    {
        hungerPoints -= Time.deltaTime;
        if (hungerPoints < 50)
        {
            scavengerMood = ScavengerMood.Hungry;
        }
        else
        {
            scavengerMood = ScavengerMood.Calm;
        }


        HandleMovement();
    }
    private void HandleMovement()
    {
        float rotationSpeed = 5;
        Vector3 moveDir;
        float angle;


        if (targetDeadFish != null)
        {

            Cell checkCurrentCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
            if (checkCurrentCell != null && checkCurrentCell != GetCurrentCell())
            {
                SetCurrentCell(checkCurrentCell);
            }

            moveDir = (targetDeadFish.transform.position - transform.position).normalized;
            transform.position += moveDir * GetSpeed() * Time.deltaTime;
            angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(transform.eulerAngles.z - angle) > Mathf.Abs(transform.eulerAngles.z - (360 + angle)))
            {
                angle = 360 + angle;
            }
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
            return;
        }
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
        // if current cell has a dead fish or leaf then make the fish target it, otherwise the fish passes over them
        if (targetDeadFish == null && GetCurrentCell().GetDeadFishList().Count > 0)
        {
            targetDeadFish = GetCurrentCell().GetDeadFishList().First();
        }
        else if (targetLeaf == null && GetCurrentCell().GetLeafList().Count > 0)
        {
            targetLeaf = GetCurrentCell().GetLeafList().First();
        }
        Water candidateTargetCell = adjacentWaterCellList[0];
        adjacentWaterCellList.RemoveAt(0);
        while (adjacentWaterCellList.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, adjacentWaterCellList.Count);

            switch (scavengerMood)
            {
                case ScavengerMood.Calm:
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
                case ScavengerMood.Hungry:
                    if (targetDeadFish == null && adjacentWaterCellList[random].GetDeadFishList().Count > 0)
                    {
                        targetDeadFish = adjacentWaterCellList[random].GetDeadFishList().First();
                    }
                    else if (targetLeaf == null && adjacentWaterCellList[random].GetLeafList().Count > 0)
                    {
                        targetLeaf = adjacentWaterCellList[random].GetLeafList().First();
                    }
                    else if (adjacentWaterCellList[random].GetDeadFishExistencePossibility() > candidateTargetCell.GetDeadFishExistencePossibility())
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                    }
                    else if (candidateTargetCell.GetDeadFishExistencePossibility() == 0 && adjacentWaterCellList[random].GetLeafExistencePossiblity() > candidateTargetCell.GetLeafExistencePossiblity())
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                    }
                    else if (candidateTargetCell.GetDeadFishExistencePossibility() <= 0 && candidateTargetCell.GetLeafExistencePossiblity() <= 0)
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
                default:
                    break;
            }
        }
        if(scavengerMood == ScavengerMood.Hungry)
        {
            GetCurrentCell().ScavengerPassedThrough();
        }
        SetTargetCell(candidateTargetCell);
        spriteRenderer.color = color;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish fish = collision.gameObject.GetComponent<Fish>();
        if (fish != null && fish.GetHealthStatus() == HealthStatus.Dead)
        {
            Destroy(fish.gameObject);
            targetDeadFish = null;
            hungerPoints += 30;
        }
        Leaf leaf = collision.gameObject.GetComponent<Leaf>();
        if (leaf != null && leaf == targetLeaf)
        {
            Destroy(leaf.gameObject);
            targetLeaf = null;
            hungerPoints += 5;
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Fish fish = collision.gameObject.GetComponent<Fish>();
        if (fish != null && fish.GetHealthStatus() == HealthStatus.Dead)
        {
            Destroy(fish.gameObject);
            targetDeadFish = null;
            hungerPoints += 30;
        }
        Leaf leaf = collision.gameObject.GetComponent<Leaf>();
        if (leaf != null && leaf == targetLeaf)
        {
            Destroy(leaf.gameObject);
            targetLeaf = null;
            hungerPoints += 5;
        }
    }
}
