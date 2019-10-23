using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {

    // Use this for initialization

    [SerializeField] private float m_SpawnWidthMax=5;
    [SerializeField] private float m_SpawnHeigthMax=5;
    [Space]
    [SerializeField] private float m_SecondsToSpawn;
    [SerializeField] private GameObject m_ObjectToSpawn;
    [SerializeField] GameManager m_GameManager = null;

    private void Start()
    {
        InvokeRepeating("Spawn", m_SecondsToSpawn, m_SecondsToSpawn);
    }

    void Spawn() {
        if (m_GameManager.IsGameRunning())
        {
            Vector3 pos = new Vector3(Random.Range(-m_SpawnWidthMax / 2, m_SpawnWidthMax / 2), 0.0f, Random.Range(-m_SpawnHeigthMax / 2, m_SpawnHeigthMax / 2));
            Instantiate(m_ObjectToSpawn, transform.position - pos, Quaternion.identity);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(m_SpawnWidthMax, 1.0f, m_SpawnHeigthMax));
    }
}
