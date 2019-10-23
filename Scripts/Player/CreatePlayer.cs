using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enums
public enum PlayerSkeleton
{
    pelvis,
    leftHips,
    leftKnee,
    leftFoot,
    rightHips,
    rightKnee,
    rightFoot,
    leftArm,
    leftElbow,
    rightArm,
    rightElbow,
    middleSpine,
    head,
    leftShoulder,
    rightShoulder
}
#endregion

public class CreatePlayer : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private int m_MaxHealth = 100;
    [SerializeField] private int m_MaxStamina = 100;
    [SerializeField] private float m_TimeTillStaminaRegen = 2.0f;
    [SerializeField] private int m_StaminaRegenAmmount = 5;
    [SerializeField] private float m_Speed = 80.0f;
    [SerializeField] private float m_PushBackForce = 10000.0f;
    [SerializeField] private float m_JumpForce = 3500.0f;
    [SerializeField] private float m_DeathZone_Y = -10.0f;

    [Header("Movement")]
    [SerializeField] private bool m_UseKeyboard = false;

    [Header("Combat")]
    [SerializeField] private int m_PunchDamage = 10;
    [SerializeField] private int m_DropKickDamage = 30;
    [SerializeField] private float m_PunchKnockbackForce = 5000;
    [SerializeField] private float m_DropKickKnockbackForce = 5000;
    [SerializeField] private float m_ThrowForce = 10000;
    [SerializeField] private float m_SphereColliderRadius = 0.3f;
    [SerializeField] private int m_AerialKickDamage = 40;

    [Header("Revive")]
    [SerializeField] private int m_BaseHealthGain = 5;
    [SerializeField] private float m_ExplosionRadius = 1.0f;
    [SerializeField] private int m_ExplosionDamage = 0;

    [Header("Power Ups")]
    [Header("Strength")]
    [SerializeField] private float m_StrengthMultiplier;
    [SerializeField] private float m_StrengthTimer;

    [Header("Speed")]
    [SerializeField] private float m_SpeedMultiplier;
    [SerializeField] private float m_SpeedTimer;

    [Header("KnockBack")]
    [SerializeField] private float m_KnockBackMultiplier;
    [SerializeField] private float m_KnockBackTimer;

    [Header("Defence")]
    [SerializeField] private int m_Defence;
    [SerializeField] private float m_DefenceTimer;

    [Header("Invulnerability")]
    [SerializeField] private float m_InvulnerableTimer;

    [Header("Particles")]
    [SerializeField] private GameObject m_PowerUpAuraParticle;
    [SerializeField] private GameObject m_PowerUpChildContainerParticle;
    [SerializeField] private GameObject m_HitParticle;
    [SerializeField] private GameObject m_ReviveParticle;
    [SerializeField] private GameObject m_DropKickHitParticle;

    private GameObject[] m_Skeleton = new GameObject[13];

    // Use this for initialization
    void Start()
    {
        //CreatePlayerGameObject();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePlayerGameObject()
    {
        SetUpSkeleton();
        SetPlayerManager();
        SetMovementManager();
        SetCombatManager();
        SetReviveManager();
        SetPowerUpManager();
    }

    private void SetUpSkeleton()
    {
        m_Skeleton[(int)PlayerSkeleton.pelvis] = FindChildWithName("mixamorig:Hips", transform);
        m_Skeleton[(int)PlayerSkeleton.leftHips] = FindChildWithName("mixamorig:LeftUpLeg", m_Skeleton[(int)PlayerSkeleton.pelvis].transform);
        m_Skeleton[(int)PlayerSkeleton.leftKnee] = FindChildWithName("mixamorig:LeftLeg", m_Skeleton[(int)PlayerSkeleton.leftHips].transform);
        m_Skeleton[(int)PlayerSkeleton.leftFoot] = FindChildWithName("mixamorig:LeftFoot:", m_Skeleton[(int)PlayerSkeleton.leftKnee].transform);
        m_Skeleton[(int)PlayerSkeleton.rightHips] = FindChildWithName("mixamorig:RightUpLeg", m_Skeleton[(int)PlayerSkeleton.pelvis].transform);
        m_Skeleton[(int)PlayerSkeleton.rightKnee] = FindChildWithName("mixamorig:RightLeg", m_Skeleton[(int)PlayerSkeleton.rightHips].transform);
        m_Skeleton[(int)PlayerSkeleton.rightFoot] = FindChildWithName("mixamorig:RightFoot", m_Skeleton[(int)PlayerSkeleton.rightKnee].transform);
        m_Skeleton[(int)PlayerSkeleton.middleSpine] = FindChildWithName("mixamorig:Spine2", m_Skeleton[(int)PlayerSkeleton.pelvis].transform);
        m_Skeleton[(int)PlayerSkeleton.leftArm] = FindChildWithName("mixamorig:LeftArm", m_Skeleton[(int)PlayerSkeleton.middleSpine].transform);
        m_Skeleton[(int)PlayerSkeleton.leftElbow] = FindChildWithName("mixamorig:LeftForeArm", m_Skeleton[(int)PlayerSkeleton.leftArm].transform);
        m_Skeleton[(int)PlayerSkeleton.rightArm] = FindChildWithName("mixamorig:RightArm", m_Skeleton[(int)PlayerSkeleton.middleSpine].transform);
        m_Skeleton[(int)PlayerSkeleton.rightElbow] = FindChildWithName("mixamorig:RightForeArm", m_Skeleton[(int)PlayerSkeleton.rightArm].transform);
        m_Skeleton[(int)PlayerSkeleton.head] = FindChildWithName("mixamorig:Head", m_Skeleton[(int)PlayerSkeleton.middleSpine].transform);
        //m_Skeleton[(int)PlayerSkeleton.leftShoulder] = FindChildWithName("mixamorig:LeftArm", m_Skeleton[(int)PlayerSkeleton.middleSpine].transform);
        //m_Skeleton[(int)PlayerSkeleton.rightShoulder] = FindChildWithName("mixamorig:RightArm", m_Skeleton[(int)PlayerSkeleton.middleSpine].transform);
        SetLimbs();
    }

    private void SetLimbs()
    {
        tag = "Player";
        float handOffset = 0.15f;

        GameObject limb = m_Skeleton[(int)PlayerSkeleton.rightKnee];
        limb.tag = "Foot";
        limb.AddComponent<CollisionManager>();
        limb.AddComponent<ConstantForce>();
        SphereCollider sphereCollider = limb.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = m_SphereColliderRadius;

        limb = m_Skeleton[(int)PlayerSkeleton.leftKnee];
        limb.tag = "Foot";
        limb.AddComponent<CollisionManager>();
        limb.AddComponent<ConstantForce>();
        sphereCollider = limb.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = m_SphereColliderRadius;

        limb = m_Skeleton[(int)PlayerSkeleton.leftElbow];
        limb.tag = "Hand";
        limb.AddComponent<CollisionManager>();
        limb.AddComponent<ConstantForce>();
        sphereCollider = limb.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = m_SphereColliderRadius;
        sphereCollider.center = new Vector3(0, handOffset, 0);

        limb = m_Skeleton[(int)PlayerSkeleton.rightElbow];
        limb.tag = "Hand";
        limb.AddComponent<CollisionManager>();
        limb.AddComponent<ConstantForce>();
        sphereCollider = limb.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = m_SphereColliderRadius;
        sphereCollider.center = new Vector3(0, handOffset, 0);

        limb = m_Skeleton[(int)PlayerSkeleton.rightArm];
        limb.AddComponent<ConstantForce>();

        limb = m_Skeleton[(int)PlayerSkeleton.head];
        limb.AddComponent<ConstantForce>();
    }

    private void SetPlayerManager()
    {
        PlayerManager playerManager = m_Skeleton[(int)PlayerSkeleton.pelvis].AddComponent<PlayerManager>();

        playerManager.SetMaxHealth(m_MaxHealth);
        playerManager.SetSpeed(m_Speed);
        playerManager.SetPushBackForce(m_PushBackForce);
        playerManager.SetJumpForce(m_JumpForce);
        playerManager.SetMesh(transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>());
        playerManager.SetHealthBar(transform.GetChild(2).GetComponent<HealthBar>());
        playerManager.SetDeathHeight(m_DeathZone_Y);
        playerManager.SetStaminaBar(transform.GetChild(7).GetComponent<StaminaBar>());
        playerManager.SetMaxStamina(m_MaxStamina);
        playerManager.SetTimeTillStaminaRegen(m_TimeTillStaminaRegen);
        playerManager.SetStaminaRegenAmmount(m_StaminaRegenAmmount);
        playerManager.SetSkeleton(m_Skeleton);
    }

    private void SetMovementManager()
    {
        MovementManager movementManager = m_Skeleton[(int)PlayerSkeleton.pelvis].AddComponent<MovementManager>();

        movementManager.SetRayCastMask(LayerMask.GetMask("Ground"));
        movementManager.UseKeyboard(m_UseKeyboard);
    }

    private void SetCombatManager()
    {
        CombatManager combatManager = m_Skeleton[(int)PlayerSkeleton.pelvis].AddComponent<CombatManager>();

        combatManager.SetPunchDamage(m_PunchDamage);
        combatManager.SetDropKickDamage(m_DropKickDamage);
        combatManager.SetPunchKnockbackForce(m_PunchKnockbackForce);
        combatManager.SetDropKickKnockbackForce(m_DropKickKnockbackForce);
        combatManager.SetThrowForce(m_ThrowForce);
        combatManager.SetAerialKickDamage(m_AerialKickDamage);
        combatManager.SetParticleObject(m_HitParticle);
        combatManager.SetDropKickHitParticle(m_DropKickHitParticle);
    }

    private void SetReviveManager()
    {
        ReviveManager reviveManager = m_Skeleton[(int)PlayerSkeleton.pelvis].AddComponent<ReviveManager>();

        reviveManager.SetBaseHealthGain(m_BaseHealthGain);
        reviveManager.SetExplosionRadius(m_ExplosionRadius);
        reviveManager.SetExplosionDamage(m_ExplosionDamage);
        reviveManager.SetReviveParticle(m_ReviveParticle);
    }

    private void SetPowerUpManager()
    {
        PowerUpManager powerUpManager = m_Skeleton[(int)PlayerSkeleton.pelvis].AddComponent<PowerUpManager>();

        powerUpManager.SetStrengthMultiplier(m_StrengthMultiplier);
        powerUpManager.SetStrengthTimer(m_StrengthTimer);
        powerUpManager.SetSpeedMultiplier(m_SpeedMultiplier);
        powerUpManager.SetSpeedTimer(m_SpeedTimer);
        powerUpManager.SetKnockBackMultiplier(m_KnockBackMultiplier);
        powerUpManager.SetKnockBackTimer(m_KnockBackTimer);
        powerUpManager.SetDefence(m_Defence);
        powerUpManager.SetDefenceTimer(m_DefenceTimer);
        powerUpManager.SetInvulnerabilityTimer(m_InvulnerableTimer);
        //powerUpManager.SetParticleObject(m_PowerUpAuraParticle);
        powerUpManager.SetParticleChildContainer(m_PowerUpChildContainerParticle);
    }

    private GameObject FindChildWithName(string childName, Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.gameObject.name == childName)
            {
                return child.gameObject;
            }
        }

        return null;
    }
}
