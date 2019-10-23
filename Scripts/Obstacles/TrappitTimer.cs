using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappitTimer : MonoBehaviour
{

    [SerializeField] private GameObject m_TrapLeft;
    [SerializeField] private GameObject m_TrapRight;
    [SerializeField] private GameObject m_HitBox;
    [SerializeField] private float m_OpenSpeed = 1.0f;
    [SerializeField] private float m_ClosedTimer = 8.0f;
    [SerializeField] private float m_OpenTimer = 5.0f;
    //[SerializeField] private bool m_ClockWise = true;

    private List<GameObject> m_Doors = new List<GameObject>();
    private float m_Delay = 0.0f;

    private bool m_ShouldRotateOpen = true;
    private float m_Counter = 0.0f;
    private void Start()
    {
        m_Doors.Add(m_TrapLeft);
        m_Doors.Add(m_TrapRight);
        m_HitBox.SetActive(false);
    }

    private void Update()
    {
        m_Counter += Time.deltaTime;

        //Rotate to open
        if (m_ShouldRotateOpen)
        {
            bool clockWise = true;
            //Delay the hitbox going to true
            m_Delay += Time.deltaTime;

            foreach (GameObject door in m_Doors)
            {
                if (clockWise)
                    door.transform.rotation = Quaternion.Slerp(door.transform.rotation, Quaternion.Euler(90.0f, 0.0f, 0.0f), m_OpenSpeed * Time.deltaTime);
                else
                    door.transform.rotation = Quaternion.Slerp(door.transform.rotation, Quaternion.Euler(-90.0f, 0.0f, 0.0f), m_OpenSpeed * Time.deltaTime);

                if (m_Delay >= 1.5f)
                {
                    m_HitBox.SetActive(false);
                    m_Delay = 0.0f;
                }

                if (m_Counter >= m_OpenTimer)
                {
                    m_ShouldRotateOpen = false;
                    m_Counter = 0.0f;
                }

                clockWise = false;
            }
        }
        //Rotate to close
        else
        {
            //Delay the hitbox going to true
            m_Delay += Time.deltaTime;

            foreach (GameObject door in m_Doors)
            {
                door.transform.rotation = Quaternion.Slerp(door.transform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), m_OpenSpeed * Time.deltaTime);

                if (m_Delay >= 2.0f)
                {
                    m_HitBox.SetActive(true);
                    m_Delay = 0.0f;
                }

                if (m_Counter >= m_ClosedTimer)
                {
                    m_ShouldRotateOpen = true;
                    m_Counter = 0.0f;
                    //m_HitBox.SetActive(false);
                }
            }
        }
        //StartCoroutine(StopTrap());
    }

    void ChangeOpen() {
        m_ShouldRotateOpen = !m_ShouldRotateOpen;
    }
}
