using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEm : MonoBehaviour
{
    public GameObject runner;

    int count = 0;
    private void OnGUI()
    {
        if (GUILayout.Button("Spawn Runner"))
        {
            count++;
            var _run = GameObject.Instantiate(runner);
            _run.name = $"Runner {count}";
        }
    }
}
