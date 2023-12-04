using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestChangePredatorBehaviour : MonoBehaviour
{
    public Predator predator;
    public Button moodChangeButton;
    public Button disableAutoMoodChangeButton;


    private void Start()
    {
        moodChangeButton.onClick.AddListener(() =>
        {
            predator.SetMood(Mood.ActivePredation);
        });
 
    }


}
