using UnityEngine;
using Unity.Netcode;

public class Goal : NetworkBehaviour
{
    private bool hasWinner = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        NetworkObject netObj = collision.GetComponent<NetworkObject>();
        if (netObj == null) return;
        
        TryWinServerRpc(netObj.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void TryWinServerRpc(ulong clientId)
    {
        if (hasWinner) return;
        
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
            return;

        var playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObj == null) return;

        HeartHealth hp = playerObj.GetComponent<HeartHealth>();
        
        if (hp != null && hp.IsAlive())
        {
            hasWinner = true;

            AnnounceWinClientRpc(clientId);
        }
    }

    [ClientRpc]
    void AnnounceWinClientRpc(ulong winnerId)
    {
        ulong myId = NetworkManager.Singleton.LocalClientId;

        if (myId == winnerId)
        {
            Debug.Log("YOU WIN!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");
        }
        else
        {
            Debug.Log("YOU LOSE!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScene");
        }
    }
}