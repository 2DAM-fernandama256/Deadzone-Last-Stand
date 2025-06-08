using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Prefabs de cada tipo de zombie")]
    public GameObject zombieBasicoPrefab;
    public GameObject zombieRapidoPrefab;
    public GameObject zombieTanquePrefab;
    public GameObject zombieMutantePrefab;



    [Header("Puntos de aparición")]
    public Transform[] puntosSpawn;

    // Estructura de datos para tipos + prefab
    private struct TipoZombie
    {
        public Zombie datos;
        public GameObject prefab;

        public TipoZombie(Zombie datos, GameObject prefab)
        {
            this.datos = datos;
            this.prefab = prefab;
        }
    }

    private List<TipoZombie> tiposZombies = new List<TipoZombie>();

    void Start()
    {
        // Cargar cada tipo de zombie con su prefab y comportamiento
        tiposZombies.Add(new TipoZombie(new ZombieBasico(), zombieBasicoPrefab));
        tiposZombies.Add(new TipoZombie(new ZombieRapido(), zombieRapidoPrefab));
        tiposZombies.Add(new TipoZombie(new ZombieTanque(), zombieTanquePrefab));
        tiposZombies.Add(new TipoZombie(new ZombieMutante(), zombieMutantePrefab));

        // Spawnea una tanda inicial
        for (int i = 0; i < 10; i++)
        {
            SpawnearZombieAleatorio();
        }
    }

    public void SpawnearZombieAleatorio()
    {
        int tipoIndex = Random.Range(0, tiposZombies.Count);
        int puntoIndex = Random.Range(0, puntosSpawn.Length);

        TipoZombie tipoSeleccionado = tiposZombies[tipoIndex];

        GameObject zombieGO = Instantiate(tipoSeleccionado.prefab, puntosSpawn[puntoIndex].position, Quaternion.identity);

        ZombieIA ia = zombieGO.GetComponent<ZombieIA>();
        ia.Configurar(tipoSeleccionado.datos);
    }
}
