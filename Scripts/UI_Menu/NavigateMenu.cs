using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigateMenu : MonoBehaviour {
    [SerializeField] private EventSystem m_eventSytem = null;
    [SerializeField] private GameObject m_SelectedObject = null;

    private bool m_IsAButtonSelected = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetAxis("P1_Menu_Vertical") != 0 && !m_IsAButtonSelected)
        {
            m_eventSytem.SetSelectedGameObject(m_SelectedObject);
            m_IsAButtonSelected = true;
        }    
    }

    private void OnDisable()
    {
        m_IsAButtonSelected = false;
    }
}
