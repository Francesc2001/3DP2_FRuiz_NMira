using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FPSControllerPlayer : MonoBehaviour
{

    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationSpeed;
    public float m_PitchRotationSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;

    public Transform m_PitchController;

    public bool m_UseYawInverted;
    public bool m_UsePitchInverted;

    [Header("Input")]
    public CharacterController m_CharacterController;
    public float m_PlayerSpeed;
    public float m_FastSpeedMultiplier;
    public KeyCode m_LeftKeyCode;
    public KeyCode m_RightKeyCode;
    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    public KeyCode m_AttachObjectKeyCode = KeyCode.E;


    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    public Camera m_Camera;
    public float m_NormalMovementFOV = 60.0f;
    public float m_RunMovementFOV = 75.0f;
    public float m_FOVSpeed = 4f;
    public float m_FOVSpeedReleased = 10f;

    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;

    public float m_JumpSpeed = 10.0f;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public Portal m_DoomiePortal;

    float m_TimeOfGround;
    public float m_TimeGrounded = 0.2f;

    public float m_MaxShootDistance;

    public LayerMask m_ShootingLayerMask;

    public float m_OffsetTeleportPortal;

    bool m_Shooting = false;

    private int m_Size;
    public List<float> m_SizeNumbers = new List<float>();

    public float m_Health;
    public float m_bullets = 10;
    public float m_MaxBullets = 100;
    public float m_ChargerBullets = 10;
    public float m_Points;
    public float m_Shield;

    bool isReloading;
    bool isRunning;

    public bool pointsActive = false;



    Vector3 m_Direction;
    [Range(0.0f, 90.0f)] public float m_AngleToEnterPortalInDegrees;

    [Header("AttachObject")]
    public Transform m_AttachingPosition;
    Rigidbody m_ObjectAttached;
    bool m_AttachingObject = false;
    public float m_AttachingObjectSpeed = 3.0f;
    Quaternion m_AttachingObjectStartRotation;
    public float m_MaxDistanceToAttachObject;
    public LayerMask m_AttachObjectLayerMask;
    public float m_AttachedObjectThrowForce = 750.0f;


    void Start()
    {


        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;

        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        //#if UNITY_EDITOR //para poder bloquear o desbloquear cosas cuando le das a play
        //#else
        //#endif

        //SetIdleWeaponAnimation();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        m_SizeNumbers.Add(50);
        m_SizeNumbers.Add(1);
        m_SizeNumbers.Add(200);
        m_SizeNumbers[0] = 0.5f;
        m_SizeNumbers[1] = 1;
        m_SizeNumbers[2] = 2;

        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);

    }


    void Update()
    {
        UpdateInputDebug();
        //ShootingGalery();

        if (Input.GetKeyDown(m_AttachObjectKeyCode) && CanAttachObject())
        {
            AttachObject();
        }
        if (m_ObjectAttached && !m_AttachingObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ThrowAttachedObject(m_AttachedObjectThrowForce);
            }
            if (Input.GetMouseButtonDown(1))
            {
                ThrowAttachedObject(0.0f);
            }
        }
        else if (!m_AttachingObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot(m_BluePortal);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Shoot(m_OrangePortal);
            }
        }

        float l_FOV = m_NormalMovementFOV;

        m_TimeOfGround += Time.deltaTime;
        Vector3 l_RightDirection = transform.right;
        Vector3 l_ForwardDirection = transform.forward;
        m_Direction = Vector3.zero;

        float l_Speed = m_PlayerSpeed;

        if (Input.GetKey(m_UpKeyCode))
            m_Direction = l_ForwardDirection;
        else
        {
            l_FOV = Mathf.Lerp(m_Camera.fieldOfView, m_NormalMovementFOV, m_FOVSpeedReleased * Time.deltaTime);
        }
        if (Input.GetKey(m_DownKeyCode))
            m_Direction -= l_ForwardDirection;
        if (Input.GetKey(m_RightKeyCode))
            m_Direction += l_RightDirection;
        if (Input.GetKey(m_LeftKeyCode))
            m_Direction -= l_RightDirection;
        if (Input.GetKeyDown(m_JumpKeyCode) && m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }


        if (Input.GetKey(m_RunKeyCode))
        {
            l_Speed = m_PlayerSpeed * m_FastSpeedMultiplier;
            if (m_Direction != Vector3.zero && !isReloading)
            {
                isRunning = true;
                //SetRunWeaponAnimation();
                l_FOV = Mathf.Lerp(m_Camera.fieldOfView, m_RunMovementFOV, m_FOVSpeed * Time.deltaTime);

            }

            l_FOV = m_RunMovementFOV;
        }
        else
        {
            isRunning = false;

            l_FOV = Mathf.Lerp(m_Camera.fieldOfView, m_NormalMovementFOV, m_FOVSpeedReleased * Time.deltaTime);
        }

        m_Camera.fieldOfView = l_FOV;

        m_Direction.Normalize();

        Vector3 l_Movement = m_Direction * l_Speed * Time.deltaTime;

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");
#if UNITY_EDITOR
        if (m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
#endif

        m_Yaw = m_Yaw + l_MouseX * m_YawRotationSpeed * Time.deltaTime * (m_UseYawInverted ? -1.0f : 1.0f); //(m_UseYawInverted ? -1.0f : 1.0f) Si el bool es correcte fara lo primer, sinó lo segon.
        m_Pitch = m_Pitch + l_MouseY * m_PitchRotationSpeed * Time.deltaTime * (m_UsePitchInverted ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch); //Mathf.Clamp et clava el valor al minim o maxim que haguis donat o al de la meitat.

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
            m_TimeOfGround = 0;
        }
        else
        {
            m_OnGround = false;
        }
        if (m_TimeOfGround < m_TimeGrounded)
        {
            m_OnGround = true;
        }

        if (Input.GetMouseButtonDown(0) && CanShoot())
        {
            //Shoot();
        }

        if (m_AttachingObject)
        {
            UpdateAttachObject();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (!(m_Size >= 2))
            {
                m_Size++;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (!(m_Size <= 0))
            {
                m_Size--;
            }
        }
    }

        void UpdateInputDebug()
        {
            if (Input.GetKeyDown(m_DebugLockAngleCode))
            {
                m_AngleLocked = !m_AngleLocked;
            }
            if (Input.GetKeyDown(m_DebugLockKeyCode))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
                }
            }
        }

        void ThrowAttachedObject(float force)
        {
            if (m_ObjectAttached != null)
            {
                m_ObjectAttached.transform.SetParent(null);
                m_ObjectAttached.isKinematic = false;
                m_ObjectAttached.AddForce(m_PitchController.forward * force);
                if (m_ObjectAttached.CompareTag("Cube"))
                {
                    m_ObjectAttached.GetComponent<Companion>().SetAttached(false);
                }
                else if (m_ObjectAttached.CompareTag("Turret"))
                {
                    m_ObjectAttached.GetComponent<Turret>().SetAttached(false);
                }
                m_ObjectAttached = null;
            }
        }

        
        void UpdateAttachObject()
        {
            Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
            Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;
            if (l_Movement >= l_Distance)
            {
                m_AttachingObject = false;
                m_ObjectAttached.transform.SetParent(m_AttachingPosition);
                m_ObjectAttached.transform.localPosition = Vector3.zero;
                m_ObjectAttached.transform.localRotation = Quaternion.identity;
                m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            }
            else
            {
                l_Direction /= l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            }
        }

    bool CanAttachObject()
    {
        return m_ObjectAttached == null;
    }
    void AttachObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistanceToAttachObject, m_AttachObjectLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "Cube")
            {
                m_AttachingObject = true;
                m_ObjectAttached = l_RaycastHit.collider.GetComponent<Rigidbody>();
                m_ObjectAttached.GetComponent<Companion>().SetAttached(true);
                m_ObjectAttached.isKinematic = true;
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
            if (l_RaycastHit.collider.tag == "Turret")
            {
                m_AttachingObject = true;
                m_ObjectAttached = l_RaycastHit.collider.GetComponent<Rigidbody>();
                m_ObjectAttached.GetComponent<Turret>().SetAttached(true);
                m_ObjectAttached.isKinematic = true;
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
        }
    }

    bool CanShoot()
    {
        return true;
    }

        void Shoot(Portal _Portal)
        {
            Vector3 l_Position;
            Vector3 l_Normal;
            _Portal.transform.localScale = Vector3.one * ReturnSize();

            if (_Portal.IsValidPosition(m_Camera.transform.position, m_Camera.transform.forward, m_MaxShootDistance, m_ShootingLayerMask, out l_Position, out l_Normal))
            {
                _Portal.gameObject.SetActive(true);
            }
            else
            {
                _Portal.gameObject.SetActive(false);
            }
        }


    private void OnTriggerEnter(Collider other) //si colisiona
    {
        if (other.tag == "WallKill")
        {
            Die();
        }
        else if (other.tag == "Portal")
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (Vector3.Dot(l_Portal.transform.forward, -m_Direction) >= Mathf.Cos(m_AngleToEnterPortalInDegrees * Mathf.Deg2Rad))
            {
                Teleport(other.GetComponent<Portal>());
            }
        }
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosotion = _Portal.m_PortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_LocalDirection = _Portal.m_PortalTransform.transform.InverseTransformDirection(transform.forward);
        Vector3 l_LocalDirectionMovement = _Portal.m_PortalTransform.transform.InverseTransformDirection(m_Direction);
        Vector3 l_WorldDirectionMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirectionMovement);


        m_CharacterController.enabled = false;

        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);
        m_Yaw = transform.rotation.eulerAngles.y;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosotion) + l_WorldDirectionMovement * m_OffsetTeleportPortal;
        m_CharacterController.enabled = true;
    }

    public float ReturnSize()
    {
        return m_SizeNumbers[m_Size];
    }

    public void Die()
    {
        m_Health = 0.0f;

        SceneManager.LoadScene("GameOver");
        //GameController.GetGameController().RestartGame();
    }
}