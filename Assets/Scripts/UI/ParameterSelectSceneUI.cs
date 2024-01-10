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
            preyParametersUI.SetActive(true);
            predatorParametersUI.SetActive(false);
            algaeParametersUI.SetActive(false);
        });
        predatorParametersButton.onClick.AddListener(() => {
            preyParametersUI.SetActive(false);
            predatorParametersUI.SetActive(true);
            algaeParametersUI.SetActive(false);
        });
        algaeParametersButton.onClick.AddListener(() => {
            preyParametersUI.SetActive(false);
            predatorParametersUI.SetActive(false);
            algaeParametersUI.SetActive(true);
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
