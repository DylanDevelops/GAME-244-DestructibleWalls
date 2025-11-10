using System;
using TMPro;
using UnityEngine;

public class DestructibleWallsDemoGameView : MonoBehaviour
{
    public TMP_Text velocityText;
    public TMP_Text glassText;
    public TMP_Text woodText;
    
    private void Start()
    {
        SetVelocityText(0);
    }

    public void SetVelocityText(float velocity)
    {
        velocityText.text = $"Player Velocity: {velocity:F2}";
    }

    public void SetGlassVisualDisplayText(float velocity)
    {
        glassText.text = $"Glass\nVelocity: {velocity}";
    }
    
    public void SetWoodVisualDisplayText(float velocity)
    {
        woodText.text = $"Wood\nVelocity: {velocity}";
    }
}
