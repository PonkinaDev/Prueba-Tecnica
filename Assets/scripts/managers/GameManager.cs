using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start() { }
    void Update() { }
    public void LoadScene(string MyScene)
    {
        SceneManager.LoadScene(MyScene);
    }

    public void LoadSceneWithData(string MyScene)
    {
        RegistrationManager.Instance.LoadScene(MyScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
