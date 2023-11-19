using UnityEngine;

public class ParticleMN
{
    public void TurnOnParticle(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        particle.Play();
    }

    public void TurnOffParticle(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        particle.Stop();
    }
}
