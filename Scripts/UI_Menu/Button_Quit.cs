using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Quit: MonoBehaviour
{

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
