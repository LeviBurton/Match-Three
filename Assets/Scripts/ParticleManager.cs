using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Required] public GameObject clearFXPrefab;
    [Required] public GameObject breakFXPrefab;
    [Required] public GameObject doubleBreakFXPrefab;
    [Required] public GameObject bombFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z = 0)
    {
        if (clearFXPrefab != null)
        {
            GameObject clearFX = Instantiate(clearFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;

            ParticlePlayer particlePlayer = clearFX.GetComponent<ParticlePlayer>();
            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }

    public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
    {
        GameObject breakFX = null;
        ParticlePlayer particlePlayer = null;

        if (breakableValue > 1)
        {
            breakFX = Instantiate(doubleBreakFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
        }
        else
        {
            breakFX = Instantiate(breakFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
        }

        if (breakFX != null)
        {
            particlePlayer = breakFX.GetComponent<ParticlePlayer>();
            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }

    public void BombFXAt(int x, int y, int z = 0)
    {
        GameObject bombFX = Instantiate(bombFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
        var particlePlayer = bombFX.GetComponent<ParticlePlayer>();
        if (particlePlayer)
        {
            particlePlayer.Play();
        }
    }
}