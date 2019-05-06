using System.Collections.Generic;
using UnityEngine;

namespace MLAgents
{
    public class RoboRayPerception3D : RoboRayPerception
    {
        private string[] myTeam;
        private string[] enemyTeam;
        Vector3 endPosition;
        RaycastHit hit;
        public override List<float> Perceive(float rayDistance,
            float[] rayAngles, string[] detectableObjects,
            float startOffset, float endOffset)
        {
            perceptionBuffer.Clear();
            foreach (float angle in rayAngles)
            {
                endPosition = transform.TransformDirection(
                    PolarToCartesian(rayDistance, angle));
                endPosition.y = endOffset;
                if (Application.isEditor)
                {
                    Debug.DrawRay(transform.position + new Vector3(0f, startOffset, 0f),
                        endPosition, Color.black, 0.01f, true);
                }

                float[] subList = new float[detectableObjects.Length + 2];
                if (Physics.SphereCast(transform.position +
                                       new Vector3(0f, startOffset, 0f), 0.5f,
                    endPosition, out hit, rayDistance))
                {
                    for (int i = 0; i < detectableObjects.Length; i++)
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        print(hit.transform.tag + "\t" + hitObject.tag);
                        if (hitObject.CompareTag("frontArmor"))
                        {
                            
                        }
                        if (hitObject.CompareTag(detectableObjects[i]))
                        {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            Debug.Log(hitObject.tag + "\t" + angle.ToString() + "\t" + detectableObjects[i] + "  \t" + subList[i].ToString());
                            break;
                        }
                        
                    }
                }
                else
                {
                    subList[detectableObjects.Length] = 1f;
                    Debug.Log(angle.ToString() + "\t" + subList[detectableObjects.Length].ToString());
                }
                perceptionBuffer.AddRange(subList);
            }

            return perceptionBuffer;
        }
        /// <summary>
        /// Converts polar coordinate to cartesian coordinate.
        /// </summary>
        public static Vector3 PolarToCartesian(float radius, float angle)
        {
            float x = radius * Mathf.Cos(DegreeToRadian(angle));
            float z = radius * Mathf.Sin(DegreeToRadian(angle));
            return new Vector3(x, 0f, z);
        }

    }
}