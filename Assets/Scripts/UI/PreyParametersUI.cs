using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreyParametersUI : MonoBehaviour
{
    [SerializeField] private Slider defaultSpeedSlider;
    [SerializeField] private Slider escapingSpeedSlider;
    [SerializeField] private Slider neededTimeToLayEggSlider;
    [SerializeField] private Slider neededTimeToLayEggSliderIncreaseAmountSlider;
    [SerializeField] private Slider hungerLimitSlider;
    [SerializeField] private Slider eggLayingDurationSlider;
    [SerializeField] private Slider leafFeedPointSlider;
    [SerializeField] private Slider escapeTimeSlider;

    [SerializeField] private TextMeshProUGUI defaultSpeedText;
    [SerializeField] private TextMeshProUGUI escapingSpeedText;
    [SerializeField] private TextMeshProUGUI neededTimeToLayEggText;
    [SerializeField] private TextMeshProUGUI neededTimeToLayEggSliderIncreaseAmountText;
    [SerializeField] private TextMeshProUGUI hungerLimitText;
    [SerializeField] private TextMeshProUGUI eggLayingDurationText;
    [SerializeField] private TextMeshProUGUI leafFeedPointText;
    [SerializeField] private TextMeshProUGUI escapeTimeText;

    [SerializeField] private Button randomButton;
    // Start is called before the first frame update
    private void Awake()
    {
        defaultSpeedSlider.onValueChanged.AddListener((float x) => { SetDefaultSpeed(); });
        escapingSpeedSlider.onValueChanged.AddListener((float x) => { SetEscapingSpeed(); });
        neededTimeToLayEggSlider.onValueChanged.AddListener((float x) => { SetNeededTimeToLayEgg(); });
        neededTimeToLayEggSliderIncreaseAmountSlider.onValueChanged.AddListener((float x) => { SetNeededTimeToLayEggIncreseAmount(); });
        hungerLimitSlider.onValueChanged.AddListener((float x) => { SetHungerLimit(); });
        eggLayingDurationSlider.onValueChanged.AddListener((float x) => { SetEggLayingDuration(); });
        leafFeedPointSlider.onValueChanged.AddListener((float x) => { SetLeafFeedPoint(); });
        escapeTimeSlider.onValueChanged.AddListener((float x) => { SetEscapeTime(); });
        randomButton.onClick.AddListener(() => { FillWithRandomValues(); });
    }
    void Start()
    {
        SetDefaultSpeed();
        SetEscapingSpeed();
        SetEggLayingDuration();
        SetEscapeTime();
        SetHungerLimit();
        SetNeededTimeToLayEgg();
        SetNeededTimeToLayEggIncreseAmount();
        SetLeafFeedPoint();
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void FillWithRandomValues()
    {

        SetSliderRandom(defaultSpeedSlider);
        SetSliderRandom(escapingSpeedSlider);
        SetSliderRandom(neededTimeToLayEggSlider);
        SetSliderRandom(neededTimeToLayEggSliderIncreaseAmountSlider);
        SetSliderRandom(eggLayingDurationSlider);
        SetSliderRandom(hungerLimitSlider);
        SetSliderRandom(leafFeedPointSlider);
        SetSliderRandom(escapeTimeSlider);
    }
    private void SetSliderRandom(Slider slider)
    {
        slider.value = Random.Range(slider.minValue, slider.maxValue);

    }

    public void SetDefaultSpeed()
    {
        PreyParameters.DefaultSpeed = defaultSpeedSlider.value;
        defaultSpeedText.text = defaultSpeedSlider.value.ToString("0.00");
    }
    public void SetEscapingSpeed()
    {
        PreyParameters.EscapingSpeed = escapingSpeedSlider.value;
        escapingSpeedText.text = escapingSpeedSlider.value.ToString("0.00");
    }
    public void SetNeededTimeToLayEgg()
    {
        PreyParameters.NeededTimeToLayEgg = neededTimeToLayEggSlider.value;
        neededTimeToLayEggText.text = neededTimeToLayEggSlider.value.ToString("0.00");
    }
    public void SetNeededTimeToLayEggIncreseAmount()
    {
        PreyParameters.NeededTimeToLayEggIncreaseAmount = neededTimeToLayEggSliderIncreaseAmountSlider.value;
        neededTimeToLayEggSliderIncreaseAmountText.text = neededTimeToLayEggSliderIncreaseAmountSlider.value.ToString("0.00");
    }
    public void SetHungerLimit()
    {
        PreyParameters.HungerLimit = hungerLimitSlider.value;
        hungerLimitText.text = hungerLimitSlider.value.ToString("0.00");
    }
    public void SetEggLayingDuration()
    {
        PreyParameters.EggLayingDuration = eggLayingDurationSlider.value;
        eggLayingDurationText.text = eggLayingDurationSlider.value.ToString("0.00");
    }
    public void SetLeafFeedPoint()
    {
        PreyParameters.LeafFeedPoint = leafFeedPointSlider.value;
        leafFeedPointText.text = leafFeedPointSlider.value.ToString("0.00");
    }
    public void SetEscapeTime()
    {
        PreyParameters.EscapeTime = escapeTimeSlider.value;
        escapeTimeText.text = escapeTimeSlider.value.ToString("0.00");
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
