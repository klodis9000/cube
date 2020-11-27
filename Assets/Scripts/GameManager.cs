using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static event Action OnCubeSpawned = delegate {  };

    private CubeSpawner[] spawners;
    private int spawnerIndex;
    private CubeSpawner currentSpawner;

    private void Awake()
    {
        spawners = FindObjectsOfType<CubeSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //если текущий куб существует и он не основной
            if (MovingCube.CurrentCube != null)
            {
                //оставнока куба
                MovingCube.CurrentCube.Stop();
            }

            //FindObjectOfType<CubeSpawner>().SpawnCube();
            spawnerIndex = spawnerIndex == 0 ? 1 : 0;
            currentSpawner = spawners[spawnerIndex];
            
            currentSpawner.SpawnCube();
            OnCubeSpawned();

        }
    }
}
