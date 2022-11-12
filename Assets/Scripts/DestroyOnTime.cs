using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float m_DestroyOnTimeFn = 3.0f;
    
    void Start()
    {
        StartCoroutine(DestroyOnTimeFn());
    }

    // Update is called once per frame
    IEnumerator DestroyOnTimeFn()
    {
        yield return new WaitForSeconds(m_DestroyOnTimeFn);
        GameObject.Destroy(gameObject);
    }
}
