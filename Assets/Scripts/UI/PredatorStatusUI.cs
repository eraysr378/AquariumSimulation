using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredatorStatusUI : MonoBehaviour
{
    public Predator predator;
    public TextMeshProUGUI mood;
    public TextMeshProUGUI hungerPoints;
    public TextMeshProUGUI healthStatus;
    public TextMeshProUGUI speed;


    private void Start()
    {


    }
    private void Update()
    {
        if (predator == null)
        {
            predator = FindAnyObjectByType<Predator>();
        }
        else
        {
            mood.text = "Mood: " + predator.GetMood().ToString();
            hungerPoints.text = "Hunger: " + predator.GetHungerPoints().ToString("0.0");
            healthStatus.text = "HealthStatus: " + predator.GetHealthStatus().ToString();
            speed.text = "Speed: " + predator.GetSpeed().ToString("0.00");
        }

    }


}
