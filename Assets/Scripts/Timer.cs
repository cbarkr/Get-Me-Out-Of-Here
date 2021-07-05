using System.Collections;
using System.Collections.Generic;
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
            endScreen.text = "You got out of there in " + MaxTime.ToString("0") + " seconds";
            return;
        }
        MaxTime += Time.deltaTime;
        TimerText.text = MaxTime.ToString("0");
    }    
}
