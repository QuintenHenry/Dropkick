using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private LayerMask m_RayCastMask = -1;
    [SerializeField] private bool m_UseKeyboard = false;
   
    private PlayerManager m_Player = null;

    //HeadForce
    private float m_MaxRayCastDistance = 0.2f;
    private float m_AntiGravityForce = 180.0f;

    //Move
    private Vector3 m_Velocity = new Vector3(0, 0, 0);
    private bool m_IsMoving = false;
    private Rigidbody m_RigidBody = new Rigidbody();
    private GameObject m_ActiveLeg;

    //Jump
    private bool m_shouldJump = false;

    //StopMoving
    private bool m_StopMoving;
    private CombatManager combatManager;

    //Particle
    private bool m_IsDustParticleActive = false;

    #region Setters/Getters
    public void SetRayCastMask(LayerMask maskId) { m_RayCastMask = maskId; }
    public void UseKeyboard(bool useKeyboard) { m_UseKeyboard = useKeyboard; }
    #endregion

    // Use this for initialization
    void Start()
    {
        m_Player = GetComponent<PlayerManager>();
        m_RigidBody = transform.GetComponent<Rigidbody>();
        combatManager = GetComponent<CombatManager>();

        m_ActiveLeg = m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftFoot);

        InvokeRepeating("LegDustTrial", 0.2f, 0.2f);
    }
    // Update is called once per frame
    void Update ()
    {
        //Define wich player's input we recieve
        string playerIdentification = "P" + (m_Player.GetId() + 1);

        Move(playerIdentification);
        Jump(playerIdentification);
    }

    public void SetStopMoving(bool stopmoving)
    {
        m_StopMoving = stopmoving;
    }
    //Physics Update 50hrtz
    private void FixedUpdate()
    {
        //Set the character upright or down on the floor
        ApplyHeadForce();

        //Move the character
        PhysicsMove(m_Player.GetSpeed());

        //Jump
        PhysicsJump();
    }

    private void ApplyHeadForce()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * m_MaxRayCastDistance);

        //Push the head up from the ground
        if (Physics.Raycast(transform.position, Vector3.down, m_MaxRayCastDistance, m_RayCastMask) && !m_Player.GetKnockDownState())
        {
            m_Player.GetSkeleton_Part((int)PlayerSkeleton.head).GetComponent<ConstantForce>().force = new Vector3(0.0f, m_AntiGravityForce, 0.0f);
            m_Player.SetIsOnGround(true);
        }
        //Make the head go down with gravity
        else
        {
            m_Player.GetSkeleton_Part((int)PlayerSkeleton.head).GetComponent<ConstantForce>().force = new Vector3(0, Physics.gravity.y * Time.fixedDeltaTime, 0);
            m_Player.SetIsOnGround(false);
        }
    }

    private void Move(string playerIdentification)
    {
        if (m_StopMoving)
            return;
        //Determine wether we use the keyboard or not
        if (m_UseKeyboard)
        {
            //Set the velocity tot he input
            m_Velocity = new Vector3(Input.GetAxis(playerIdentification + "_Horizontal_Keyboard"), 0, Input.GetAxis(playerIdentification + "_Vertical_Keyboard"));
        }
        else
        {
            //Set the velocity tot he input
            m_Velocity = new Vector3(Input.GetAxis(playerIdentification + "_Horizontal"), 0, Input.GetAxis(playerIdentification + "_Vertical"));
        }

        //Normalize the velocity
        m_Velocity = m_Velocity.normalized;

        //If the movement is big enough say that we are moving
        if (m_Velocity.magnitude > 0.3)
        {
            m_IsMoving = true;
        }
        //If the movemnt is not big enough say that we are not moving
        else
        {
            m_IsMoving = false;
        }

        if (m_IsMoving)
            combatManager.SetLatestInputDirection(m_Velocity);
    }

    IEnumerator LegDustTrial()
    {
        if(!m_IsDustParticleActive)
        {
            //Set the dust on the currentleg active
            m_IsDustParticleActive = true;

        }

        yield return new WaitForSeconds(0.2f);

        m_ActiveLeg.transform.GetChild(0).gameObject.SetActive(false);

        if (m_ActiveLeg == m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftFoot))
        {
            m_ActiveLeg = m_Player.GetSkeleton_Part((int)PlayerSkeleton.rightFoot);
        }
        else
        {
            m_ActiveLeg = m_Player.GetSkeleton_Part((int)PlayerSkeleton.leftFoot);
        }

        m_ActiveLeg.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DisableParticle());
    }

    IEnumerator DisableParticle()
    {
        yield return new WaitForSeconds(0.2f);
        m_ActiveLeg.transform.GetChild(0).gameObject.SetActive(false);
        StopCoroutine(DisableParticle());
        m_IsDustParticleActive = false;
    }

    private void PhysicsMove(float speed)
    {
        //Check if the actor is moving
        if (m_IsMoving)
        {
            if (m_Player.GetKnockDownState())
                return;

            //Add a force to move the actor
            m_RigidBody.AddForce(m_Velocity * speed, ForceMode.Force);

            //Check if the current velocity doesn't go over the maximum speed
            if (m_RigidBody.velocity.magnitude > speed)
            {
                m_RigidBody.velocity = m_RigidBody.velocity.normalized * speed;
            }

            transform.forward = m_Velocity.normalized;
        }
        //If the actor is not moving
        else
        {
            //Check if the magnitude of the velocity is bigger than x
            if (m_RigidBody.velocity.magnitude > 1.0f)
            {
                //Substract xx% of the current velocity
                m_RigidBody.velocity = m_RigidBody.velocity - m_RigidBody.velocity * 0.3f;
            }
            else
            {
                //Stop movement of the rigidbody, except on the y axis (because gravity)
                m_RigidBody.velocity = new Vector3(0, m_RigidBody.velocity.y, 0);
            }
        }
    }

    private void Jump(string playerIdentification)
    {
        if (Input.GetButtonDown(playerIdentification + "_Jump") && !m_Player.GetKnockDownState() && m_Player.GetIsOnGround())
        {
            m_Player.SetIsOnGround(false);
            m_shouldJump = true;
        }
    }

    private void PhysicsJump()
    {
        if (m_shouldJump)
        {
            m_RigidBody.AddForce(new Vector3(.0f, 1.0f, .0f) * m_Player.GetJumpForce());
            m_shouldJump = false;
        }
    }
}