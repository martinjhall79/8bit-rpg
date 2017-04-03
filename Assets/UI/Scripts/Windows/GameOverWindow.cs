using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : GenericWindow
{
    public Text leftStatsLabel;
    public Text leftStatsValues;
    public Text rightStatsLabel;
    public Text rightStatsValues;
    public Text scoreValue;
    // Stat update animation
    public float statsDelay = 1f;
    public int totalStats = 6;
    public int statsPerColumn = 3;

    private int currentStat = 0;
    private float delay = 0;

    public void ClearText()
    {
        leftStatsLabel.text = "";
        leftStatsValues.text = "";
        rightStatsLabel.text = "";
        rightStatsValues.text = "";
        scoreValue.text = "";
    }

    public override void Open()
    {
        ClearText();
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        // Reset stat update animation counter
        currentStat = 0;
    }

    // Add new stat to label stat
    private void UpdateStatText(Text label, Text value)
    {
        label.text += "Stat " + currentStat + "\n";
        // Pad zeros to keep label format
        value.text += Random.Range(0, 1000).ToString("D4") + "\n";
    }

    private void ShowNextStat()
    {
        // Reached end of stats, display score
        if (currentStat > totalStats - 1)
        {
            scoreValue.text = Random.Range(0, 1000000000).ToString("D10");
            currentStat = -1;
            return;
        }

        // Test which stat we're up to so we know which column to display stat in
        if (currentStat < statsPerColumn)
        {
            UpdateStatText(leftStatsLabel, leftStatsValues);
        }
        else
        {
            UpdateStatText(rightStatsLabel, rightStatsValues);
        }

        currentStat++;
    }

    // Next button
    public void OnNext()
    {
        Debug.Log("Next button pressed");
    }

    // Don't automatically close start window
    protected override void Awake()
    {

    }

    void Start()
    {
        Open();
    }

    private void Update()
    {
        delay += Time.deltaTime;

        if (delay > statsDelay && currentStat != -1)
        {
            ShowNextStat();
            delay = 0;
        }
    }
}
