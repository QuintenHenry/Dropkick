using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherRoot = other.transform.root.gameObject;

        if (GetComponent<SphereCollider>().enabled && otherRoot.tag == "Player")
        {
            if (transform.root.tag == "Player")
            {
                CombatManager combatManager = transform.root.GetComponentInChildren<CombatManager>();
                PlayerManager playerManager = transform.root.GetComponentInChildren<PlayerManager>();

                PlayerManager otherPlayerManager = otherRoot.GetComponentInChildren<PlayerManager>();

                if (otherPlayerManager.GetTeam() != playerManager.GetTeam() && !otherPlayerManager.GetInvulnerableState())
                {
                    combatManager.Collision(gameObject, otherRoot);
                    GetComponent<SphereCollider>().enabled = false;
                }
            }
        }
    }
}
