using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Water currentWaterCell;
    [SerializeField] private Water targetWaterCell;
    private Vector3 targetPos;
    private Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        SetCurrentCell((Water)GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))));
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        Water checkCell = GridManager.GetCellAtPosition(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y))) as Water;
        if (checkCell != null && currentWaterCell != checkCell)
        {
            SetCurrentCell(checkCell);
        }

        if (!(targetWaterCell != null && targetWaterCell != currentWaterCell))
        {
            targetWaterCell = GridManager.GetCellAtPosition(new Vector2(currentWaterCell.transform.position.x, currentWaterCell.transform.position.y + 1)) as Water;
        }
        if (targetWaterCell != null)
        {
            targetPos = targetWaterCell.GetPosition();
            moveDir = (targetPos - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;
        }
        else
        {
            targetPos = currentWaterCell.GetPosition();
            moveDir = (targetPos - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;
        }




        //if (targetWaterCell == null)
        //{
        //    targetWaterCell = (Water)GridManager.GetCellAtPosition(new Vector2(currentWaterCell.transform.position.x, currentWaterCell.transform.position.y + 1));
        //}
        //if (targetWaterCell != null && (Mathf.Abs(transform.position.x - targetWaterCell.GetPosition().x) < 0.1f && Mathf.Abs(transform.position.y - targetWaterCell.GetPosition().y) < 0.1f))
        //{
        //    SetCurrentCell(targetWaterCell);
        //    targetWaterCell = null;
        //}
        //if (targetWaterCell != null)
        //{
        //    Vector3 moveDir = (targetWaterCell.GetPosition() - transform.position).normalized;
        //    transform.position += moveDir * speed * Time.deltaTime;
        //}


    }
    public void SetTargetWaterCell(Water cell)
    {
        targetWaterCell = cell;
    }
    public void SetCurrentCell(Water cell)
    {
        if (currentWaterCell != null)
        {
            currentWaterCell.RemoveLeaf(this);
        }
        currentWaterCell = cell;
        currentWaterCell.AddLeaf(this);
    }
    private void OnDestroy()
    {
        if (currentWaterCell != null)
            currentWaterCell.RemoveLeaf(this);
    }

}
