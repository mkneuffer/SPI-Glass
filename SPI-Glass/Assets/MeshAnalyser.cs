using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyser : MonoBehaviour
{
    [SerializeField] private float groundThreshold;
    [SerializeField] private float avgNormal;
    [SerializeField] private float minVerts;
    [SerializeField] private bool isGround;


    public bool IsGround
    {
        get => isGround;
    }

    public float AvgNorm
    {
        get => avgNormal;
    }

    private MeshFilter _meshFilter;

    public event Action analysisDone;
    // Start is called before the first frame update


    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();

        if(_meshFilter != null)
         StartCoroutine(CheckForGround());
    }

    IEnumerator CheckForGround()
    {
        yield return new WaitUntil(() =>
        {
            return _meshFilter.sharedMesh.vertices.Length > minVerts;
        });

        isGround = AnalyseForGround(_meshFilter.sharedMesh);
        analysisDone?.Invoke();
    }

    bool AnalyseForGround(Mesh mesh)
    {
        float averageVert = 0;

        foreach (var normal in mesh.normals)
        {
            averageVert += normal.normalized.y;
        }

        averageVert /= mesh.vertices.Length;
        avgNormal =  averageVert;
        // mat.SetFloat("_LerpValue", avgNormal);

        if (averageVert >= groundThreshold)
        {
            return true;
        }

        return false;
    }
}