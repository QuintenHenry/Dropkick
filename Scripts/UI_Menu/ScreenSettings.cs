using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScreenSettings : MonoBehaviour {

    [SerializeField] private Dropdown m_DropDownResolution = null;

    private Resolution[] m_Resolution = { };

	// Use this for initialization
	void Start () {
        m_Resolution = Screen.resolutions;
        m_DropDownResolution.ClearOptions();

        List<string> options = new List<string>();
        int currentResolution = 0;

        for (int i = 0; i < m_Resolution.Length; i++)
        {
            string optionName = m_Resolution[i].width + " x " + m_Resolution[i].height;
            options.Add(optionName);

            if (m_Resolution[i].width == Screen.currentResolution.width && m_Resolution[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }
        m_DropDownResolution.AddOptions(options);
        m_DropDownResolution.value = currentResolution;
        m_DropDownResolution.RefreshShownValue();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetScreenResolution(int index)
    {
        Screen.SetResolution(m_Resolution[index].width, m_Resolution[index].height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
