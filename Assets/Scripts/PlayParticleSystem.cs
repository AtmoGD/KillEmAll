using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSys;

    public void Play()
    {
        particleSys.Play();
    }
}
