using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeRemaining : MonoBehaviour
{
    public static int life = 100;
    Text lifeText;
    // Start is called before the first frame update
    void Start()
    {
        lifeText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeText.text = "Life: " + life;
    }
}
