using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoboRayPerception : MonoBehaviour
{
    protected List<GameObject> hitObject = new List<GameObject>();
    protected List<float> perceptionBuffer = new List<float>();

    public virtual List<float> Perceive(float rayDistance,
        float[] rayAngles, string[] detectableObjects,
        float startOffset, float endOffset)
    {
        return perceptionBuffer;
    }

    public virtual List<float> RoboPerceive(float rayDistance,
        float[] rayAngles, string[] detectableObjects,
        float startOffset, float endOffset)
    {
        return perceptionBuffer;
    }

    public static float DegreeToRadian(float degree)
    {
        return degree * Mathf.PI / 180f;
    }

    public List<GameObject> GetHitObject()
    {
        return hitObject;
    }

    internal IEnumerable<float> Perceive(float rayDistance, float[] rayAngles, string[] detectableObjects)
    {
        throw new NotImplementedException();
    }
}