using UnityEngine;
using System.Collections;

public class ParticleEffectManager : MonoBehaviour {

    public ParticleSystem[] dust;
    public ParticleSystem[] tongueStick;
    public ParticleSystem[] muchSpeed;

    public float timeBetweenSpeedParticles = 1;

    private bool canGenerateSpeedPart = true;

    public void spawnParticles(ParticleSystem particles, Vector3 location, float despawnTime)
    {
        particles.transform.position = location;
        particles.gameObject.SetActive(true);
        particles.time = 0;
        StartCoroutine(mementoMori(particles, despawnTime));
    }

    public void generateDust(Vector3 pos)
    {
        int av = getAvailableParticle(dust);
        spawnParticles(dust[av], pos, dust[av].duration);
    }

    public void generateSalivaSplash(Vector3 pos)
    {
        int av = getAvailableParticle(tongueStick);
        spawnParticles(tongueStick[av], pos, tongueStick[av].duration);
    }

    public void generateSpeedParticles(Vector3 pos)
    {
        if (canGenerateSpeedPart)
        {
            int av = getAvailableParticle(muchSpeed);
            spawnParticles(muchSpeed[av], pos, timeBetweenSpeedParticles);
        }
    }

    IEnumerator mementoMori(ParticleSystem particles, float time)
    {
        canGenerateSpeedPart = false;

        yield return new WaitForSeconds(time);
        particles.gameObject.SetActive(false);

        canGenerateSpeedPart = true;
    }

    int getAvailableParticle(ParticleSystem[] sys)
    {
        int PositionInArray = -1;
        int sizeOfArray = sys.Length;

        for (int a = 0; a < 100; a++)
        {
            if (a > sizeOfArray - 1)
            {
                break;
            }

            if (!sys[a].gameObject.activeInHierarchy)
            {
                PositionInArray = a;
                break;
            }
        }

        if (PositionInArray >= 0)
            return PositionInArray;
        else
            return 0;


    }
}
