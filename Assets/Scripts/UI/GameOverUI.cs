using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private float gameTime;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button menuButton;
    private void Awake()
    {
        menuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ParameterSelectScene");
        });
  
    }
    void Start()
    {
        Hide();
    }

    void Update()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        gameOverText.text = "Aquarium balance is lost after " + GameManager.Instance.GetSimulationEndTime().ToString("0") + " seconds.";
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
