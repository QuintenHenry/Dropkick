using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] Vector3 m_Offset;
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position + m_Offset;
    }

    public void SetStaminaPercentage(float Percentage)
    {
            GetComponent<Renderer>().material.color = Color.black;
            transform.GetChild(0).transform.localScale = new Vector3(Percentage, 1, 1);
    }
}
