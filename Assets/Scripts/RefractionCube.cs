using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_RefractionCube;
    public LayerMask m_RefractionLayerMask;
    public float m_MaxRefractionCubeDistance = 250.0f;
    bool m_RefractionEnabled = false;

    // Update is called once per frame
    void Update()
    {
        m_RefractionCube.gameObject.SetActive(m_RefractionEnabled);
        m_RefractionEnabled = false;
    }

    public void CreateRefraction()
    {
        if (m_RefractionEnabled)
        {
            return;
        }
        Ray l_Ray = new Ray(m_RefractionCube.transform.position, m_RefractionCube.transform.forward);
        float l_RefractionCubeDistance = m_MaxRefractionCubeDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxRefractionCubeDistance, m_RefractionLayerMask.value))
        {
            l_RefractionCubeDistance = Vector3.Distance(m_RefractionCube.transform.position, l_RaycastHit.point);
            if (l_RaycastHit.collider.tag == "RefractionCube")
            {
                l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
            }
        }
        m_RefractionCube.SetPosition(1, new Vector3(0.0f, 0.0f, l_RefractionCubeDistance));
    }
}
