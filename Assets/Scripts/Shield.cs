using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    public static int shield = 50;
    Text shieldText;
    // Start is called before the first frame update
    void Start()
    {
        shieldText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        shieldText.text = "Shield: " + shield;
    }
}
