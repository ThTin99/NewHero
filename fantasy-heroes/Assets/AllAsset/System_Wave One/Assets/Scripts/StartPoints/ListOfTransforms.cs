using System.Collections.Generic;
using UnityEngine;
using SemihOrhan.WaveOne.StartPoints.StartPointPickers;

namespace SemihOrhan.WaveOne.StartPoints
{
#pragma warning disable 0649
    public class ListOfTransforms : MonoBehaviour, IStartPoint
    {
        [SerializeField] private List<Transform> startPoints = new List<Transform>();
        [SerializeField] private bool drawGizmos = true;
        private IStartPointPicker startPointPicker;
        private Vector3 v;
        public List<Transform> StartPoints { get => startPoints; set => startPoints = value; }
        List<int> CountList = new List<int>();
        int count;

        private void Start()
        {
            startPointPicker = GetComponent<IStartPointPicker>();
            startPointPicker.SetListSize(startPoints.Count);
        }

        public Vector3 GetPoint()
        {
            if(CountList.Count == 0){
                count = 0;
            }else{
                count++;
            }
            CountList.Add(count);
            if(CountList.Count >= startPoints.Count){
                CountList.Clear();
            }
            if(count >= startPoints.Count){
                count = 0; 
            }
            return v = startPoints[count].position;
        }

        #region Gizmos
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = Color.white;
            for (int i = 0; i < startPoints.Count; i++)
            {
                if (startPoints[i])
                    Gizmos.DrawWireSphere(startPoints[i].position, 0.2f); 
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(v, .1f);
        }
        #endregion
    }
}