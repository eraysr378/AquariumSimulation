using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    private const string isWalking = "IsWalking";

    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private Sand currentSandCell;
    [SerializeField] private Sand cellOnLeft;
    [SerializeField] private Sand cellOnRight;
    [SerializeField] private Sand targetCell;
    [SerializeField] private float hungerPoints;
    [SerializeField] private Algae targetAlgae;

    void Start()
    {
        SetCurrentCell(GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
        animator = GetComponentInChildren<Animator>();
        hungerPoints = 70;
    }

    void Update()
    {
        hungerPoints -= Time.deltaTime;
        HandleMovement();

    }

    private void SelectTargetCell()
    {
        if (hungerPoints < 50)
        {
            if (cellOnLeft != null && cellOnLeft.GetCurrentAlgae() != null)
            {
                targetCell = cellOnLeft;
                targetAlgae = targetCell.GetCurrentAlgae();
            }
            else if (cellOnRight != null && cellOnRight.GetCurrentAlgae() != null)
            {
                targetCell = cellOnRight;
                targetAlgae = targetCell.GetCurrentAlgae();
            }
            else if (cellOnLeft != null && cellOnRight != null)
            {
                if (cellOnLeft.GetAlgaeExistencePossibility() > cellOnRight.GetAlgaeExistencePossibility())
                {
                    targetCell = cellOnLeft;
                }
                else
                {
                    targetCell = cellOnRight;
                }
            }
            else if (cellOnLeft != null && cellOnLeft.GetAlgaeExistencePossibility() > 0)
            {
                targetCell = cellOnLeft;
            }
            else if (cellOnRight != null && cellOnRight.GetAlgaeExistencePossibility() > 0)
            {
                targetCell = cellOnRight;
            }
            else
            {
                targetCell = UnityEngine.Random.Range(0, 2) == 0 ? cellOnLeft : cellOnRight;
            }

        }
        else
        {
            targetCell = UnityEngine.Random.Range(0, 2) == 0 ? cellOnLeft : cellOnRight;
        }



    }
    private void HandleMovement()
    {
        if (targetAlgae != null)
        {
            Vector3 moveDir = (targetAlgae.transform.position - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;
            return;
        }
        if (GetTargetCell() == null)
        {
            SelectTargetCell();
        }
        if (GetTargetCell() != null && (Mathf.Abs(transform.position.x - GetTargetCell().GetPosition().x) < 0.1f && Mathf.Abs(transform.position.y - GetTargetCell().GetPosition().y) < 0.1f))
        {
            SetCurrentCell(GetTargetCell());
            SetTargetCell(null);
        }
        if (GetTargetCell() != null)
        {
            animator.SetBool(isWalking, true);
            Vector3 moveDir = (GetTargetCell().GetPosition() - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;
        }

    }
    private void SetCurrentCell(Cell cell)
    {
        currentSandCell = (Sand)cell;
        cellOnLeft = (Sand)GridManager.GetCellAtPosition(new Vector2(currentSandCell.GetPosition().x - 1, currentSandCell.GetPosition().y));
        cellOnRight = (Sand)GridManager.GetCellAtPosition(new Vector2(currentSandCell.GetPosition().x + 1, currentSandCell.GetPosition().y));
    }
    public Cell GetTargetCell()
    {
        return targetCell;
    }
    public void SetTargetCell(Cell cell)
    {
        targetCell = (Sand)cell;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Algae algae = collision.gameObject.GetComponent<Algae>();
        if (algae != null && algae == targetAlgae)
        {
            algae.EatenByCrab();
            Destroy(algae.gameObject);
            targetAlgae = null;
            hungerPoints += 35;
        }
    }
}
