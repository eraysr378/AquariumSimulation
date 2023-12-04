using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PredatorPreferredDepthSO : ScriptableObject
{
    public float depthCalmMin;
    public float depthCalmMax;
    public float depthPassivePredationMax;
    public float depthPassivePredationMin;
    public float depthActivePredationMax;
    public float depthActivePredationMin;
}
