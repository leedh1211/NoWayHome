using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerLimiter : MonoBehaviour
{
    void Awake()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        for (int i = 1; i < listeners.Length; i++)
        {
            Destroy(listeners[i]);
        }
    }
}
