using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public Player player;
    public Text scoreText;
    public Text endLevelText;

    public float gravity = 9.81f;

    private int score;
    private float restartTimer = 3f;
    private float finishedTImer = 5f;

    private bool finished;

	// Use this for initialization
	void Start () {
        player.onCollectCoin = onCollectCoin;
        endLevelText.enabled = false;
        
	}
	
	// Update is called once per frame
	void Update () {
        Physics.gravity = new Vector3(0, -gravity, 0);
		if (player.Dead) {
            restartTimer -= Time.deltaTime;
            if (restartTimer <= 0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (player.Finished) {
            if (finished == false){
                finished = true;
                OnFinish();

            }

            finishedTImer -= Time.deltaTime;
            if (finishedTImer < 0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
	}

    void onCollectCoin () {
        score++;
        scoreText.text = "Score: " + score;
        Debug.Log("gotCoin!");
        
    }

    void OnFinish() {
        endLevelText.enabled = true;
        endLevelText.text = "Ayo Gawd, you just beat " + SceneManager.GetActiveScene().name + "!";
        endLevelText.text += "\nYour Score: " + score;
    }
}
