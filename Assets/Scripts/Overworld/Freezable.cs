using UnityEngine;

public class Freezable : MonoBehaviour
{
    private MonoBehaviour[] logicScripts;

    // Only disable scripts that are in specific gameplay categories
    [SerializeField] private MonoBehaviour[] ignoreThese;

    private Vector2 storedVelocity2D;
    private float storedAngularVelocity2D;

    private bool storedKinematic2D;

    void Awake()
    {
        logicScripts = GetComponents<MonoBehaviour>();
    }

    public void Freeze()
    {
        foreach (var script in logicScripts)
        {
            if (script != this && !ShouldIgnore(script))
                script.enabled = false;
        }

        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if(rb2D != null)
        {
            storedVelocity2D = rb2D.velocity;
            storedAngularVelocity2D = rb2D.angularVelocity;
            storedKinematic2D = rb2D.isKinematic;
            rb2D.velocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            rb2D.isKinematic = true;
        }
    }

    public void Unfreeze()
    {
        foreach (var script in logicScripts)
        {
            if (script != this && !ShouldIgnore(script))
                script.enabled = true;
        }
        
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if(rb2D != null)
        {
            rb2D.velocity = storedVelocity2D;
            rb2D.angularVelocity = storedAngularVelocity2D;
            rb2D.isKinematic = storedKinematic2D;
        }
    }

    private bool ShouldIgnore(MonoBehaviour script)
    {
        foreach (var ignored in ignoreThese)
        {
            if (script == ignored) return true;
        }
        return false;
    }
}