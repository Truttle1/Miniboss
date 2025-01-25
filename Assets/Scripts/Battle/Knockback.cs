using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 knockbackPosition;

    private IEnumerator knockback(float time)
    {
        float startTime = Time.time;
        float duration = .1f;
        float progress = 0;

        if (!GetComponent<HasHP>().isBlocking())
        {
            if (GetComponent<Monster>() != null)
            {
                GetComponent<Monster>().setHurtAnimation(true);
            }
            if (GetComponent<Konrad>() != null)
            {
                GetComponent<Konrad>().setHurtAnimation(true);
            }
        }

        while (progress < 1.0f)
        {
            yield return null;
            progress = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(startPosition, knockbackPosition, progress);
        }
        yield return new WaitForSeconds(time);

        startTime = Time.time;
        duration = .4f;
        progress = 0;
        while (progress < 1.0f)
        {
            yield return null;
            progress = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(knockbackPosition, startPosition, progress);
        }

        if (GetComponent<Monster>() != null)
        {
            GetComponent<Monster>().setHurtAnimation(false);
        }

        if (GetComponent<Konrad>() != null)
        {
            GetComponent<Konrad>().setHurtAnimation(false);
        }
    }

    public void doKnockback(float amount, float time)
    {
        startPosition = transform.position;
        if(GetComponent<HasHP>() != null)
        {
            if (GetComponent<HasHP>().isBlocking())
            {
                amount *= 0.3f;
            }
        }
        knockbackPosition = transform.position + new Vector3(amount, 0, 0);
        StartCoroutine(knockback(time));
    }
}
