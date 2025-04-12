using System;
using System.Collections.Generic;
using Aya.Events;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace BzKovSoft.ObjectSlicer.Samples
{
    /// <summary>
    /// 该脚本必须附加到具有标记为"IsTrigger"碰撞体的GameObject上。
    /// </summary>
    public class BzKnife : MonoBehaviour
    {
        private Vector3 _previousPosition;
        private Vector3 _currentPosition;

        [SerializeField] private Vector3 _origin = Vector3.down;
        [SerializeField] private Vector3 _direction = Vector3.up;
        private Rigidbody _rigidbody;

        public List<Food> cuttingFoods;
        // public Vector3 normal = Vector3.up; // 平面法向量
        // public Vector3 point = Vector3.zero; // 平面上一点
        public float planeSize = 5f; // 平面尺寸
        public Color planeColor = Color.green; // 平面颜色

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _previousPosition = _currentPosition;
            _currentPosition = transform.position;

            // Debug.Log(cuttingFoods.Count);
        }

        /// <summary>
        /// 获取刀刃的原点位置
        /// </summary>
        public Vector3 Origin
        {
            get
            {
                Vector3 localShifted = transform.InverseTransformPoint(transform.position) + _origin;
                return transform.TransformPoint(localShifted);
            }
        }

        /// <summary>
        /// 获取刀刃的方向
        /// </summary>
        public Vector3 BladeDirection => transform.rotation * _direction.normalized;

        /// <summary>
        /// 获取刀刃的运动方向
        /// </summary>
        public Vector3 MoveDirection => transform.right;

        // public Vector3 EdgeDirection => transform.forward;

        /// <summary>
        /// 当接触食物时调用
        /// </summary>
        public void OnEnterFood(Food food, ref Plane plane)
        {
            if (cuttingFoods.Count == 0)
                UEvent.Dispatch(EventType.OnKnifeTouchBegin, BladeDirection);

            if (!cuttingFoods.Contains(food))
            {
                cuttingFoods.Add(food);
                Vector3 collisionPoint = GetCollisionPoint();
                Vector3 normal = Vector3.Cross(MoveDirection, BladeDirection);

                plane = new Plane(normal, collisionPoint);

                // food.OnKnifeEnter(plane, transform.position, -transform.forward);
            }
        }

        /// <summary>
        /// 当食物离开时调用
        /// </summary>
        public void OnExitFood(Food food)
        {
            if (cuttingFoods.Contains(food))
                cuttingFoods.Remove(food);

            if (cuttingFoods.Count == 0)
                UEvent.Dispatch(EventType.OnKnifeTouchEnd);
        }

        /// <summary>
        /// 获取碰撞点
        /// </summary>
        private Vector3 GetCollisionPoint()
        {
            Vector3 distanceToObject = transform.position - Origin;
            Vector3 projected = Vector3.Project(distanceToObject, BladeDirection);
            return Origin + projected;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 normal = Vector3.Cross(MoveDirection, BladeDirection);
            // Vector3 point = transform.position;

            // Vector3 right = Vector3.Cross(normal, Vector3.up);
            // if (right.magnitude < 0.1f)
            //     right = Vector3.Cross(normal, Vector3.forward);
            // right.Normalize();

            // Vector3 forward = Vector3.Cross(normal, right).normalized;
            Gizmos.color = planeColor;
            Vector3 collisionPoint = GetCollisionPoint();

            Vector3 corner1 = collisionPoint + MoveDirection * planeSize + BladeDirection * planeSize;
            Vector3 corner2 = collisionPoint + MoveDirection * planeSize - BladeDirection * planeSize;
            Vector3 corner3 = collisionPoint - MoveDirection * planeSize - BladeDirection * planeSize;
            Vector3 corner4 = collisionPoint - MoveDirection * planeSize + BladeDirection * planeSize;

            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner4);
            Gizmos.DrawLine(corner4, corner1);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(collisionPoint, collisionPoint + normal * planeSize);

            Gizmos.color = Color.blue;
            foreach(var foods in cuttingFoods){
                Gizmos.DrawLine(foods.in_knifePos, transform.position);
            }

            
        }
    }
}
