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
            mood.text = "mood: " + predator.GetMood().ToString();
            hungerPoints.text = "hunger: " + predator.GetHungerPoints().ToString("0.0");
            healthStatus.text = "healthStatus: " + predator.GetHealthStatus().ToString();
            speed.text = "speed: " + predator.GetSpeed().ToString("0.00");
        }

    }


}
