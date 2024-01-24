using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] private TextMeshProUGUI alivePreyCountText;
    [SerializeField] private ShowExistencesUI showExistencesUI;
    [SerializeField] float alivePreyCount;
    //[SerializeField] private TextMeshProUGUI debugText;

    private float simulationEndTime;
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
            Prey prey = Instantiate(preyPrefab, new Vector3(5 + i * 3, 5, 0), Quaternion.identity).GetComponent<Prey>();
            prey.SetHungePoints(Random.Range(40, 60));
            prey.SetMoveDirection((Direction)Random.Range(0, 2));
            alivePreyCount++;
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
        simulationEndTime = gameTime;
        //debugText.text = "simulation count: " + GlobalVariables.simulationCount + "\nmax time: " + GlobalVariables.maxBalancedTime;
        //gameOverUI.Show();
        //Invoke("LoadAgain", 3f);
    }
    private void LoadAgain()
    {
        GlobalVariables.simulationCount++;
        if(simulationEndTime > GlobalVariables.maxBalancedTime)
        {
            GlobalVariables.maxBalancedTime = simulationEndTime;
        }
        SceneManager.LoadScene(1);

    }
    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (alivePreyCount > 15 && !endSimulationButton.gameObject.activeSelf)
        {

            endSimulationButton.gameObject.SetActive(true);
            simulationEndTime = gameTime;
            //debugText.text = "simulation count: " + GlobalVariables.simulationCount + "\nmax time: " + GlobalVariables.maxBalancedTime;
            //gameOverUI.Show();
            //Invoke("LoadAgain", 3f);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                showExistencesUI.Show();
                Time.timeScale = 0;
            }
            else
            {
                showExistencesUI.Hide();
                Time.timeScale = 1;
            }
        }
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
    public float GetSimulationEndTime()
    {
        return simulationEndTime;
    }
    public void IncreaseAlivePreyCount()
    {
        alivePreyCount++;
        alivePreyCountText.text = "Alive Prey: " + alivePreyCount;
    }
    public void DecreaseAlivePreyCount()
    {
        alivePreyCount--;
        alivePreyCountText.text = "Alive Prey: " + alivePreyCount;
    }
}
