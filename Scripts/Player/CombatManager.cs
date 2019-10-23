using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class CombatManager : MonoBehaviour
{
    //Punch
    [SerializeField] private int m_PunchDamage = 0;
    [SerializeField] private float m_KnockBackForcePunch = 0;
    private float m_PunchForce = 100;
    private bool m_IsPunchOnCooldown = false;
    private float m_PunchCoolDown = 0.15f;
    private int m_PunchStaminaCost = 10;

    //DropKick
    [SerializeField] private int m_DropKickDamage = 0;
    [SerializeField] private float m_KnockBackForceDropkick = 0;
    private float m_KickForce = 5.2f;
    private bool m_IsDropKickOnCooldown = false;
    private float m_DropKickCooldown = 2.0f;
    private int m_DropKickStaminaCost = 25;

    //AerialDropKick
    private bool m_DidAerialKick = false;
    private int m_AerialKickDamage = 0;

    //Grab
    private float m_GrabRange = 0.5f;
    private List<GameObject> m_GrabTargets = new List<GameObject>();
    private bool CanGrab = true;
    private int m_GrabStaminaCost = 1;

    //Trow
    [SerializeField] private float m_ThrowForce = 10000.0f;
    private int m_TrowStaminaCost = 30;

    //Falcon
    //[SerializeField] private float m_KnockBackForceFalcon = 4000.0f;

    //Particle
    GameObject m_ContactHitParticle;
    GameObject m_DropKickHitParticle;

    //Sound
    SoundManager m_SoundManager;


    private PlayerManager m_Player = null;


    private bool DropKicking;
    private Vector3 LatestInputDirection= new Vector3(1.0f,0.0f,0.0f);



    #region PhysicsVariables
    //Punch
    bool m_DoFirstPunchAnimation = false;
    bool m_DoSecondPunchAnimation = false;

    ////Falcon
    //bool m_DoFirstFalconPunchAnimation = false;
    //bool m_DoSecondFalconPunchAnimation = false;
    
    //Grab
    bool m_ShouldGetGrabTargets = false;

    //Knockback
    bool m_ShouldDoKnockback = false;
    GameObject m_PushBackTarget = null;
    Vector3 m_PushBackVelocity = Vector3.zero;
    float m_KnockBackForce = 0.0f;
    float m_KnockBackMultiplier = 1.0f;
    #endregion

    #region Getters/Setters
    public void SetLatestInputDirection(Vector3 direction)
    {
        LatestInputDirection = direction;
    }
    public void SetParticleObject(GameObject particleObject)
    {
        m_ContactHitParticle = particleObject;
    }
    public void SetDropKickHitParticle(GameObject particle)
    {
        m_DropKickHitParticle = particle;
    }
    public void SetAerialKickDamage(int damage)
    {
        m_AerialKickDamage = damage;
    }
    public int GetAerialKickDamage() { return m_AerialKickDamage; }

    public void SetKnockbackMultiplier(float multiplier)
    {
        m_KnockBackMultiplier = multiplier;
    }
    public float GetKnockbackMultiplier()
    {
        return m_KnockBackMultiplier;
    }

    public void SetPunchDamage(int damage)
    {
        m_PunchDamage = damage;
    }
    public int GetPunchDamage()
    {
        return m_PunchDamage;
    }
    public void SetDropKickDamage(int damage)
    {
        m_DropKickDamage = damage;
    }
    public int GetDropKickDamage()
    {
        return m_DropKickDamage;
    }

    public void SetPunchKnockbackForce(float force)
    {
        m_KnockBackForcePunch = force;
    }

    public void SetDropKickKnockbackForce(float force)
    {
        m_KnockBackForceDropkick = force;
    }

    public void SetThrowForce(float force)
    {
        m_ThrowForce = force;
    }
    #endregion
    // Use this for initialization
    void Start()
    {
        m_Player = GetComponent<PlayerManager>();
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<SphereCollider>().enabled = false;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<SphereCollider>().enabled = false;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftKnee).GetComponent<SphereCollider>().enabled = false;
        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

    }

    // Update is called once per frame
    void Update()
    {
        string playerIdentification = "P" + (m_Player.GetId() + 1);

        Punch(playerIdentification);
        DropKick(playerIdentification);
        Grab(playerIdentification);
        BesidesBodyThrow(playerIdentification);
    }

    private void FixedUpdate()
    {
        if (m_DoFirstPunchAnimation)
        {
            Vector3 direction = Quaternion.AngleAxis(60, Vector3.up) * transform.forward;
            direction = Quaternion.AngleAxis(30, Vector3.forward) * direction;

            m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<ConstantForce>().force = direction.normalized * m_PunchForce;
            m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<SphereCollider>().enabled = true;

            m_DoFirstPunchAnimation = false;
        }
        else if (m_DoSecondPunchAnimation)
        {
            m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<ConstantForce>().force = (transform.forward + new Vector3(0, 0.3f, 0)).normalized * m_PunchForce;

            m_DoSecondPunchAnimation = false;
        }

        if (DropKicking)
        {

            m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftKnee).GetComponent<Rigidbody>().velocity = LatestInputDirection * m_KickForce;
            m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<Rigidbody>().velocity = LatestInputDirection * m_KickForce;
            transform.rotation = Quaternion.LookRotation(LatestInputDirection+new Vector3(0,1,0));
        }
        //Falcon
        //if (m_DoFirstFalconPunchAnimation)
        //{
        //    Vector3 direction = transform.forward + new Vector3(0.0f, 0.3f, 0.0f);
        //    m_RightArm.GetComponent<ConstantForce>().force = direction.normalized * m_PunchForce;
        //    m_RightHand.GetComponent<SphereCollider>().enabled = true;
        //    GetComponent<Rigidbody>().AddForce(transform.forward * m_KnockBackForceFalcon);
        //    m_DoFirstFalconPunchAnimation = false;
        //}
        //else if (m_DoSecondFalconPunchAnimation)
        //{
        //    m_RightHand.GetComponent<ConstantForce>().force = (transform.forward + new Vector3(0, 0.3f, 0)).normalized * m_PunchForce;
        //    m_DoSecondFalconPunchAnimation = false;
        //}


        if (m_ShouldGetGrabTargets)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_GrabRange);

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject otherRoot = hitCollider.transform.root.gameObject;

                if (otherRoot.tag == "Player")
                {
                    PlayerManager otherPlayerManager = otherRoot.GetComponentInChildren<PlayerManager>();

                    if (m_Player.GetId() != otherPlayerManager.GetId() && m_Player.GetTeam() != otherPlayerManager.GetTeam() && otherPlayerManager.GetKnockDownState())
                    {
                        m_GrabTargets.Add(otherRoot);
                    }
                }
            }

            m_ShouldGetGrabTargets = false;
        }

        if (m_ShouldDoKnockback)
        {
            m_PushBackTarget.GetComponent<Rigidbody>().AddForce(m_PushBackVelocity * (m_KnockBackForce) * m_KnockBackMultiplier);// - m_KnockBackForce / m_Player.GetMaxHealth() * m_Player.GetCurrentHealth()));

            m_ShouldDoKnockback = false;
        }
    }

    #region Punch
    private void Punch(string playerIdentification)
    {
        if (Input.GetButtonDown(playerIdentification + "_Punch") && !m_Player.GetKnockDownState() && !m_IsPunchOnCooldown && !(Input.GetButton(playerIdentification + "_Grab") && m_GrabTargets.Count > 0))
        {
            if (m_Player.GetCurrentStamina() >= m_PunchStaminaCost)
            {
                m_SoundManager.PlaySound("Whoosh", m_Player.GetId());
                //m_DoFirstPunchAnimation = true;
                m_IsPunchOnCooldown = true;

                m_Player.SetCurrentStamina(m_Player.GetCurrentStamina() - m_PunchStaminaCost);
                m_Player.ShouldStaminaRegen(false);

                //StartCoroutine(FistAnimationSecondStep());
                //StartCoroutine(FistRelease());
                m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<SphereCollider>().enabled = true;

                m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<Animator>().enabled = true;
                m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<Animator>().speed = 1;
                m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<Animator>().Play("Punch",0,0.0f);

                StartCoroutine(ReloadPunch());
            }
        }
    }

    IEnumerator FistRelease()
    {
        yield return new WaitForSeconds(0.5f);
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<ConstantForce>().force = Vector3.zero;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<SphereCollider>().enabled = false;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<ConstantForce>().force = Vector3.zero;
    }
    IEnumerator ReloadPunch()
    {
        yield return new WaitForSeconds(m_PunchCoolDown);

        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightElbow).GetComponent<SphereCollider>().enabled = false;

        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<Animator>().speed = 0;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightArm).GetComponent<Animator>().enabled = false;
        m_IsPunchOnCooldown = false;
    }

    IEnumerator FistAnimationSecondStep()
    {
        yield return new WaitForSeconds(0.3f);
        m_DoSecondPunchAnimation = true;
    }
    #endregion

    #region FalconPunch
    //private void FalconPunch(string playerIdentification)
    //{
    //    if (Input.GetButtonDown(playerIdentification + "_Punch") && !m_Player.GetKnockDownState() && !m_IsPunchOnCooldown && !(Input.GetButton(playerIdentification + "_Grab") && m_GrabTargets.Count > 0))
    //    {
    //        if (m_Player.GetCurrentStamina() >= m_PunchStaminaCost)
    //        {
    //            m_DoFirstFalconPunchAnimation = true;
    //            m_IsPunchOnCooldown = true;

    //            m_Player.SetCurrentStamina(m_Player.GetCurrentStamina() - m_PunchStaminaCost);
    //            m_Player.ShouldStaminaRegen(false);

    //            StartCoroutine(FistAnimationSecondStep());
    //            StartCoroutine(FistRelease());
    //            StartCoroutine(ReloadPunch());
    //        }
    //    }
    //}

    //IEnumerator FistReleaseFalcon()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    m_RightHand.GetComponent<ConstantForce>().force = Vector3.zero;
    //    m_RightHand.GetComponent<SphereCollider>().enabled = false;
    //    m_RightArm.GetComponent<ConstantForce>().force = Vector3.zero;
    //}
    //IEnumerator ReloadPunchFalcon()
    //{
    //    yield return new WaitForSeconds(m_PunchCoolDown);
    //    m_IsPunchOnCooldown = false;
    //}

    //IEnumerator FistAnimationSecondStepFalcon()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    m_DoSecondFalconPunchAnimation = true;
    //}
    #endregion

    #region DropKick
    private void DropKick(string playerIdentification)
    {
        if (Input.GetButtonDown(playerIdentification + "_DropKick") && !m_Player.GetKnockDownState() && !m_IsDropKickOnCooldown)
        {
            if (m_Player.GetCurrentStamina() >= m_DropKickStaminaCost)
            {

                DropKicking = true;
                GetComponent<MovementManager>().SetStopMoving(true);

                m_SoundManager.PlaySound("Whoosh", m_Player.GetId());

                m_Player.SetCurrentStamina(m_Player.GetCurrentStamina() - m_DropKickStaminaCost);
                m_Player.ShouldStaminaRegen(false);

                StartCoroutine(LegRelease());
                StartCoroutine(ReloadDropKick());

                m_IsDropKickOnCooldown = true;
                m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<SphereCollider>().enabled = true;
            }

            if (!m_Player.GetIsOnGround())
            {
                m_DidAerialKick = true;
            }
        }
    }

    IEnumerator LegRelease()
    {
        yield return new WaitForSeconds(0.5f);
        DropKicking = false;
        GetComponent<MovementManager>().SetStopMoving(false);


        m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftKnee).GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<Rigidbody>().velocity = Vector3.zero;

        m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftKnee).GetComponent<ConstantForce>().force = Vector3.zero;
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<ConstantForce>().force = Vector3.zero;

        m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightKnee).GetComponent<SphereCollider>().enabled = false;
    }
    IEnumerator ReloadDropKick()
    {
        yield return new WaitForSeconds(m_DropKickCooldown);
        m_IsDropKickOnCooldown = false;
        //GetComponent<MovementManager>().SetBlockMovement(false);
    }
    #endregion

    #region Grab
    private void Grab(string playerIdentification)
    {
        if (Input.GetButtonDown(playerIdentification + "_Grab"))
        {
            m_ShouldGetGrabTargets = true;
        }

        if (m_GrabTargets.Count > 1)
        {
            //Find the closest Target
            GameObject closestTarget = m_GrabTargets[0];

            if (m_GrabTargets.Count > 1)
            {
                float prevDistance = Vector3.Distance(closestTarget.transform.Find("mixamorig:Hips").position, transform.position);

                foreach (GameObject currGrabTarget in m_GrabTargets)
                {
                    float currDistance = Vector3.Distance(currGrabTarget.transform.Find("mixamorig:Hips").position, transform.position);
                    if (currDistance < prevDistance)
                    {
                        prevDistance = currDistance;
                        closestTarget = currGrabTarget;
                    }
                }

                m_GrabTargets.Clear();
                m_GrabTargets.Add(closestTarget);
            }
        }

        if (Input.GetButton(playerIdentification + "_Grab") && m_GrabTargets.Count > 0 && CanGrab)
        {
            if (!m_GrabTargets[0].GetComponentInChildren<PlayerManager>().GetKnockDownState())
            {
                m_GrabTargets.Clear();
            }
            else
            {
               // if (m_Player.GetCurrentStamina() >= m_GrabStaminaCost)
               // {
                   // m_Player.SetCurrentStamina(m_Player.GetCurrentStamina() - m_GrabStaminaCost);
                    //m_Player.ShouldStaminaRegen(false);

                    SetLayerRecursively(m_GrabTargets[0].gameObject, LayerMask.NameToLayer("IgnorePlayerCollision"));
                    m_GrabTargets[0].transform.Find("mixamorig:Hips").position = m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftElbow).transform.position;
             //   }
            }
        }

        if (Input.GetButtonUp(playerIdentification + "_Grab") && m_GrabTargets.Count > 0)
        {

            SetLayerRecursively(m_GrabTargets[0].gameObject, LayerMask.NameToLayer("Player"));
            m_GrabTargets.Clear();
        }
    }
    #endregion

    #region BesidesBodyThrow
    private void BesidesBodyThrow(string playerIdentification)
    {
        if (Input.GetButton(playerIdentification + "_Grab") && m_GrabTargets.Count > 0 && Input.GetButtonDown(playerIdentification + "_Punch"))
        {
            if (m_Player.GetCurrentStamina() >= m_GrabStaminaCost)
            {
                m_Player.SetCurrentStamina(m_Player.GetCurrentStamina() - m_TrowStaminaCost);
                m_Player.ShouldStaminaRegen(false);

                m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftElbow).GetComponent<ConstantForce>().force = (transform.forward + new Vector3(0, 0.3f, 0)).normalized * m_PunchForce;

                m_SoundManager.PlaySound("Whoosh", m_Player.GetId());
                Rigidbody toThrow = m_GrabTargets[0].transform.Find("mixamorig:Hips").GetComponent<Rigidbody>();
                SetLayerRecursively(toThrow.gameObject, LayerMask.NameToLayer("IgnorePlayerCollision"));
                m_GrabTargets.Clear();
                toThrow.velocity = Vector3.zero;
                toThrow.AddForce((transform.forward + new Vector3(0.0f, 0.3f, 0.0f)) * m_ThrowForce);
                CanGrab = false;
                StartCoroutine(StopBesidesBodyThrow(toThrow.gameObject));
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newlayer)
    {
        obj.layer = newlayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newlayer);
        }
    }

    IEnumerator StopBesidesBodyThrow(GameObject obj) {
        //when to stop
        yield return new WaitForSeconds(0.5f);
        m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftElbow).GetComponent<ConstantForce>().force = Vector3.zero;
        CanGrab = true;
        SetLayerRecursively(obj, LayerMask.NameToLayer("Player"));
    }
    #endregion

    #region collision
        public void Collision(GameObject self, GameObject other)
        {
        //Get the playerManager of the player that I hit
        PlayerManager otherPlayerManager = other.GetComponentInChildren<PlayerManager>();

        //Set ourselves as the player that last touched the other player
        otherPlayerManager.SetPlayerThatLastTouchedMe(m_Player);

        //Search for the bone that we will apply force on
        m_PushBackTarget = other.transform.root.Find("mixamorig:Hips").gameObject;

        //Get our own booty
        GameObject selfHips = transform.root.Find("mixamorig:Hips").gameObject;

        //Determine the velocity of the pushback
        m_PushBackVelocity = m_PushBackTarget.transform.position - selfHips.transform.position;
        m_PushBackVelocity = m_PushBackVelocity.normalized;
        m_PushBackVelocity = new Vector3(m_PushBackVelocity.x, 0.2f, m_PushBackVelocity.z);

        //Check if the collider was attached to a hand && the other is NOT knockeddown
        if (self.tag == "Hand" && !otherPlayerManager.GetKnockDownState() && m_IsPunchOnCooldown)
        {
            m_SoundManager.PlaySound("Punch", m_Player.GetId());
            otherPlayerManager.SetCurrentHealth(otherPlayerManager.GetCurrentHealth() - m_PunchDamage - otherPlayerManager.GetDefence());
            m_KnockBackForce = m_KnockBackForcePunch;
            m_ShouldDoKnockback = true;
            //Activate particle
            m_ContactHitParticle.SetActive(true);
            //Vibrate both controllers
            StartCoroutine(VibrateGamePads(otherPlayerManager));

            StartCoroutine(DisableParticle("punch"));
        }
        //else check if the collider was a foot && the other is NOT knockedDown
        else if (self.tag == "Foot" && m_IsDropKickOnCooldown)
        {
            m_SoundManager.PlaySound("Punch", m_Player.GetId());
            if (!otherPlayerManager.GetKnockDownState())
            {
                Camera.main.GetComponent<DynamicCamera>().Shake(0.02f, 0.2f);
                if (m_DidAerialKick)
                {
                    otherPlayerManager.SetCurrentHealth(otherPlayerManager.GetCurrentHealth() - m_AerialKickDamage - otherPlayerManager.GetDefence());
                }
                else
                {
                    otherPlayerManager.SetCurrentHealth(otherPlayerManager.GetCurrentHealth() - m_DropKickDamage - otherPlayerManager.GetDefence());
                }
            }
            m_KnockBackForce = m_KnockBackForceDropkick;
            m_ShouldDoKnockback = true;
            //Activate particle
            m_DropKickHitParticle.transform.position = m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightFoot).transform.position;
            m_DropKickHitParticle.SetActive(true);

            //Vibrate both controllers
            StartCoroutine(VibrateGamePads(otherPlayerManager));

            StartCoroutine(DisableParticle("dropkick"));
        }
    }
    #endregion

    IEnumerator DisableParticle(string particleName)
    {
        if (particleName == "punch")
        {
            yield return new WaitForSeconds(m_ContactHitParticle.GetComponentInChildren<ParticleSystem>().main.duration);
            m_ContactHitParticle.SetActive(false);
            StopCoroutine(DisableParticle("punch"));
        }
        else if (particleName == "dropkick")
        {
            yield return new WaitForSeconds(m_DropKickHitParticle.GetComponentInChildren<ParticleSystem>().main.duration);
            m_DropKickHitParticle.SetActive(false);
            StopCoroutine(DisableParticle("dropkick"));
        }
    }

    IEnumerator VibrateGamePads(PlayerManager otherPlayer = null)
    {
        GamePad.SetVibration((PlayerIndex)m_Player.GetId(), 1, 1);
        if (otherPlayer)
        {
            GamePad.SetVibration((PlayerIndex)otherPlayer.GetId(), 1, 1);
        }

        yield return new WaitForSeconds(0.1f);

        GamePad.SetVibration((PlayerIndex)m_Player.GetId(), 0, 0);
        if (otherPlayer)
        {
            GamePad.SetVibration((PlayerIndex)otherPlayer.GetId(), 0, 0);
        }
    }
}