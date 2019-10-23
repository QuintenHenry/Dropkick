using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GoalManager: MonoBehaviour
{
    [SerializeField] int m_TeamToDetect =0;
    [SerializeField] private float m_KnockBackForce = 0;
    [SerializeField] private float m_ExplosionRadius = 0.0f;
    [SerializeField] private int m_ExplosionDamage = 0;
    [SerializeField] private bool m_ScoreWhenAlive = false;
    [SerializeField] private GameObject m_ParticleObject;


    SoundManager m_SoundManager;
    #region PhysicsVariables
    //private List<GameObject> m_ExplosionForceTargets = new List<GameObject>();
    bool m_ShouldPhysicsDoExplosion = false;
    #endregion 
    // Use this for initialization
    void Start()
    {
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (m_ShouldPhysicsDoExplosion)
        {
            List<GameObject> explosionForceTargets = new List<GameObject>();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
            foreach (Collider currCollider in hitColliders)
            {
                if (currCollider.transform.root.CompareTag("Player"))
                {
                    explosionForceTargets.Add(currCollider.transform.root.Find("mixamorig:Hips").gameObject);

                    PlayerManager currColliderPlayerManager = currCollider.transform.root.GetComponentInChildren<PlayerManager>();
                    if (currColliderPlayerManager.GetTeam() == m_TeamToDetect)
                    {
                        currColliderPlayerManager.SetCurrentHealth(currColliderPlayerManager.GetCurrentHealth() - m_ExplosionDamage);
                    }
                }
            }

            explosionForceTargets = explosionForceTargets.Distinct().ToList();

            if (explosionForceTargets.Count > 0)
            {
                foreach (GameObject target in explosionForceTargets)
                {
                    target.GetComponent<Rigidbody>().AddExplosionForce(m_KnockBackForce, transform.position, m_ExplosionRadius, 0.5f);
                }
                explosionForceTargets.Clear();
            }
            m_ShouldPhysicsDoExplosion = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherRoot = other.transform.root.gameObject;

        if (otherRoot.tag == "Player")
        {
            PlayerManager otherPlayerManager = otherRoot.GetComponentInChildren<PlayerManager>();

            if (!otherPlayerManager.GetKnockDownState() && !m_ScoreWhenAlive)
                return;
            if (otherPlayerManager.GetTeam() == m_TeamToDetect && otherPlayerManager.GetCanBeScored())
            {
                otherPlayerManager.SetTimesScored(otherPlayerManager.GetTimesScored() + 1);

                var lastTouch = otherPlayerManager.GetPlayerThatLastTouchedMe(); //.IncrementTimesGoalScored();
                if (lastTouch)
                {
                    lastTouch.IncrementTimesGoalScored();
                }

                otherPlayerManager.Respawn(true, true, false);


                //change sound
                m_SoundManager.PlaySound("Horn",-1);

                Camera.main.GetComponent<DynamicCamera>().Shake(0.05f, 1.0f);

                m_ShouldPhysicsDoExplosion = true;
                m_ParticleObject.GetComponent<ParticleSystem>().Play();
                //StartCoroutine(DisableParticle());
            }
        }
    }

    IEnumerator DisableParticle()
    {
        yield return new WaitForSeconds(m_ParticleObject.GetComponentInChildren<ParticleSystem>().main.duration);
        m_ParticleObject.SetActive(false);
        StopCoroutine(DisableParticle());
    }
}