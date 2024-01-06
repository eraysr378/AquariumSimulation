using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private float maxAlgaeAmount;
    [SerializeField] private float currentAlgaeAmount;
    [SerializeField] private Transform hunterPrefab;
    [SerializeField] private Transform preyPrefab;
    [SerializeField] private Transform crabPrefab;
    [SerializeField] private Transform scavengerPrefab;


    // Start is called before the first frame update
    float timer = 0;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {


        for (int i = 0; i < 5; i++)
        {
            Fish fish = Instantiate(preyPrefab, new Vector3(5+i*3, 5, 0), Quaternion.identity).GetComponent<Fish>();
            fish.SetHungePoints(Random.Range(30, 70));

        }
        for (int i = 0; i < 1; i++)
        {
            Instantiate(hunterPrefab, new Vector3(12, 2, 0), Quaternion.identity);
        }
        for (int i = 0; i < 1; i++)
        {
            Instantiate(crabPrefab, new Vector3(5, 0, 0), Quaternion.identity);
        }
        for (int i = 0; i < 1; i++)
        {
            Instantiate(scavengerPrefab, new Vector3(5, 10, 0), Quaternion.identity);
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        //timer += Time.deltaTime;
        //if (timer >= 0.1f)
        //{
        //    Instantiate(hunterPrefab, new Vector3(5, 2, 0), Quaternion.identity);
        //    timer = 0;
        //}
    }
    public bool IsAlgaeSpawnable()
    {
        if (currentAlgaeAmount < maxAlgaeAmount)
        {
            return true;
        }
        return false;
    }
    public void IncreaseCurrentAlgaeAmount()
    {
        currentAlgaeAmount++;
    }
    public void DecreaseCurrentAlgaeAmount()
    {
        currentAlgaeAmount--;
    }
}
