using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameStartCountdown : NetworkBehaviour
{
    public float startTime = 20f;

    private NetworkVariable<float> timeLeft = new NetworkVariable<float>();

    public TextMeshProUGUI countdownText;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeLeft.Value = startTime;
        }
    }

    void Update()
    {
        
        if (IsServer && timeLeft.Value > 0)
        {
            timeLeft.Value -= Time.deltaTime;

            if (timeLeft.Value <= 0)
            {
                timeLeft.Value = 0;
                StartGame();
            }
        }

        
        if (countdownText != null)
        {
            countdownText.text = "Start in: " + Mathf.Ceil(timeLeft.Value).ToString();
        }
    }

    void StartGame()
    {
        Debug.Log("Game Start!");
        StartGameClientRpc();
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        foreach (var player in FindObjectsOfType<Controller2D>())
        {
            if (player.IsOwner)
                player.canMove = true;
        }
        foreach (var ghost in FindObjectsOfType<GhostAI>())
        {
            ghost.gameStarted = true;
        }
    }
}