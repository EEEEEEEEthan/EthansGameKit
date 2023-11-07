using System;
using Object = UnityEngine.Object;

namespace EthansGameKit.Assurance
{
	[Serializable]
	public struct Problem
	{
		public string title;
		public string details;
		public MessageType type;
		public Object context;
		public Problem(string title, string details, MessageType type, Object context)
		{
			this.title = title;
			this.details = details;
			this.type = type;
			this.context = context;
		}
	}
}
