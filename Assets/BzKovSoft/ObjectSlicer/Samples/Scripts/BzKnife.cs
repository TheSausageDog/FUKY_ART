using System;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace BzKovSoft.ObjectSlicer.Samples
{
	/// <summary>
	/// The script must be attached to a GameObject that have collider marked as a "IsTrigger".
	/// </summary>
	public class BzKnife : MonoBehaviour
	{
		Vector3 _prevPos;
		Vector3 _pos;

		[SerializeField]
		private Vector3 _origin = Vector3.down;

		[SerializeField]
		private Vector3 _direction = Vector3.up;

		private void Update()
		{
			_prevPos = _pos;
			_pos = transform.position;
		}

		/// <summary>
		/// Origin of the knife
		/// </summary>
		public Vector3 Origin
		{
			get
			{
				Vector3 localShifted = transform.InverseTransformPoint(transform.position) + _origin;
				return transform.TransformPoint(localShifted);
			}
		}
		private Rigidbody _rb;

		/// <summary>
		/// The direction the knife is pointed to
		/// </summary>
		public Vector3 BladeDirection { get { return transform.rotation * _direction.normalized; } }
		/// <summary>
		/// The nnife moving direction
		/// </summary>
		public Vector3 MoveDirection
		{
			get { return -transform.right; }
		}

		void OnDrawGizmosSelected()
		{
			var color = Gizmos.color;
			var direction = transform.rotation * _direction;
			var from = Origin;
			var to = Origin + direction;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(from, to);
			Gizmos.DrawSphere(from, 0.06f);
			DrawArrowEnd(from, direction);
			Gizmos.color = color;
		}

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
		}

		private static void DrawArrowEnd(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
			Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
			Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
			right *= arrowHeadLength;
			left *= arrowHeadLength;
			up *= arrowHeadLength;
			down *= arrowHeadLength;

			Vector3 center = pos + direction;
			Gizmos.DrawRay(center, right);
			Gizmos.DrawRay(center, left);
			Gizmos.DrawRay(center, up);
			Gizmos.DrawRay(center, down);
			
			Gizmos.DrawLine(center + right, center + left);
			Gizmos.DrawLine(center + up, center + down);
		}

		bool isTrigging = false;
		async void OnTriggerEnter(Collider other)
		{
			if (isTrigging|| _rb.velocity.magnitude<0.05f)
			{
				return;
			}
			
			var slicer = other.GetComponentInParent<IBzMeshSlicer>();
			if (slicer == null)
			{
				return;
			}
			
			isTrigging = true;

			Vector3 point = GetCollisionPoint();
			Vector3 normal = Vector3.Cross(MoveDirection, BladeDirection);
			
			//if(normal==Vector3.zero)Debug.Log(MoveDirection+" "+BladeDirection);
			
			Plane plane = new Plane(normal, point);
			this.normal = normal;
			this.point = point;

			await slicer.SliceAsync(plane);

			await Task.Delay(TimeSpan.FromSeconds(0.5f));
			
			isTrigging = false;
		}


			public Vector3 normal = Vector3.up;  // 平面的法向量
			public Vector3 point = Vector3.zero; // 平面上的一点
			public float planeSize = 5f;         // 平面尺寸
			public Color planeColor = Color.green; // 颜色

			private void OnDrawGizmos()
			{
				//if (normal == Vector3.zero) return;

				// 计算平面上的两个方向向量
				Vector3 right = Vector3.Cross(normal, Vector3.up);
				if (right.magnitude < 0.1f)
				{
					right = Vector3.Cross(normal, Vector3.forward);
				}
				right.Normalize();

				Vector3 forward = Vector3.Cross(normal, right).normalized;

				// 绘制平面
				Gizmos.color = planeColor;
				Vector3 corner1 = point + right * planeSize + forward * planeSize;
				Vector3 corner2 = point + right * planeSize - forward * planeSize;
				Vector3 corner3 = point - right * planeSize - forward * planeSize;
				Vector3 corner4 = point - right * planeSize + forward * planeSize;

				Gizmos.DrawLine(corner1, corner2);
				Gizmos.DrawLine(corner2, corner3);
				Gizmos.DrawLine(corner3, corner4);
				Gizmos.DrawLine(corner4, corner1);

				// 绘制法向量
				Gizmos.color = Color.red;
				Gizmos.DrawLine(point, point + normal * planeSize);
			}
		


		private Vector3 GetCollisionPoint()
		{
			Vector3 distToObject = transform.position - Origin;
			Vector3 proj = Vector3.Project(distToObject, BladeDirection);

			Vector3 collisionPoint = Origin + proj;
			return collisionPoint;
		}
	}
}
