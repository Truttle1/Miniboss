using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject bang;

    public void SpawnBang(int damage)
    {
        if (bang != null)
        {
            GameObject bangInstance = Instantiate(bang, transform.position, Quaternion.identity);
            BangNumberController bangScript = bangInstance.GetComponent<BangNumberController>();
            if (bangScript != null)
            {
                bangScript.SetNumber(damage);
            }
        }
        else
        {
            Debug.LogWarning("Bang prefab is not assigned in the inspector.");
        }
    }
}
