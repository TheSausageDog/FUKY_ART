﻿using System;
using System.Threading.Tasks;
using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

namespace BzKovSoft.ObjectSlicer
{
	/// <summary>
	/// Base class for sliceable object with a static mesh
	/// </summary>
	public class BzSliceableObject : BzSliceableBase
	{
		[Tooltip("小于这个值就不发射粒子")]
		public float minSplashSize = 0.1f;
		[Tooltip("大于这个值发射三个，小于这个值发射一个")]
		public float maxSplashSize = 0.3f;

		private Plane cutPlane; // 切割平面
		[NonSerialized] public Vector3 in_knifePos; // 刀的位置
		[NonSerialized] public Vector3 in_edgeDir; // 刀的方向
		[NonSerialized] public BzKnife knife; // 刀的引用

		public bool cutted; // 是否已切割
		[HideInInspector] public float cuttingDepth => VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;

		[NonSerialized] public float volume; // 食物体积

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent<BzKnife>(out var _knife))
			{
				if (knife == null)
				{
					Vector3 move_dir = Vector3.Normalize(transform.position - _knife.transform.position);
					if (Vector3.Dot(move_dir, _knife.BladeDirection) > Mathf.Epsilon)
					{
						// Debug.Log(Vector3.Dot(move_dir, _knife.EdgeDirection));
						OnKnifeEnter(_knife);
					}
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent<BzKnife>(out var _knife))
			{
				OnKnifeExit(_knife);
			}
		}

		public void Update()
		{
			if (knife != null)
			{
				if (CheckCutted())
				{
					HandleSlice(cutPlane);
				}

				// timer += Time.deltaTime;
				// if (timer > 1 && knife != null)
				// {
				//     // Knife.OnFoodExit(this);
				// }
			}
		}


		protected bool CheckCutted()
		{
			Vector3 knife_move = knife.transform.position - in_knifePos;
			Vector3 knife_in_dist = Vector3.Project(knife_move, in_edgeDir);
			// float dot = Vector3.Dot(knife_in_dist, in_edgeDir);
			// Debug.Log(knife_move.magnitude + " " + knife_in_dist.magnitude + " " + cuttingDepth + " " + (knife_in_dist.magnitude > cuttingDepth));
			return knife_in_dist.magnitude > cuttingDepth;
		}

		/// <summary>
		/// 切割处理逻辑
		/// </summary>
		async void HandleSlice(Plane plane)
		{
			var slicer = GetComponent<IBzMeshSlicer>();
			var sliceResults = await slicer.SliceAsync(plane);
			cutted = true;

			if (sliceResults != null && sliceResults.resultObjects != null)
			{
				foreach (var resultObject in sliceResults.resultObjects)
				{
					resultObject.gameObject.layer = LayerMask.NameToLayer("Default");
					if (resultObject.gameObject.TryGetComponent(out BzSliceableObject slicedObject))
					{
						slicedObject.cutted = true;
					}
				}
			}

			// int splashCount = volume < maxSplashSize ? 1 : 3;
			// splashCount = volume < minSplashSize ? UnityEngine.Random.Range(0, 2) : splashCount;

			// SFXManager.Instance.PlaySfx(SFXName.Food, transform.position, foodColor, splashCount);
			OnKnifeExit(knife);
		}

		public void OnKnifeEnter(BzKnife _knife)
		{
			_knife.OnEnterObject(this, ref cutPlane);

			// cutPlane = plane;

			// what if have multiable knife
			knife = _knife;
			in_knifePos = knife.transform.position;
			in_edgeDir = knife.BladeDirection;
			gameObject.layer = LayerMask.NameToLayer("Player");
		}

		public void OnKnifeExit(BzKnife _knife)
		{
			_knife.OnExitObject(this);

			in_knifePos = Vector3.zero;
			in_edgeDir = Vector3.zero;
			knife = null;

			// what for?
			gameObject.layer = LayerMask.NameToLayer("Default");
		}

		protected override AdapterAndMesh GetAdapterAndMesh(Renderer renderer)
		{
			var meshRenderer = renderer as MeshRenderer;

			if (meshRenderer != null)
			{
				var result = new AdapterAndMesh();
				result.mesh = meshRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
				result.adapter = new BzSliceMeshFilterAdapter(renderer.transform);
				return result;
			}

			return null;
		}

		public override async Task<BzSliceTryResult> SliceAsync(Plane plane, object sliceData)
		{
			var res = await base.SliceAsync(plane, sliceData);


			// if (TryGetComponent<Food>(out Food food))
			// {
			// 	food.CalculateTaste();
			// }

			return res;
		}

		/// <summary>
		/// 计算食物的味道属性。
		/// </summary>
		// public async void CalculateTaste()
		// {
		//     var standardValues = await FoodManager.Instance.GetStandardValueAsync(foodType);
		//     volume = await VolumeCalculator.CalculateVolumesAsync(gameObject);

		//     var tastes = new List<Taste>();

		//     foreach (var taste in standardValues)
		//     {
		//         var newTaste = taste;
		//         newTaste.tasteValue *= volume;
		//         tastes.Add(newTaste);
		//     }

		//     Tastes = new AllTaste(tastes);
		// }
	}
}