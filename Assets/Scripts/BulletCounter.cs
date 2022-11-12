using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletCounter : MonoBehaviour
{
    public int l_MaxBullets = 30;
    public static int l_MinBullets = 0;
    public static int m_CurrentBullets;
    Text bulletsCounter;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentBullets = l_MaxBullets;
        bulletsCounter = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_CurrentBullets > l_MinBullets)
        {
            Shot();
        }

        bulletsCounter.text = "Bullets Left: " + m_CurrentBullets;
    }

    void Shot()
    {
        m_CurrentBullets--;
    }
}
