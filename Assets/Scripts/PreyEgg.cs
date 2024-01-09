using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyEgg : MonoBehaviour
{
    private enum EggMaturity
    {
        New,
        Young,
        Developed
    }
    [SerializeField] private EggMaturity eggMaturity;
    [SerializeField] private float scalingAmount;
    [SerializeField] private Prey preyPrefab;
    [SerializeField] private Water currentCell;
    float timer;
    void Start()
    {
        scalingAmount = 1.2f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(eggMaturity == EggMaturity.New && timer > 3)
        {
            timer = 0;
            eggMaturity = EggMaturity.Young;
            transform.localScale *= scalingAmount;
        }
        else if (eggMaturity == EggMaturity.Young && timer > 3)
        {
            timer = 0;
            eggMaturity = EggMaturity.Developed;
            transform.localScale *= scalingAmount;
        }
        else if (eggMaturity == EggMaturity.Developed && timer > 3)
        {
            SpawnPrey();
            Destroy(gameObject);
        }
        HandleMovement();
    }
    private void HandleMovement()
    {
        if(transform.position.y - currentCell.transform.position.y + 0.5f > 0.05f)
        {
            transform.position -= new Vector3(0,Time.deltaTime,0);
        }

    }
    private void SpawnPrey()
    {
        Prey prey = Instantiate(preyPrefab, currentCell.transform.position, Quaternion.identity);
        prey.SetHungePoints(80);

    }
    public void SetCurrentCell(Water cell)
    {
        currentCell = cell;
    }
}
