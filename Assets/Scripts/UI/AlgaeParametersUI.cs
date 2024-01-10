
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlgaeParametersUI : MonoBehaviour
{
    [SerializeField] private Slider seedToYoungTimeSlider;
    [SerializeField] private Slider youngToMatureTimeSlider;
    [SerializeField] private Slider matureToRottenTimeSlider;
    [SerializeField] private Slider rottenToPoisonousTimeSlider;
    [SerializeField] private Slider poisonousToDeadTimeSlider;
    [SerializeField] private Slider poisonPossibilitySlider;
    [SerializeField] private Slider poisonSpreadTimeSlider;
    [SerializeField] private Slider leafSpawnTimeSlider;


    [SerializeField] private TextMeshProUGUI seedToYoungTimeText;
    [SerializeField] private TextMeshProUGUI youngToMatureTimeText;
    [SerializeField] private TextMeshProUGUI matureToRottenTimeText;
    [SerializeField] private TextMeshProUGUI rottenToPoisonousTimeText;
    [SerializeField] private TextMeshProUGUI poisonousToDeadTimeText;
    [SerializeField] private TextMeshProUGUI poisonPossibilityText;
    [SerializeField] private TextMeshProUGUI poisonSpreadTimeText;
    [SerializeField] private TextMeshProUGUI leafSpawnTimeText;
    private void Awake()
    {
        seedToYoungTimeSlider.onValueChanged.AddListener((float x) => { SetSeedToYoungTime(); });
        youngToMatureTimeSlider.onValueChanged.AddListener((float x) => { SetYoungToMatureTime(); });
        matureToRottenTimeSlider.onValueChanged.AddListener((float x) => { SetMatureToRottenTime(); });
        rottenToPoisonousTimeSlider.onValueChanged.AddListener((float x) => { SetRottenToPoisonousTime(); });
        poisonousToDeadTimeSlider.onValueChanged.AddListener((float x) => { SetPoisonousToDeadTime(); });
        poisonPossibilitySlider.onValueChanged.AddListener((float x) => { SetPoisonPossibility(); });
        poisonSpreadTimeSlider.onValueChanged.AddListener((float x) => { SetPoisonSpreadTime(); });
        leafSpawnTimeSlider.onValueChanged.AddListener((float x) => { SetLeafSpawnTime(); });
    }
    void Start()
    {
        SetSeedToYoungTime();
        SetYoungToMatureTime();
        SetMatureToRottenTime();
        SetRottenToPoisonousTime();
        SetPoisonousToDeadTime();
        SetPoisonPossibility();
        SetPoisonSpreadTime();
        SetLeafSpawnTime();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void SetSeedToYoungTime()
    {
        AlgaeParameters.SeedToYoungTime = seedToYoungTimeSlider.value;
        seedToYoungTimeText.text = seedToYoungTimeSlider.value.ToString("0.00");

    }
    private void SetYoungToMatureTime()
    {
        AlgaeParameters.YoungToMatureTime = youngToMatureTimeSlider.value;
        youngToMatureTimeText.text = youngToMatureTimeSlider.value.ToString("0.00");

    }
    private void SetMatureToRottenTime()
    {
        AlgaeParameters.MatureToRottenTime = matureToRottenTimeSlider.value;
        matureToRottenTimeText.text = matureToRottenTimeSlider.value.ToString("0.00");
    }
    private void SetRottenToPoisonousTime()
    {
        AlgaeParameters.RottenToPoisonousTime = rottenToPoisonousTimeSlider.value;
        rottenToPoisonousTimeText.text = rottenToPoisonousTimeSlider.value.ToString("0.00");
    }
    private void SetPoisonousToDeadTime()
    {
        AlgaeParameters.PoisonousToDeadTime = poisonousToDeadTimeSlider.value;
        poisonousToDeadTimeText.text = poisonousToDeadTimeSlider.value.ToString("0.00");
    }
    private void SetPoisonPossibility()
    {
        AlgaeParameters.PoisonPossibility = poisonPossibilitySlider.value;
        poisonPossibilityText.text = poisonPossibilitySlider.value.ToString("0.00");
    }
    private void SetPoisonSpreadTime()
    {
        AlgaeParameters.PoisonSpreadTime = poisonSpreadTimeSlider.value;
        poisonSpreadTimeText.text = poisonSpreadTimeSlider.value.ToString("0.00");
    }
    private void SetLeafSpawnTime()
    {
        AlgaeParameters.LeafSpawnTime = leafSpawnTimeSlider.value;
        leafSpawnTimeText.text = leafSpawnTimeSlider.value.ToString("0.00");
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
