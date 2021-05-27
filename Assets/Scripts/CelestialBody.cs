using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [SerializeField] float sma = 15; //semi major axis
    [SerializeField] [Range(0f, 0.99f)] float ecc; // eccentricity
    [SerializeField] [Range(0f, Helpers.tau)] float incl; // inclination
    [SerializeField] [Range(0f, Helpers.tau)] float longitudeOfAcendingNode;  //n - swivel
    [SerializeField] [Range(0f, Helpers.tau)] float argumentOfPeriapsis;      //w - position
    [SerializeField] float meanLongitude;             //L - offset
    [SerializeField] RefBody refBody;
    [SerializeField] float meanAnomaly;
    [Space]
    [SerializeField] float acc = 1e-6f; // accuracy
    [SerializeField] int maxIts = 5;           // Max iterations.

    float mu;
    float n, cosLOAN, sinLOAN, sinI, cosI, realAnomalyConstant;

    public void CalculateSemiConstants()    //Numbers that only need to be calculated once if the orbit doesn't change.
    {
        mu = Helpers.G * refBody.mass;
        n = Mathf.Sqrt(mu / Mathf.Pow(sma, 3));
        realAnomalyConstant = Mathf.Sqrt((1 + ecc) / (1 - ecc));
        cosLOAN = Mathf.Cos(longitudeOfAcendingNode);
        sinLOAN = Mathf.Sin(longitudeOfAcendingNode);
        cosI = Mathf.Cos(incl);
        sinI = Mathf.Sin(incl);
    }

    public float Keppler(float E, float e, float M)
    {
        return (M - E + e * Mathf.Sin(E));
    }

    public float Fkeppler(float E, float e)
    {
        return ((-1f) + e * Mathf.Cos(E));
    }

    private void Awake()
    {
        CalculateSemiConstants();
    }
    void Update()
    {
        CalculateSemiConstants();

        meanAnomaly = n * (Time.time - meanLongitude);

        float initAnomaly = meanAnomaly;
        float diff = 1f;
        for(int i = 0; diff > acc && i < maxIts; i++)
        {
            float secondAnomaly = initAnomaly;
            initAnomaly = secondAnomaly - Keppler(secondAnomaly, ecc, meanAnomaly) / Fkeppler(secondAnomaly, ecc);
            diff = Mathf.Abs(initAnomaly - secondAnomaly);
        }
        float eccAnomaly = initAnomaly;

        float realAnomaly = 2 * Mathf.Atan(realAnomalyConstant * Mathf.Tan(eccAnomaly / 2));
        float dist = sma * (1 - ecc * Mathf.Cos(realAnomaly));

        float cosPeriAndRA = Mathf.Cos(argumentOfPeriapsis + realAnomaly);
        float sinPeriAndRA = Mathf.Sin(argumentOfPeriapsis + realAnomaly);

        float x = dist * ((cosLOAN * cosPeriAndRA) - (sinLOAN * sinPeriAndRA * cosI));
        float z = dist * ((sinLOAN * cosPeriAndRA) - (cosLOAN * sinPeriAndRA * cosI));
        float y = dist * (sinI * sinPeriAndRA);

        transform.position = new Vector3(x, y, z) + refBody.transform.position;
    }
}
