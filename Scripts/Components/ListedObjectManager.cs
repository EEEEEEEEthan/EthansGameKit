using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit.Components
{
	public class ListedObjectManager : MonoBehaviour
	{
		[SerializeField] GameObject prefab;
		[SerializeField, HideInInspector] List<GameObject> activeObjects;
		[SerializeField, HideInInspector] List<GameObject> inactiveObjects;
		void Awake()
		{
			prefab.SetActive(false);
		}
		public void Clear()
		{
			foreach (var obj in activeObjects)
			{
				obj.SetActive(false);
				inactiveObjects.Add(obj);
			}
			activeObjects.Clear();
		}
		public GameObject Generate()
		{
			GameObject obj;
			if (inactiveObjects.Count <= 0)
			{
				obj = Instantiate(prefab, prefab.transform.parent);
			}
			else
			{
				var index = inactiveObjects.Count - 1;
				obj = inactiveObjects[index];
				inactiveObjects.RemoveAt(index);
			}
			obj.SetActive(true);
			activeObjects.Add(obj);
			return obj;
		}
	}
}
