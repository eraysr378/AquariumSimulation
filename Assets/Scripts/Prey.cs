using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Prey : Fish
{
    [SerializeField] private bool isTargetCellSelectionStarted;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        SetPreferredDepthMax(5);
        SetPreferredDepthMin(2);
    }
    private void Update()
    {
        HandleMovement();
    }

    private IEnumerator SelectTargetCell()
    {
        if (adjacentCells.Count == 0)
        {
            isTargetCellSelectionStarted = false;
            yield break;
        }
        Color color = Color.white;
        Cell candidateTargetCell = adjacentCells[0];
        adjacentCells.RemoveAt(0);
        for (int i = 0; i < adjacentCells.Count; i++)
        {
            int random = UnityEngine.Random.Range(0, adjacentCells.Count);
            if (adjacentCells[random].GetPredatorExistencePossibility() > 0)
            {
                color = Color.red;
            }
            if (adjacentCells[random].GetPredatorExistencePossibility() < candidateTargetCell.GetPredatorExistencePossibility())
            {
                candidateTargetCell = adjacentCells[random];
                Debug.Log("Predator existence possibility: " + adjacentCells[random].GetPredatorExistencePossibility());

            }
            else if (candidateTargetCell.GetPredatorExistencePossibility() == 0)
            {
                if (Cell.IsDepthBetween(adjacentCells[random].GetDepth(), GetPreferredDepthMin(), GetPreferredDepthMax()) && adjacentCells[random].GetPredatorExistencePossibility() == 0)
                {
                    candidateTargetCell = adjacentCells[random];
                }
                else if (adjacentCells[random].GetPredatorExistencePossibility() == 0)
                {
                    if (GetCurrentDepth() > GetPreferredDepthMax() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMax()) > Mathf.Abs(adjacentCells[random].GetDepth() - GetPreferredDepthMax()))
                    {
                        candidateTargetCell = adjacentCells[random];
                    }
                    else if (GetCurrentDepth() < GetPreferredDepthMin() && Mathf.Abs(candidateTargetCell.GetDepth() - GetPreferredDepthMin()) > Mathf.Abs(adjacentCells[random].GetDepth() - GetPreferredDepthMin()))
                    {
                        candidateTargetCell = adjacentCells[random];
                    }
                }
            }

            adjacentCells.RemoveAt(random);
            yield return null;

        }
        SetTargetCell ( candidateTargetCell);
        spriteRenderer.color = color;

    }
    private void HandleMovement()
    {
        if (GetTargetCell() == null && !isTargetCellSelectionStarted)
        {
            isTargetCellSelectionStarted = true;
            GetAdjacentCells();
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
            float rotationSpeed = 10f;
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * rotationSpeed);
        }

    }


 
 

}
