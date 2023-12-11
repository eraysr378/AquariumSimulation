using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestChangePredatorBehaviour : MonoBehaviour
{
    public Predator predator;
    public Button moodChangeButton;
    public Button disableAutoMoodChangeButton;
    public TextMeshProUGUI mood;
    public TextMeshProUGUI hungerPoints;
    public TextMeshProUGUI healthStatus;


    private void Start()
    {
        moodChangeButton.onClick.AddListener(() =>
        {
            predator.SetMood(Mood.ActivePredation);
        });
 
    }
    private void Update()
    {
        if(predator == null)
        {
            predator = FindAnyObjectByType<Predator>();
        }
        else
        {
            mood.text = predator.GetMood().ToString();
            hungerPoints.text = predator.GetHungerPoints().ToString();
            healthStatus.text = predator.GetHealthStatus().ToString();
        }

    }


}
