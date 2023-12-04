using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager Instance { get; private set; }
    public static int Generation;
    public event EventHandler OnGenerationChanged;


    [SerializeField] private TextMeshProUGUI generationText;

    private float timer;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= 0.25f)
        //{
        //    Generation++;
        //    generationText.text = "Generation: " + Generation.ToString();
        //    timer = 0;
        //    OnGenerationChanged?.Invoke(this, EventArgs.Empty);
        //}
        generationText.text = "fps: " + 1 / Time.deltaTime;

    }
}
