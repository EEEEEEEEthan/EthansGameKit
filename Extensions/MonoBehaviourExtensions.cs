using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void EditorSetField(this MonoBehaviour @this, string field, Object value)
		{
#if UNITY_EDITOR
			var serializedObject = new UnityEditor.SerializedObject(@this);
			serializedObject.FindProperty(field).objectReferenceValue = value;
			serializedObject.ApplyModifiedProperties();
#endif
		}
		public static IAwaitable<bool> AwaitCoroutine(this MonoBehaviour @this, IEnumerator routine)
		{
			var awaitable = IAwaitable<bool>.Create(out var handle);
			@this.StartCoroutine(wrappedEnumerator());
			return awaitable;

			IEnumerator wrappedEnumerator()
			{
				while (true)
				{
					bool next;
					try
					{
						next = routine.MoveNext();
					}
					catch (Exception e)
					{
						Debug.LogException(e);
						goto FAIL;
					}
					if (next)
					{
						yield return routine.Current;
					}
					else
					{
						goto SUCCESS;
					}
				}
			SUCCESS:
				yield return null;
				handle.Set(true);
				yield break;
			FAIL:
				yield return null;
				handle.Set(false);
			}
		}
	}
}
