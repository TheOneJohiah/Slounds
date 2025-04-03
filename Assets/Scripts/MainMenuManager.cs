using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // This function will be called when ButtonPlayLocal is clicked.
    public void PlayLocal()
    {
        SceneManager.LoadScene("LocalLobbyScene");
    }

    // This function will be called when ButtonPlayOnline is clicked.
    public void PlayOnline()
    {
        Debug.Log("Open Online Panel");
    }

    // This function will be called when ButtonCustomizer is clicked.
    public void OpenCustomizer()
    {
        SceneManager.LoadScene("CustomizerScene");
    }

    // This function will be called when ButtonOptions is clicked.
    public void OpenOptions()
    {
        // You might show/hide a UI panel for options or load an Options scene.
        Debug.Log("Open Options Panel");
    }

    // This function will be called when ButtonQuit is clicked.
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}