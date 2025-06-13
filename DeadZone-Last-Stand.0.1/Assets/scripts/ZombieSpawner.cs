using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Prefabs de cada tipo de zombie")]
    public GameObject zombieBasicoPrefab;
    public GameObject zombieRapidoPrefab;
    public GameObject zombieTanquePrefab;
    public GameObject zombieMutantePrefab;

    [Header("Datos ScriptableObject de cada tipo de zombie")]
    public Zombie zombieBasicoData;
    public Zombie zombieRapidoData;
    public Zombie zombieTanqueData;
    public Zombie zombieMutanteData;

    [Header("Puntos de aparición")]
    public Transform[] puntosSpawn;

    [Header("Oleadas automáticas")]
    public int cantidadInicial = 200;
    public int incrementoPorMinuto = 50;
    public float tiempoEntreOleadas = 60f;

    private int cantidadZombiesActual;

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
        tiposZombies.Add(new TipoZombie(zombieBasicoData, zombieBasicoPrefab));
        tiposZombies.Add(new TipoZombie(zombieRapidoData, zombieRapidoPrefab));
        tiposZombies.Add(new TipoZombie(zombieTanqueData, zombieTanquePrefab));
        tiposZombies.Add(new TipoZombie(zombieMutanteData, zombieMutantePrefab));

        // Spawnea la tanda inicial
        cantidadZombiesActual = cantidadInicial;
        for (int i = 0; i < cantidadZombiesActual; i++)
        {
            SpawnearZombieAleatorio();
            Debug.Log($" Spawneado zombie {i + 1}/{cantidadZombiesActual}");
        }

        // Inicia la generación automática
        StartCoroutine(SpawnearZombiesCadaMinuto());
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

    private IEnumerator SpawnearZombiesCadaMinuto()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreOleadas);

            cantidadZombiesActual += incrementoPorMinuto;
            Debug.Log($"🧟 Nueva oleada: {cantidadZombiesActual} zombies");

            for (int i = 0; i < cantidadZombiesActual; i++)
            {
                SpawnearZombieAleatorio();
            }
        }
    }
}
