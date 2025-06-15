using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStartEncounterOnCollision : MonoBehaviour
{
    [SerializeField] private GameObject encounterPrefab;
    private BoxCollider2D collider;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        Debug.Log("Collision detected with " + other.gameObject.name);

        if(other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            PlayerMovement konrad = other.gameObject.GetComponent<PlayerMovement>();
            //konrad.TriggerIFrames();
            EventBus.Publish(new EncounterStartEvent(encounterPrefab, gameObject));
        }  
    }
}
