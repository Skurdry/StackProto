using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    GameObject menu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu.gameObject.SetActive(false);

        TheStack.OnGameFinished += Show;
    }

    public void Show()
    {
        menu.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        TheStack.instance.ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        TheStack.instance.GenerateNewPlateform();
    }

}
