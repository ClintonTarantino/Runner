using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public GameObject helpPanel;

   public void OnPressPlay()
    {
        SceneManager.LoadScene("NumeroUno");
    }
    public void OnPressHelp() {
        helpPanel.SetActive(true);
    }
}
