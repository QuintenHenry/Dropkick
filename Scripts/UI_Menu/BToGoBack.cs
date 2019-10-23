using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BToGoBack : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private GameObject m_BackTarget;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ReturnToTarget();
    }

    //Press B to go back to main menu
    private void ReturnToTarget()
    {
        //Check if a player presses the b button
        if (Input.GetButtonDown("P1_Menu_Cancel"))
        {
            m_BackTarget.SetActive(true);
            m_Self.SetActive(false);
        }
    }
}
