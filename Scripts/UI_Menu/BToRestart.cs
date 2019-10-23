using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BToRestart : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ReloadGame();
    }

    //Press B to go back to main menu
    private void ReloadGame()
    {
        //Check if a player presses the b button
        if (Input.GetButtonDown("P1_Menu_Cancel"))
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }
}
