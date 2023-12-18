using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EthansGameKit.Components
{
	[RequireComponent(typeof(Toggle))]
	class ToggleExtension : MonoBehaviour
	{
		[SerializeField] UnityEvent onToggleOn;
		[SerializeField] UnityEvent onToggleOff;
		void OnEnable()
		{
			GetComponent<Toggle>().onValueChanged.AddListener(OnValueChanged);
		}
		void OnValueChanged(bool value)
		{
			try
			{
				if (value) onToggleOn?.Invoke();
				else onToggleOff?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e, this);
			}
		}
	}
}
