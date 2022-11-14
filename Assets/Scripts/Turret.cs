using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Turret : MonoBehaviour
{
    bool m_IsAttached = false;
    public LineRenderer m_Laser;
    public LayerMask m_LaserLayerMask;
    public float m_MaxLaserDistance = 250.0f;

    Portal m_ExitPortal = null;
    public float m_OffsetTeleportPortal = 1.5f;
    Rigidbody m_RigidBody;
    float l_RemainingDistance;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
        float l_LaserDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxLaserDistance, m_LaserLayerMask.value))
        {
            l_LaserDistance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.tag == "Player" && !m_IsAttached)
            {
                SceneManager.LoadScene("GameOver");
            }
            if (l_RaycastHit.collider.tag == "Portal")
            {
                l_RemainingDistance = m_MaxLaserDistance - l_RaycastHit.distance;
                Portal l_Portal = l_RaycastHit.collider.GetComponent<Portal>();
                l_Portal.m_lineRenderer.gameObject.SetActive(true);
                Vector3 l_Velocity = l_Portal.m_PortalTransform.transform.InverseTransformDirection(l_Ray.direction);
                l_Portal.m_lineRenderer.transform.forward = l_Portal.m_MirrorPortal.transform.TransformDirection(l_Velocity);
                Vector3 l_localPos = l_Portal.m_MirrorPortal.transform.InverseTransformPoint(l_RaycastHit.point);
                l_Portal.m_lineRenderer.transform.position = l_Portal.m_PortalTransform.transform.TransformPoint(l_localPos);
                l_Portal.m_MirrorPortal.m_Camera.nearClipPlane = l_RemainingDistance + l_Portal.m_OffsetNearPlane;
            }
        }
        m_Laser.SetPosition(1, new Vector3(0.0f, 0.0f, l_LaserDistance));
    }

    public void SetAttached(bool Attached)
    {
        m_IsAttached = Attached;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Portal" && !m_IsAttached)
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (l_Portal != m_ExitPortal)
            {
                Teleport(l_Portal);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Portal")
        {
            if (other.GetComponent<Portal>() == m_ExitPortal)
            {
                m_ExitPortal = null;
            }
        }
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosotion = _Portal.m_PortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_LocalDirection = _Portal.m_PortalTransform.transform.InverseTransformDirection(transform.forward);

        Vector3 l_LocalVelocity = _Portal.m_PortalTransform.transform.InverseTransformDirection(m_RigidBody.velocity);
        Vector3 l_WorldVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);

        m_RigidBody.isKinematic = true;

        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);
        Vector3 l_WorldVelocityNormalized = l_WorldVelocity.normalized;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosotion) + l_WorldVelocityNormalized * m_OffsetTeleportPortal;
        transform.localScale *= (_Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x);
        m_RigidBody.isKinematic = false;
        m_RigidBody.velocity = l_WorldVelocity;
        m_ExitPortal = _Portal.m_MirrorPortal;
    }
}
