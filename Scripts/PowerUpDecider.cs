using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDecider : MonoBehaviour {

   public PowerUpManager.PowerUp m_PowerUp;

    private float m_Duration = 20.0f;

    SoundManager m_SoundManager;

    private void Start()
    {
        m_PowerUp = (PowerUpManager.PowerUp)Random.Range(1, System.Enum.GetValues(typeof(PowerUpManager.PowerUp)).Length);
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        switch (m_PowerUp)
        {
            case PowerUpManager.PowerUp.Strength:
                transform.GetChild((int)PowerUpManager.PowerUp.Strength - 1).gameObject.SetActive(true);
                //transform.GetChild((int)PowerUpManager.PowerUp.Strength - 1).GetComponent<MeshRenderer>().material.color = Color.black;
                break;
            case PowerUpManager.PowerUp.Speed:
                transform.GetChild((int)PowerUpManager.PowerUp.Speed - 1).gameObject.SetActive(true);
                //transform.GetChild((int)PowerUpManager.PowerUp.Speed - 1).GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case PowerUpManager.PowerUp.KnockBack:
                transform.GetChild((int)PowerUpManager.PowerUp.KnockBack - 1).gameObject.SetActive(true);
                //transform.GetChild((int)PowerUpManager.PowerUp.KnockBack - 1).GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case PowerUpManager.PowerUp.Defence:
                transform.GetChild((int)PowerUpManager.PowerUp.Defence - 1).gameObject.SetActive(true);
                //transform.GetChild((int)PowerUpManager.PowerUp.Defence - 1).GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
            case PowerUpManager.PowerUp.Invulnerability:
                transform.GetChild((int)PowerUpManager.PowerUp.Invulnerability - 1).gameObject.SetActive(true);
                //transform.GetChild((int)PowerUpManager.PowerUp.Invulnerability - 1).GetComponent<MeshRenderer>().material.color = Color.green;
                break;
        }

        Destroy(gameObject, m_Duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.tag == "Player")
        {
            if (other.transform.root.GetChild(1).GetComponent<PowerUpManager>().AddPowerUp(m_PowerUp)) //returns true if power is added succesful
            {
             //change sound
                m_SoundManager.PlaySound("Punch");
                Destroy(gameObject);
            }
        }
    }

}
