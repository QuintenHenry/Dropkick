using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextFromSliderValue : MonoBehaviour {
    [SerializeField] private Text m_Text = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetText((int)GetComponent<Slider>().value);
	}

    public void SetText(int value)
    {
        m_Text.text = "Time: " + value.ToString() + "Minutes";
    }
}
