using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_RefractionCube;
    public LayerMask m_RefractionLayerMask;
    public float m_MaxRefractionCubeDistance = 250.0f;
    public bool m_RefractionEnabled;
    bool m_isAttached = false;
    Portal m_ExitPortal = null;
    Rigidbody m_RigidBody;
    public float m_OffsetTeleportPortal = 1.5f;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_RefractionEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_RefractionCube.gameObject.SetActive(m_RefractionEnabled);
    }

    public void CreateRefraction()
    {
        Ray l_Ray = new Ray(m_RefractionCube.transform.position, m_RefractionCube.transform.forward);
        float l_RefractionCubeDistance = m_MaxRefractionCubeDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxRefractionCubeDistance, m_RefractionLayerMask.value))
        {
            l_RefractionCubeDistance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.tag == "RefractionCube" || l_RaycastHit.collider.tag == "Laser")
            {
                l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
            }
        }
        m_RefractionCube.SetPosition(1, new Vector3(0.0f, 0.0f, l_RefractionCubeDistance));
    }

    public void SetAttached(bool Attached)
    {
        m_isAttached = Attached;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            Debug.Log("Laser");
            m_RefractionEnabled = true;
        }
        else if (!other.CompareTag("Laser"))
        {
            m_RefractionEnabled = false;
        }
        if (other.tag == "Portal" && !m_isAttached)
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
