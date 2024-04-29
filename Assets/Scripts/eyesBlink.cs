using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyesBlink : MonoBehaviour
{
    SpriteRenderer eyelidTex;
    float time;
    float randInt;

    // Start is called before the first frame update
    void Start()
    {
        eyelidTex = this.GetComponent<SpriteRenderer>();
        time = 0;
        randInt = Random.Range(1.2f, 2.4f);

        eyelidTex.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= randInt)
        {
            time = 0;
            randInt = Random.Range(1.2f, 2.4f);

            eyelidTex.enabled = true;
        }

        if (time >= 0.1f && eyelidTex.enabled)
        {
            eyelidTex.enabled = false;
        }
    }
}
