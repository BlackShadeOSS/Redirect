using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EndPanelController : MonoBehaviour
{
    public Button Menubtn;
    public Button Exitbtn;
    
    void Start () {
        Button btn = Menubtn.GetComponent<Button>();
        btn.onClick.AddListener(EnterMenu);
        
        Button btn2 = Exitbtn.GetComponent<Button>();
        btn2.onClick.AddListener(ExitGame);
    }
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
