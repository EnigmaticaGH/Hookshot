using UnityEngine;
using System.Collections;

public class ParticleEffectManager : MonoBehaviour {

    public ParticleSystem dust;
    public ParticleSystem tongueStick;
    public ParticleSystem muchSpeed;

    public float timeBetweenSpeedParticles = 1;

    private bool canGenerateSpeedPart = true;

    public void spawnParticles(ParticleSystem particles, Vector3 location, float despawnTime)
    {
        ParticleSystem part = Instantiate(particles, location, Quaternion.Euler(particles.transform.eulerAngles)) as ParticleSystem;
        StartCoroutine(mementoMori(part, despawnTime));
    }

    public void generateDust(Vector3 pos)
    {
        spawnParticles(dust, pos, dust.duration);
    }

    public void generateSalivaSplash(Vector3 pos)
    {
        spawnParticles(tongueStick, pos, dust.duration);
    }

    public void generateSpeedParticles(Vector3 pos)
    {
        if (canGenerateSpeedPart)
        {
            spawnParticles(muchSpeed, pos, timeBetweenSpeedParticles);
        }
    }

    IEnumerator mementoMori(ParticleSystem particles, float time)
    {
        canGenerateSpeedPart = false;

        yield return new WaitForSeconds(time);
        Destroy(particles.gameObject);

        canGenerateSpeedPart = true;
    }
}
