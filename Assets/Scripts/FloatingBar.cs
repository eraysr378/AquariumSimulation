
using UnityEngine;
using UnityEngine.UI;

public class FloatingBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 offset;
    private float currentVal;
    private float maxVal;

    private Slider barSlider;
    private Gradient gradient;
    private void Awake()
    {
        SetGradient();
        barSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = targetTransform.position + offset;
        barSlider.value = currentVal / maxVal;
        fillImage.color = gradient.Evaluate(currentVal / maxVal);
        currentVal += Time.deltaTime;
        if(currentVal >= maxVal)
        {
            Hide();
        }
    }
    public void FillBar(float time)
    {
        currentVal = 0;
        maxVal = time;
        Show();
    }

    private void SetGradient()
    {
        gradient = new Gradient();

        // Blend color from red at 0% to green at 100%
        GradientColorKey[] colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.red, 0.0f);
        colors[1] = new GradientColorKey(Color.green, 1.0f);

        GradientAlphaKey[] alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 0.0f);
        gradient.SetKeys(colors, alphas);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
