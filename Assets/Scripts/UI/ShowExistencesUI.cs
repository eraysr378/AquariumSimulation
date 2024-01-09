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

    List<Button> buttonList;
    private void Awake()
    {
        buttonList = new List<Button>
        {
            predatorExistenceButton,
            preyExistenceButton,
            leafExistenceButton,
            deadFishExistenceButton,
            poisonousnessButton
        };
        preyExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowPreyExistence();
            ResetButtonVisuals();
            ButtonClicked(preyExistenceButton);
        });
        predatorExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowPredatorExistence();
            ResetButtonVisuals();
            ButtonClicked(predatorExistenceButton);
        });
        leafExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowLeafExistence();
            ResetButtonVisuals();
            ButtonClicked(leafExistenceButton);
        });
        poisonousnessButton.onClick.AddListener(() =>
        {
            Water.ShowPoisonousness();
            ResetButtonVisuals();
            ButtonClicked(poisonousnessButton);
        });
        deadFishExistenceButton.onClick.AddListener(() =>
        {
            Water.ShowDeadFishExistence();
            ResetButtonVisuals();
            ButtonClicked(deadFishExistenceButton);
        });

        ButtonClicked(preyExistenceButton);
    }
    private void ResetButtonVisuals()
    {
        foreach (Button button in buttonList)
        {
            button.image.color = new Color32(50, 50, 50, 255);
        }
    }
    private void ButtonClicked(Button button)
    {
        if (button.image.color.a == 1)
        {
            button.image.color = new Color32(55, 50, 50, 180);
        }

    }


}
