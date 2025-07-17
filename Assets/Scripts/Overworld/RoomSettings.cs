using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSettings : MonoBehaviour
{
    public static RoomSettings Instance;

    [SerializeField] private AudioClip music;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioClip GetMusic()
    {
        return music;
    }
}
