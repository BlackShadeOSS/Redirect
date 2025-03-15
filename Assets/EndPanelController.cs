using UnityEngine;
using UnityEngine.SceneManagement;
public class EndPanelController : MonoBehaviour
{
    public void EnterMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif 
    }
}
