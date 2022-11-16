using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalButtonSpawner : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionPrefab;

    public void Spawn()
    {
        GameObject l_GameObject = GameObject.Instantiate(m_CompanionPrefab);
        l_GameObject.transform.position = m_SpawnPosition.position;
        l_GameObject.transform.rotation = m_SpawnPosition.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Cube"))
        {
            Spawn();
        }
    }
}
