using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCatcher : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("star"))
        {
            //doSometing
        }
    }
}
