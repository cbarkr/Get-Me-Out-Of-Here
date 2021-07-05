using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{
    bool hasGameEnded = false;
    public Timer timer;
    
    public void Finish(){
        timer.isFinished = true;
    }

    public void EndGame(){
        if (hasGameEnded == false){
            hasGameEnded = true;
        }
    }
}
