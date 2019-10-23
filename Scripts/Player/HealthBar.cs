using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] Transform Player;
    [SerializeField] Vector3 m_Offset;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        transform.position = Player.transform.position + m_Offset;
	}

    public void SetHealthPercentage(float Percentage,bool isknockeddown)
    {
        if (isknockeddown)
        {
            GetComponent<Renderer>().material.color = Color.blue;
            transform.GetChild(0).transform.localScale = new Vector3(0.0f, 1, 1);
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.black;
            transform.GetChild(0).transform.localScale = new Vector3(Percentage, 1, 1);
        }

        //transform.GetChild(0).transform.localPosition = new Vector3(-Percentage, 0.001f, 0.0f);
    }
}
