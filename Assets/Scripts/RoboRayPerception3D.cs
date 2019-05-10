using System.Collections.Generic;
using UnityEngine;

namespace MLAgents
{
    public class RoboRayPerception3D : RoboRayPerception
    {
        Vector3 endPosition;
        RaycastHit hit;
        public override List<float> Perceive(float rayDistance,
            float[] rayAngles, string[] detectableObjects,
            float startOffset, float endOffset)
        {
            perceptionBuffer.Clear();
            foreach (float angle in rayAngles)
            {
                endPosition = transform.Find("Fixed Pivot").transform.TransformDirection(
                    PolarToCartesian(rayDistance, angle));
                endPosition.y = endOffset;
                if (Application.isEditor)
                {
                    Debug.DrawRay(transform.Find("Fixed Pivot").transform.position + new Vector3(0f, startOffset, 0f),
                        endPosition, Color.black, 0.01f, true);
                }
                float[] subList = new float[detectableObjects.Length + 2];
                if (Physics.SphereCast(transform.Find("Fixed Pivot").transform.position +
                                       new Vector3(0f, startOffset, 0f), 0.3f,
                    endPosition, out hit, rayDistance))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (hit.collider.gameObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }
                perceptionBuffer.AddRange(subList);
            }
            return perceptionBuffer;
        }

        public override List<float> RoboPerceive(float rayDistance,
            float[] rayAngles, string[] detectableObjects,
            float startOffset, float endOffset)
        {
            Vector3 offset = new Vector3(transform.Find("Fixed Pivot").transform.forward.x * 0.15f, startOffset, transform.Find("Fixed Pivot").transform.forward.z * 0.15f);
            perceptionBuffer.Clear();
            hitObject.Clear();
            foreach (float angle in rayAngles)
            {
                Vector3 offsetByAngle = transform.Find("Fixed Pivot").transform.TransformDirection(
                    PolarToCartesian(0.2f, angle));
                endPosition = transform.Find("Fixed Pivot").transform.TransformDirection(
                    PolarToCartesian(rayDistance, angle));
                endPosition.y = endOffset;
                if (Application.isEditor)
                {
                    Debug.DrawRay(transform.Find("Fixed Pivot").transform.position + offset + offsetByAngle,
                        endPosition, Color.black, 0.01f, true);
                }
                float[] subList = new float[detectableObjects.Length + 2];
                GameObject[] subHit = new GameObject[detectableObjects.Length + 2];
                if (Physics.SphereCast(transform.Find("Fixed Pivot").transform.position + offset + offsetByAngle, 0.3f,
                    endPosition, out hit, rayDistance-0.2f))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        if (detectableObjects[i] == "wall")
                        {
                            // 그 자체 Object Tag 비교
                            GameObject temp = hit.collider.gameObject;
                            if (temp.CompareTag(detectableObjects[i]))
                            {
                                subList[i] = 1;
                                subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                                subHit[i] = temp;
                                break;
                            }
                        }
                        else
                        {
                            // 부모 Object Tag 비교
                            GameObject temp = hit.transform.gameObject;
                            if (temp.CompareTag(detectableObjects[i]))
                            {
                                subList[i] = 1;
                                subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                                subHit[i] = temp;                                
                                break;
                            }
                        }
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                }
                perceptionBuffer.AddRange(subList);
                hitObject.AddRange(subHit);
            }
            //for (int i = 0; i < perceptionBuffer.Count; i++)
            //{
            //    print(rayAngles[i / 5].ToString() + "\t" + perceptionBuffer[i].ToString() + "\t" + hitObject[i]);
            //}
            return perceptionBuffer;
        }

        public static Vector3 PolarToCartesian(float radius, float angle)
        {
            float x = radius * Mathf.Cos(DegreeToRadian(angle));
            float z = radius * Mathf.Sin(DegreeToRadian(angle));
            return new Vector3(x, 0f, z);
        }
    }
}