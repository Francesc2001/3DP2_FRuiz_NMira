using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_PortalTransform;
    public Portal m_MirrorPortal;
    public FPSControllerPlayer m_Player;
    public float m_OffsetNearPlane;
    public List<Transform> m_ValidPoints;
    public float m_MinValidDistance = 0.1f;
    public float m_MaxValidDistance = 9f;
    public float m_MinDotValidAngle = 0.995f;
    public LineRenderer m_lineRenderer;

    void Start()
    {
        m_lineRenderer.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        Vector3 l_WorldPosition = m_Player.m_Camera.transform.position;
        Vector3 l_LocalPosition = m_PortalTransform.InverseTransformPoint(l_WorldPosition);
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = m_Player.m_Camera.transform.forward;
        Vector3 l_LocalDirection = m_PortalTransform.InverseTransformDirection(l_WorldDirection);
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        float l_Distance = Vector3.Distance(m_MirrorPortal.m_Camera.transform.position, m_MirrorPortal.transform.position);
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance + m_OffsetNearPlane;


        if (m_lineRenderer.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public bool IsValidPosition(Vector3 StartPosition, Vector3 forward, float MaxDistance, LayerMask PortalLayerMask, out Vector3 Position, out Vector3 Normal)
    {
        Ray l_Ray = new Ray(StartPosition, forward);
        RaycastHit l_RaycastHit;
        bool l_Valid = false;
        Position = Vector3.zero;
        Normal = Vector3.forward;

        if (Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
        {
            if (l_RaycastHit.collider.tag == "DrawableWall")
            {
                l_Valid = true;
                Debug.Log("Valid");
                Normal = l_RaycastHit.normal;
                Position = l_RaycastHit.point;
                transform.position = Position;
                transform.rotation = Quaternion.LookRotation(Normal);

                for (int i = 0; i < m_ValidPoints.Count; i++)
                {
                    Vector3 l_Direction = m_ValidPoints[i].position - StartPosition;
                    l_Direction.Normalize();
                    l_Ray = new Ray(StartPosition, l_Direction);
                    if (Physics.Raycast(l_Ray, out l_RaycastHit, MaxDistance, PortalLayerMask.value))
                    {
                        if (l_RaycastHit.collider.tag == "DrawableWall")
                        {
                            float l_Distance = Vector3.Distance(Position, l_RaycastHit.point);
                            float l_DotAngle = Vector3.Dot(Normal, l_RaycastHit.normal);
                            Debug.Log("dist " + l_Distance + " " + l_DotAngle);
                            if (!(l_Distance >= m_MinValidDistance && l_Distance <= m_MaxValidDistance && l_DotAngle > m_MinDotValidAngle))
                            {
                                Debug.Log("Not Valid");
                                l_Valid = false;
                                m_lineRenderer.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            l_Valid = false;
                            m_lineRenderer.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("Not Valid");
                        l_Valid = false;
                        m_lineRenderer.gameObject.SetActive(false);
                    }
                }
            }
        }
        return l_Valid;
    }
}
