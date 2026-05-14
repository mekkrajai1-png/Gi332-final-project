using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionButtons : MonoBehaviour
{
     

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(
            "MapLayout",
            LoadSceneMode.Single
        );
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void ReturnToMainMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
                NetworkManager.Singleton.Shutdown();
            else if (NetworkManager.Singleton.IsClient)
                NetworkManager.Singleton.Shutdown();
        }
        
        SceneManager.LoadScene("Main Menu");
    }
}