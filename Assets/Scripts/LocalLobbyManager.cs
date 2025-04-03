using UnityEngine;
using UnityEngine.SceneManagement;
public class LocalLobbyManager : MonoBehaviour
{
    public void StartMatch()
    {
        SceneManager.LoadScene("LocalGameScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
