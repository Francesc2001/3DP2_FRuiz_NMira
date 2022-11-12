using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    bool m_IsAttached = false;
    Rigidbody m_RigidBody;
    public float m_OffsetTeleportPortal = 1.5f;
    Portal m_ExitPortal = null;

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
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
    // Update is called once per frame
    void Update()
    {

    }
}