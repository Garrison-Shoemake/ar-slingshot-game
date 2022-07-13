using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBake : MonoBehaviour
{
    [SerializeField] NavMeshSurface field;

    private void Awake()
    {
        field = GetComponent<NavMeshSurface>();
    }

    public void bakeMesh()
    {
        field.BuildNavMesh();
    }
}
