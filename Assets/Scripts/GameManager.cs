using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private float maxAlgaeAmount;
    [SerializeField] private float currentAlgaeAmount;
    [SerializeField] private Transform hunterPrefab;
    [SerializeField] private Transform preyPrefab;
    [SerializeField] private Transform crabPrefab;
    [SerializeField] private Transform scavengerPrefab;
    [SerializeField] private Button endSimulationButton;
    [SerializeField] private GameOverUI gameOverUI;

    private float predatorDiedTime;
    private float gameTime;

    // Start is called before the first frame update
    float timer = 0;
    private void Awake()
    {
        Instance = this;
        endSimulationButton.onClick.AddListener(() =>
        {
            gameOverUI.Show();
            endSimulationButton.gameObject.SetActive(false);
        });
    }
    void Start()
    {
        endSimulationButton.gameObject.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            Prey prey = Instantiate(preyPrefab, new Vector3(5+i*3, 5, 0), Quaternion.identity).GetComponent<Prey>();
            prey.SetHungePoints(Random.Range(40, 60));
            prey.SetMoveDirection((Direction)Random.Range(0, 2));
        }
        for (int i = 0; i < 1; i++)
        {
            Predator predator = Instantiate(hunterPrefab, new Vector3(12, 2, 0), Quaternion.identity).GetComponent<Predator>();
            predator.SetMoveDirection((Direction)Random.Range(0, 2));
            predator.OnPredatorDied += Predator_OnPredatorDied;
        }
        for (int i = 0; i < 1; i++)
        {
            Instantiate(crabPrefab, new Vector3(5, 0, 0), Quaternion.identity);
        }
        for (int i = 0; i < 1; i++)
        {
            Scavenger scavenger = Instantiate(scavengerPrefab, new Vector3(5, 10, 0), Quaternion.identity).GetComponent<Scavenger>();
            scavenger.SetMoveDirection((Direction)Random.Range(0, 2));
        }
    }

    private void Predator_OnPredatorDied(object sender, System.EventArgs e)
    {
        endSimulationButton.gameObject.SetActive(true);
        predatorDiedTime = gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
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
    public float GetPredatorDiedTime()
    {
        return predatorDiedTime;
    }
}
