using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    public enum PowerUp {
        None,Strength,Speed, KnockBack, Defence, Invulnerability
    }
    private PowerUp m_CurrentPowerUp = PowerUp.None;

    private float m_CurrentTimer;

    private CombatManager m_CombatManager;
    private PlayerManager m_PlayerManager;

    [Header("Strength")]
    [SerializeField] private float m_StrengthMultiplier;
    [SerializeField] private float m_StrengthTimer;
    private int m_DropKickDamageStandard;
    private int m_PunchDamageStandard;
    private int m_AerialDamageStandard;

    [Header("Speed")]
    [SerializeField] private float m_SpeedMultiplier;
    [SerializeField] private float m_SpeedTimer;
    private float m_SpeedStandard;

    [Header("KnockBack")]
    [SerializeField] private float m_KnockBackMultiplier;
    [SerializeField] private float m_KnockBackTimer;
    private float m_CombatKnockbackStandard;

    [Header("Defence")]
    [SerializeField] private int m_Defence;
    [SerializeField] private float m_DefenceTimer;
    private int m_DefenceStandard;

    [Header("Invulnerability")]
    [SerializeField] private float m_InvulnerableTimer;

    [Header("Particles")]
    //[SerializeField] private GameObject m_PowerUpAuraParticle;
    [SerializeField] private GameObject m_PowerUpContainerParticle;
    private GameObject m_CurentActiveChildParticle;

    public void Start()
    {
        m_CombatManager = GetComponent<CombatManager>();
        m_DropKickDamageStandard = m_CombatManager.GetDropKickDamage();
        m_PunchDamageStandard = m_CombatManager.GetPunchDamage();
        m_AerialDamageStandard = m_CombatManager.GetAerialKickDamage();
        m_CombatKnockbackStandard = m_CombatManager.GetKnockbackMultiplier();

        m_PlayerManager = GetComponent<PlayerManager>();
        m_SpeedStandard = m_PlayerManager.GetSpeed();
        m_DefenceStandard = m_PlayerManager.GetDefence();
    }

    #region getters/setters

    public void SetParticleChildContainer(GameObject particleParent)
    {
        m_PowerUpContainerParticle = particleParent;
    }

    public PowerUp GetCurrentPowerUp()
    {
        return m_CurrentPowerUp;
    }

    public void SetStrengthMultiplier(float multiplier)
    {
        m_StrengthMultiplier = multiplier;
    }

    public void SetStrengthTimer(float timer)
    {
        m_StrengthTimer = timer;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        m_SpeedMultiplier = multiplier;
    }

    public void SetSpeedTimer(float timer)
    {
        m_SpeedTimer = timer;
    }

    public void SetKnockBackMultiplier(float multiplier)
    {
        m_KnockBackMultiplier = multiplier;
    }

    public void SetKnockBackTimer(float time)
    {
        m_KnockBackTimer = time;
    }

    public void SetDefence(int defence)
    {
        m_Defence = defence;
    }

    public void SetDefenceTimer(float time)
    {
        m_DefenceTimer = time;
    }

    public void SetInvulnerabilityTimer(float time)
    {
        m_InvulnerableTimer = time;
    }
    #endregion

    public bool AddPowerUp(PowerUp power)
    {
        if (m_CurrentPowerUp == PowerUp.None)
            m_CurrentPowerUp = power;
        else
            return false;
        //set timer
        m_PlayerManager.SetTimesPickedPowerUp(m_PlayerManager.GetTimesPickedPowerUp() + 1);

        switch (m_CurrentPowerUp)
        {
            case PowerUp.Strength:
                m_CurrentTimer = m_StrengthTimer;
                StrengthPower();
                Debug.Log("PowerUpManager::AddPowerUp Added PowerUp: Strength");
                break;
            case PowerUp.Speed:
                m_CurrentTimer = m_SpeedTimer;
                m_PlayerManager.SetSpeed(m_SpeedStandard * m_SpeedMultiplier);
                Debug.Log("PowerUpManager::AddPowerUp Added PowerUp: Speed");
                break;
            case PowerUp.KnockBack:
                m_CurrentTimer = m_KnockBackTimer;
                m_CombatManager.SetKnockbackMultiplier(m_KnockBackMultiplier);
                Debug.Log("PowerUpManager::AddPowerUp Added PowerUp: Knockback");
                break;
            case PowerUp.Defence:
                m_CurrentTimer = m_DefenceTimer;
                m_PlayerManager.SetDefence(m_Defence);
                Debug.Log("PowerUpManager::AddPowerUp Added PowerUp: Defence");
                break;
            case PowerUp.Invulnerability:
                m_CurrentTimer = m_InvulnerableTimer;
                m_PlayerManager.SetInvulnerableState(true);
                Debug.Log("PowerUpManager::AddPowerUp Added PowerUp: Invulnerability");
                break;
        }

        m_CurentActiveChildParticle = m_PowerUpContainerParticle.transform.GetChild((int)m_CurrentPowerUp -1).gameObject;
        m_CurentActiveChildParticle.SetActive(true);
        StartCoroutine(DisableParticle(m_CurrentTimer, m_CurentActiveChildParticle));

        return true;
    }

    IEnumerator DisableParticle(float duration, GameObject particle)
    {
        yield return new WaitForSeconds(duration);
        particle.SetActive(false);
        StopCoroutine(DisableParticle(duration, particle));
    }

    private void Update()
    {
        m_CurrentTimer -= Time.deltaTime;
        if (m_CurrentTimer <= 0.0f && m_CurrentPowerUp!=PowerUp.None) //if time is over remove power up
            RemovePowerUp();
    }

    private void RemovePowerUp() {
        //what to do when power up gets removed
        switch (m_CurrentPowerUp)
        {
            case PowerUp.Strength:
                m_CombatManager.SetDropKickDamage(m_DropKickDamageStandard);
                m_CombatManager.SetPunchDamage(m_PunchDamageStandard);
                m_CombatManager.SetAerialKickDamage(m_AerialDamageStandard);
                break;
            case PowerUp.Speed:
                m_PlayerManager.SetSpeed(m_SpeedStandard);
                break;
            case PowerUp.KnockBack:
                m_CombatManager.SetKnockbackMultiplier(m_CombatKnockbackStandard);
                break;
            case PowerUp.Defence:
                m_PlayerManager.SetDefence(m_DefenceStandard);
                break;
            case PowerUp.Invulnerability:
                m_PlayerManager.SetInvulnerableState(false);
                break;
        }
        m_CurrentPowerUp = PowerUp.None;
    }
    private void StrengthPower() {

        //set punchdamage and dropkick damage
        int punchdamage = (int)m_PunchDamageStandard * (int)m_StrengthMultiplier;
        int dropkickdamage = (int)m_DropKickDamageStandard * (int)m_StrengthMultiplier;
        int aerialDamage = (int)m_AerialDamageStandard * (int)m_StrengthMultiplier;

        m_CombatManager.SetDropKickDamage(dropkickdamage);
        m_CombatManager.SetPunchDamage(punchdamage);
        m_CombatManager.SetAerialKickDamage(aerialDamage);
    }
}
