using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour{
    public Transform Player;
    public Text TimerText;
    public Text endScreen;
    private float MaxTime = 0f;
    public bool isFinished = false;

    void Update(){
        if (isFinished){
            // Display time that player took to complete the game
            endScreen.text = "You got out of there in " + MaxTime.ToString("0") + " seconds";
            return;
        }
        // Add to current time
        MaxTime += Time.deltaTime;

        // Display time elapsed since player started the game
        TimerText.text = MaxTime.ToString("0");
    }    
}