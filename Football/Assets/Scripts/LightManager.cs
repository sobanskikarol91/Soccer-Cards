using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    Light[] reflectors;
    float timeBetweenTurnOn = 0.5f;

    private void Start()
    {
        reflectors = GetComponentsInChildren<Light>();
        foreach (Light l in reflectors)
            l.enabled = false;
    }

    public IEnumerator IETurnOnReflectors()
    {
        for (int i = 0; i < reflectors.Length; i++)
        {
            reflectors[i].enabled = true;
            reflectors[i].GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(timeBetweenTurnOn);
        }
    }
}
