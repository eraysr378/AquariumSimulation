using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParameterSelectSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button preyParametersButton;
    [SerializeField] private Button predatorParametersButton;
    [SerializeField] private Button algaeParametersButton;
    [SerializeField] private GameObject preyParametersUI;
    [SerializeField] private GameObject predatorParametersUI;
    [SerializeField] private GameObject algaeParametersUI;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
        preyParametersButton.onClick.AddListener(() => {
            ActivatePreyParametersUI();
           
        });
        predatorParametersButton.onClick.AddListener(() => {
            ActivatePredatorParametersUI();
        });
        algaeParametersButton.onClick.AddListener(() => {
            ActivateAlgaeParametersUI();
        });
    }
    private void Start()
    {
        preyParametersButton.image.color = new Color32(100, 100, 100, 130); // at the start prey parameters are shown
        
    }
    private void ActivatePreyParametersUI()
    {
        preyParametersUI.SetActive(true);
        predatorParametersUI.SetActive(false);
        algaeParametersUI.SetActive(false);
        preyParametersButton.image.color = new Color32(100, 100, 100, 130);
        predatorParametersButton.image.color = new Color32(100, 100, 100, 255);
        algaeParametersButton.image.color = new Color32(100, 100, 100, 255);

    }
    private void ActivatePredatorParametersUI()
    {
        preyParametersUI.SetActive(false);
        predatorParametersUI.SetActive(true);
        algaeParametersUI.SetActive(false);
        preyParametersButton.image.color = new Color32(100, 100, 100, 255);
        predatorParametersButton.image.color = new Color32(100, 100, 100, 130);
        algaeParametersButton.image.color = new Color32(100, 100, 100, 255);
    }
    private void ActivateAlgaeParametersUI()
    {
        preyParametersUI.SetActive(false);
        predatorParametersUI.SetActive(false);
        algaeParametersUI.SetActive(true);
        preyParametersButton.image.color = new Color32(100, 100, 100, 255);
        predatorParametersButton.image.color = new Color32(100 ,100, 100, 255);
        algaeParametersButton.image.color = new Color32(100, 100, 100, 130);
    }

}
