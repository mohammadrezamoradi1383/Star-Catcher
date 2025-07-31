using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star : MonoBehaviour
{
   [SerializeField] private int starScore = 20;
   public static event Action<int> OnStarHit;

   private bool hasTriggered = false; 

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (hasTriggered) return; 

      hasTriggered = true; 
      OnStarHit?.Invoke(starScore);
      Destroy(gameObject);
   }
}
