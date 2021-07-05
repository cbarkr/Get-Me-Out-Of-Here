using UnityEngine;
using UnityEngine.UI;

public class EndTrigger : MonoBehaviour{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision){
        gameManager.Finish();
    }
}