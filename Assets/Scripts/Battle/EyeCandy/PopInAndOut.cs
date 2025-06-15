using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopInAndOut : MonoBehaviour
{
    [SerializeField] private AnimationCurve pop;

    public float duration = 1.0f;
    public float popDuration = 0.5f;

    public float scaleMultiplier = 2;
    void Start()
    {
        StartCoroutine(popInAndOut());
    }

    private IEnumerator popInAndOut()
    {
        float time = 0f;
        Vector3 startScale = Vector3.zero;
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float t = pop.Evaluate(time / popDuration);
            gameObject.transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.one * scaleMultiplier, t);
            yield return null;
        }
        gameObject.transform.localScale = Vector3.one * scaleMultiplier;

        // Wait for a moment before popping out
        yield return new WaitForSeconds(duration);

        time = 0f;
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float t = pop.Evaluate(1 - (time / popDuration));
            transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.one * scaleMultiplier, t);
            yield return null;
        }

        Destroy(gameObject); // Destroy the object after popping out
        
    }
}
