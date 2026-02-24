using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnStartClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}
