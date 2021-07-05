using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour{
    public void Play(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
