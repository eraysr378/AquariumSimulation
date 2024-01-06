using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowExistencesUI : MonoBehaviour
{
    [SerializeField] private Button preyExistenceButton;
    [SerializeField] private Button predatorExistenceButton;
    [SerializeField] private Button leafExistenceButton;
    [SerializeField] private Button poisonousnessButton;
    [SerializeField] private Button deadFishExistenceButton;


    private void Awake()
    {
        preyExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowPreyExistence();
        }); 
        predatorExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowPredatorExistence();
        });
        leafExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowLeafExistence();
        });
        poisonousnessButton.onClick.AddListener(() =>
        {
            Water.ShowPoisonousness();
        });
        deadFishExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowDeadFishExistence();
        });
    }


}
