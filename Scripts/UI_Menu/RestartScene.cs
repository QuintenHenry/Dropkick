using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour {

    //Re-load the current scene
    public void RestartGameScene()
    {
        //Re-load the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
