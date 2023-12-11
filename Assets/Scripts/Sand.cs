using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sand : Cell
{

    [SerializeField] private float algaeEmergencePossibility;
    [SerializeField] private Transform algaePrefab;
    [SerializeField] private Algae currentAlgae;
    [SerializeField] private Vector3 algaeSpawnOffset;
    [SerializeField] private float algaeExistencePossibility;


    private float timer;
    private void Start()
    {
        algaeSpawnOffset = new Vector3(0, 0.5f, 0);
        algaeEmergencePossibility = 50;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 && currentAlgae == null)
        {
            TrySpawningAlgae();
            timer = 0;
        }
    }
    private void TrySpawningAlgae()
    {
        bool spawnable = true;

        Sand leftCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x - 1, GetPosition().y));
        Sand rightCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x + 1, GetPosition().y));
        if (leftCell != null && leftCell.currentAlgae != null)
        {
            spawnable = false;
        }
        if (rightCell != null && rightCell.currentAlgae != null)
        {
            spawnable = false;
        }
        if (spawnable)
        {
            if (Random.Range(0, 101) < algaeEmergencePossibility && GameManager.Instance.IsAlgaeSpawnable())
            {
                currentAlgae = Instantiate(algaePrefab, transform.position + algaeSpawnOffset, Quaternion.identity).GetComponent<Algae>();
                GameManager.Instance.IncreaseCurrentAlgaeAmount();
                InformRightCellAlgaeSpawned(2);
                InformLeftCellAlgaeSpawned(2);
            }
        }

    }
    private void InformRightCellAlgaeDestroyed(float possibility)
    {
        Sand rightCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x + 1, GetPosition().y));

        if (possibility < 0 && rightCell != null)
        {
            rightCell.algaeExistencePossibility += possibility;
            rightCell.InformRightCellAlgaeDestroyed(possibility + 1);
        }
    }
    private void InformLeftCellAlgaeDestroyed(float possibility)
    {
        Sand leftCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x - 1, GetPosition().y));

        if (possibility < 0 && leftCell != null)
        {
            leftCell.algaeExistencePossibility += possibility;
            leftCell.InformRightCellAlgaeDestroyed(possibility + 1);
        }
    }
    private void InformRightCellAlgaeSpawned(float possibility)
    {
        Sand rightCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x + 1, GetPosition().y));

        if (possibility > 0 && rightCell != null)
        {
            rightCell.algaeExistencePossibility += possibility;
            rightCell.InformRightCellAlgaeSpawned(possibility - 1);
        }


    }
    private void InformLeftCellAlgaeSpawned(float possibility)
    {
        Sand leftCell = (Sand)GridManager.GetCellAtPosition(new Vector2(GetPosition().x - 1, GetPosition().y));

        if (possibility > 0 && leftCell != null)
        {
            leftCell.algaeExistencePossibility += possibility;
            leftCell.InformRightCellAlgaeSpawned(possibility - 1);
        }

    }
    public void SpreadPoison()
    {
        Water upCell = (Water)GridManager.GetCellAtPosition(new Vector2(GetPosition().x, GetPosition().y + 1));
        upCell.IncreasePoisonInvolved();

    }
    public Algae GetCurrentAlgae()
    {
        return currentAlgae;
    }
    public void SetCurrentAlgae(Algae algae)
    {
        currentAlgae = algae;
    }
    public void RemoveCurrentAlgae()
    {
        InformLeftCellAlgaeDestroyed(-2);
        InformRightCellAlgaeDestroyed(-2);
        currentAlgae = null;

    }
    public float GetAlgaeExistencePossibility()
    {
        return algaeExistencePossibility;
    }
}
