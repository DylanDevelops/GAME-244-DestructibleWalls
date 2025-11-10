using Ravel.DestructibleWalls;
using UnityEngine;

public class DestructibleWallsDemoGameController : MonoBehaviour
{
    [SerializeField] private DestructibleWallsDemoGameView gameView;
    [SerializeField] private DestructibleWallConfig glassConfig;
    [SerializeField] private DestructibleWallConfig woodConfig;

    private PlayerController player;
    private Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        rb = player.GetComponent<Rigidbody>();

        gameView.SetGlassVisualDisplayText(glassConfig.SpeedThreshold);
        gameView.SetWoodVisualDisplayText(woodConfig.SpeedThreshold);
    }
    
    private void Update()
    {
        gameView.SetVelocityText(rb.linearVelocity.magnitude);
    }
}
