using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleItem
{
    public GameObject objectToPooler;
    public int amountToPool;
}

public class Particles : MonoBehaviour
{
    public static Particles instance;

    [Header("Particle")]
    public List<ParticleItem> particlesToPool;
    public List<GameObject> instancedParticles;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instancedParticles = new List<GameObject>();
        foreach (ParticleItem item in particlesToPool) //recorres la lista! 
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject go = Instantiate(item.objectToPooler);
                go.SetActive(false);
                instancedParticles.Add(go);
            }
        }
    }

    public GameObject GetPoolObject(string tag)
    {
        for (int i = 0; i < instancedParticles.Count; i++)
        {
            if (!instancedParticles[i].activeInHierarchy && instancedParticles[i].CompareTag(tag))
            {
                return instancedParticles[i];
            }
        }
        foreach (ParticleItem item in particlesToPool)
        {
            if (item.objectToPooler.CompareTag(tag))
            {
                GameObject go = Instantiate(item.objectToPooler); //lo agregamos a la lista de la pool, de objetos que ya pueden ser usados!
                go.SetActive(false);
                instancedParticles.Add(go); //intanciamos a la lista de objetos que podemos usar
                return go;
            }
        }
        return null;
    }
}

