using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReviveManager : MonoBehaviour
{
    [SerializeField] private int m_BaseHealthGain = 10;
    [SerializeField] private float m_ExplosionRadius = 1.0f;
    [SerializeField] private int m_ExplosionDamage = 0;

    private GameObject m_ReviveSpamButtonUI;
    private PlayerManager m_Player = null;
    private GameObject m_ReviveParticle = null;



    //Sound
    SoundManager m_SoundManager;

    #region PhysicsVariables
    //private List<GameObject> m_ExplosionForceTargets = new List<GameObject>();
    bool m_ShouldExplosionHappend = false;
    #endregion
    // Use this for initialization
    void Start ()
    {
        m_Player = GetComponent<PlayerManager>();
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        m_ReviveSpamButtonUI = transform.parent.Find("ReviveButtonUI").gameObject;
    }
	
	// Update is called once per framem_ExplosionRadius
	void Update ()
    {
        if (!m_Player.GetKnockDownState())
        {
            m_ReviveSpamButtonUI.SetActive(false);

            if (m_Player.GetCurrentHealth() <= 0)
            {
                m_Player.SetKnockDownState(true);
                m_ReviveSpamButtonUI.SetActive(true);
                m_Player.SetTimesKnockedDown(m_Player.GetTimesKnockedDown() + 1);
                m_Player.SetCurrentHealth(0.0f);
            }
        }
        else
        {
            string playerIdentification = "P" + (m_Player.GetId() + 1);

            if (Input.GetButtonUp(playerIdentification + "_Revive"))
            {
                m_ReviveSpamButtonUI.transform.localScale = new Vector3(0.015f, 1f, 0.015f);
                StartCoroutine(ResizeUISpam());
                RegainHealth();

                if (m_Player.GetCurrentHealth() >= m_Player.GetMaxHealth())
                {
                    m_ReviveSpamButtonUI.SetActive(false);
                    //change sound
                    m_SoundManager.PlaySound("Revive",m_Player.GetId());
                    m_Player.SetKnockDownState(false);
                    m_Player.SetTotalKnockDowns(m_Player.GetTotalKnockDowns() + 1);
                    m_Player.SetTimesRevived(m_Player.GetTimesRevived() + 1);
                    m_Player.SetCurrentHealth(m_Player.GetMaxHealth());
                    m_ShouldExplosionHappend = true;
                    m_ReviveParticle.SetActive(true);
                    StartCoroutine(DisableParticle());
                    SetLayerRecursively(m_Player.transform.gameObject, LayerMask.NameToLayer("Player"));
                }
            }
        }
	}

    IEnumerator ResizeUISpam()
    {
        yield return new WaitForSeconds(0.05f);
        m_ReviveSpamButtonUI.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
    }

    void SetLayerRecursively(GameObject obj, int newlayer)
    {
        obj.layer = newlayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newlayer);
        }
    }

    private void FixedUpdate()
    {
        if (m_ShouldExplosionHappend)
        {
            m_ShouldExplosionHappend = false;
            List<GameObject> explosionForceTargets = new List<GameObject>();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
            foreach (Collider currCollider in hitColliders)
            {
                if (currCollider.transform.root.CompareTag("Player"))
                {
                    PlayerManager otherPlayerManager = currCollider.transform.root.GetComponentInChildren<PlayerManager>();

                    if (otherPlayerManager.GetId() != m_Player.GetId())
                    {
                        explosionForceTargets.Add(currCollider.transform.root.Find("mixamorig:Hips").gameObject);

                        PlayerManager selfPlayerManager = transform.root.GetComponentInChildren<PlayerManager>();
                        if (selfPlayerManager.GetTeam() != otherPlayerManager.GetTeam())
                        {
                            otherPlayerManager.SetCurrentHealth(otherPlayerManager.GetCurrentHealth() - m_ExplosionDamage);
                        }
                    }
                }
            }

            explosionForceTargets = explosionForceTargets.Distinct().ToList();

            if (explosionForceTargets.Count > 0)
            {
                foreach (GameObject target in explosionForceTargets)
                {
                    target.GetComponent<Rigidbody>().AddExplosionForce(m_Player.GetPushBackForce(), transform.position, m_ExplosionRadius);
                }
                explosionForceTargets.Clear();
            }
        }
    }

    #region getters/setters
    public void SetReviveParticle(GameObject reviveParticle)
    {
        m_ReviveParticle = reviveParticle;
    }

    public void SetBaseHealthGain(int value)
    {
        m_BaseHealthGain = value;
    }

    public void SetExplosionRadius(float radius)
    {
        m_ExplosionRadius = radius;
    }

    public void SetExplosionDamage(int damage)
    {
        m_ExplosionDamage = damage;
    }
    #endregion

    private void RegainHealth()
    {
        int healthGain = m_BaseHealthGain - m_Player.GetTotalKnockDowns();

        if (healthGain <= 0)
        {
            healthGain = 1;
        }

        m_Player.SetCurrentHealth(m_Player.GetCurrentHealth() + healthGain);
    }

    IEnumerator DisableParticle()
    {
        yield return new WaitForSeconds(m_ReviveParticle.GetComponentInChildren<ParticleSystem>().main.duration);
        m_ReviveParticle.SetActive(false);
        StopCoroutine(DisableParticle());
    }
}
