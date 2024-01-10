
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredatorParametersUI : MonoBehaviour
{
    [SerializeField] private Slider defaultSpeedSlider;
    [SerializeField] private Slider predationSpeedSlider;
    [SerializeField] private Slider sickSpeedSlider;
    [SerializeField] private Slider hungerCoefficientSlider;
    [SerializeField] private Slider neededTimeToHealSlider;
    [SerializeField] private Slider hungerPointsToStopPredationSlider;
    [SerializeField] private Slider hungerPointsToActivatePredationSlider;
    [SerializeField] private Slider preyFeedPointSlider;


    [SerializeField] private TextMeshProUGUI defaultSpeedText;
    [SerializeField] private TextMeshProUGUI predationSpeedText;
    [SerializeField] private TextMeshProUGUI sickSpeedText;
    [SerializeField] private TextMeshProUGUI hungerCoefficientText;
    [SerializeField] private TextMeshProUGUI neededTimeToHealText;
    [SerializeField] private TextMeshProUGUI hungerPointsToStopPredationText;
    [SerializeField] private TextMeshProUGUI hungerPointsToActivatePredationText;
    [SerializeField] private TextMeshProUGUI preyFeedPointText;
    private void Awake()
    {
        defaultSpeedSlider.onValueChanged.AddListener((float x) => { SetDefaultSpeed(); });
        predationSpeedSlider.onValueChanged.AddListener((float x) => { SetPredationSpeed(); });
        sickSpeedSlider.onValueChanged.AddListener((float x) => { SetSickSpeed(); });
        hungerCoefficientSlider.onValueChanged.AddListener((float x) => { SetHungerCoefficient(); });
        neededTimeToHealSlider.onValueChanged.AddListener((float x) => { SetNeededTimeToHeal(); });
        hungerPointsToStopPredationSlider.onValueChanged.AddListener((float x) => { SetHungerPointsToStopPredation(); });
        hungerPointsToActivatePredationSlider.onValueChanged.AddListener((float x) => { SetHungerPointsToActivatePredation(); });
        preyFeedPointSlider.onValueChanged.AddListener((float x) => { SetPreyFeedPoint(); });
    }
    private void Start()
    {
        SetDefaultSpeed();
        SetPredationSpeed();
        SetSickSpeed();
        SetHungerCoefficient();
        SetNeededTimeToHeal();
        SetHungerPointsToActivatePredation();
        SetHungerPointsToStopPredation();
        SetPreyFeedPoint();
        Hide();
    }
    private void SetDefaultSpeed()
    {
        PredatorParameters.DefaultSpeed = defaultSpeedSlider.value;
        defaultSpeedText.text = defaultSpeedSlider.value.ToString("0.00");
    }
    private void SetPredationSpeed()
    {
        PredatorParameters.PredationSpeed = predationSpeedSlider.value;
        predationSpeedText.text = predationSpeedSlider.value.ToString("0.00");

    }
    private void SetSickSpeed()
    {
        PredatorParameters.SickSpeed = sickSpeedSlider.value;
        sickSpeedText.text = sickSpeedSlider.value.ToString("0.00");

    }
    private void SetHungerCoefficient()
    {
        PredatorParameters.HungerCoefficient = hungerCoefficientSlider.value;
        hungerCoefficientText.text = hungerCoefficientSlider.value.ToString("0.00");

    }
    private void SetNeededTimeToHeal()
    {
        PredatorParameters.NeededTimeToHeal = neededTimeToHealSlider.value;
        neededTimeToHealText.text = neededTimeToHealSlider.value.ToString("0.00");

    }
    private void SetHungerPointsToStopPredation()
    {
        PredatorParameters.HungerPointsToStopPredation = hungerPointsToStopPredationSlider.value;
        hungerPointsToStopPredationText.text = hungerPointsToStopPredationSlider.value.ToString("0.00");

    }
    private void SetHungerPointsToActivatePredation()
    {
        PredatorParameters.HungerPointsToActivatePredation = hungerPointsToActivatePredationSlider.value;
        hungerPointsToActivatePredationText.text = hungerPointsToActivatePredationSlider.value.ToString("0.00");

    }
    private void SetPreyFeedPoint()
    {
        PredatorParameters.PreyFeedPoint = preyFeedPointSlider.value;
        preyFeedPointText.text = preyFeedPointSlider.value.ToString("0.00");

    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
