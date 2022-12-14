using UnityEngine;
using System.Collections.Generic;

public class calibrationWithKabsch : MonoBehaviour
{
    public Transform[] inPoints;
    public Transform[] referencePoints;
    Vector3[] points; Vector4[] refPoints;
    private List<Vector3[]> inPointsList = new List<Vector3[]>();
    private List<Vector4[]> refPointsList = new List<Vector4[]>();
    public bool collectingSamples = true;
    KabschSolver solver = new KabschSolver();
    public GameObject Calibration;
    public GameObject CalibrationProbe;
    public int pointsPerSample =4;
    //Set up the Input Points
    void Start()
    {
        points = new Vector3[inPoints.Length];
        refPoints = new Vector4[inPoints.Length];

    }

    //Calculate the Kabsch Transform and Apply it to the input points
    void Update()
    {



    }
    public void Calibrate()
    {
        if (collectingSamples)
        {
            points = new Vector3[inPointsList.Count * pointsPerSample];
            refPoints = new Vector4[refPointsList.Count * pointsPerSample];
            foreach (var subSamplePoints in inPointsList)
            {
                for (int i = 0; i < subSamplePoints.Length; i++)
                {
                    points[i] = subSamplePoints[i];
                }

            }
            foreach (var subSamplePoints in refPointsList)
            {
                for (int i = 0; i < subSamplePoints.Length; i++)
                {
                    refPoints[i] = subSamplePoints[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < inPoints.Length; i++)
            {
                points[i] = inPoints[i].position;
            }

            for (int i = 0; i < inPoints.Length; i++)
            {
                refPoints[i] = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
            }
        }
        

        Vector3 inCentroid = Vector3.zero; Vector3 refCentroid = Vector3.zero;
        float inTotal = 0f, refTotal = 0f;
        for (int i = 0; i < inPoints.Length; i++)
        {
            inCentroid += new Vector3(inPoints[i].position.x, inPoints[i].position.y, inPoints[i].position.z) * refPoints[i].w;
            inTotal += refPoints[i].w;
            refCentroid += new Vector3(refPoints[i].x, refPoints[i].y, refPoints[i].z) * refPoints[i].w;
            refTotal += refPoints[i].w;
        }
        inCentroid /= inTotal;
        refCentroid /= refTotal;
        Vector3 vec = inCentroid - refCentroid;

        Calibration.transform.position = -vec;
        for (int i = 0; i < inPoints.Length; i++)
        {
            points[i] = inPoints[i].position;
        }
        foreach (var item in points)
        {
            Debug.Log(item);
        }
        foreach (var item in refPoints)
        {
            Debug.Log(item);
        }
        Matrix4x4 kabschTransform = solver.SolveKabsch(points, refPoints);
        Calibration.transform.position = kabschTransform.MultiplyPoint(Vector3.zero);
        Calibration.transform.rotation = kabschTransform.rotation;
        /*for (int i = 0; i < inPoints.Length; i++)
        {
            inPoints[i].position = kabschTransform.MultiplyPoint(points[i]);
        }*/
        CalibrationProbe.SetActive(false);



    }
    public void CollectSample()
    {
        points = new Vector3[inPoints.Length];
        refPoints = new Vector4[inPoints.Length];
        for (int i = 0; i < inPoints.Length; i++)
        {
            points[i] = inPoints[i].position;
        }

        for (int i = 0; i < inPoints.Length; i++)
        {
            refPoints[i] = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
        }
        inPointsList.Add(points);
        refPointsList.Add(refPoints);
        Debug.Log("SampleCollected");

    }
}