using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum PreyMood
{
    Calm,
    Hungry
}
public class Prey : Fish
{
    [SerializeField] private bool isTargetCellSelectionStarted;
    [SerializeField] private PreyMood mood;
    [SerializeField] private Leaf targetLeaf;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetPreferredDepthMax(5);
        SetPreferredDepthMin(3);
    }
    private void Update()
    {
        if (hungerPoints < 50)
        {
            mood = PreyMood.Hungry;
        }
        else
        {
            mood = PreyMood.Calm;
        }
        HandleMovement();
    }

    private IEnumerator SelectTargetCell()
    {
        if (adjacentWaterCellList.Count == 0)
        {
            isTargetCellSelectionStarted = false;
            yield break;
        }
        Color color = Color.white;
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
                        color = Color.red;
                    }
                    if (adjacentWaterCellList[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
                    {
                        candidateTargetCell = adjacentWaterCellList[random];
                        Debug.Log("Predator existence possibility: " + adjacentWaterCellList[random].GetPredatorExistencePossibility());
                    }
                    else if (candidateTargetCell.GetPredatorExistencePossibility() == 0)
                    {
                        if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()) && adjacentWaterCellList[random].GetPredatorExistencePossibility() == 0)
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
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
                    yield return null;
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
                        if (adjacentWaterCellList[random].GetPredatorExistencePossibility() > 0)
                        {
                            color = Color.red;
                        }
                        if (adjacentWaterCellList[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
                            Debug.Log("Predator existence possibility: " + adjacentWaterCellList[random].GetPredatorExistencePossibility());
                        }
                        else if (Cell.IsDepthBetween(adjacentWaterCellList[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()) && adjacentWaterCellList[random].GetPredatorExistencePossibility() == 0)
                        {
                            candidateTargetCell = adjacentWaterCellList[random];
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
                    yield return null;
                    break;
                default:
                    break;
            }
        }
        SetTargetCell(candidateTargetCell);
        spriteRenderer.color = color;

    }
    private void HandleMovement()
    {
        float rotationSpeed = 10f;

        if (targetLeaf != null)
        {
            Vector3 moveDir = (targetLeaf.transform.position - transform.position).normalized;
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





}
