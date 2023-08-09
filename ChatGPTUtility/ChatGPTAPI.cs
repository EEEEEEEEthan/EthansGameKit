using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ChatGPTUtility
{
	[Serializable]
	public struct Message
	{
		public string content;
		public string role;
	}

	[Serializable]
	public struct OpenAIRequest
	{
		public Message[] messages;
		public string model;
	}

	[Serializable]
	public struct OpenAIResponse
	{
		public Choice[] choices;
	}

	[Serializable]
	public struct Choice
	{
		public Message message;
	}

	public static class API
	{
		const string url = "https://api.openai.com/v1/chat/completions";
		public static void SendRequest(string apiKey, Message[] messages, Action<string> callback)
		{
			var req = new OpenAIRequest
			{
				model = "gpt-3.5-turbo",
				messages = messages,
			};
			var json = JsonUtility.ToJson(req);
			var postData = Encoding.UTF8.GetBytes(json);
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
			{
				uploadHandler = new UploadHandlerRaw(postData),
				downloadHandler = new DownloadHandlerBuffer(),
			};
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Authorization", "Bearer " + apiKey);
			var asyncOperation = request.SendWebRequest();
			asyncOperation.completed += onCompleted;

			void onCompleted(AsyncOperation asyncOperation)
			{
				// ReSharper disable once MergeIntoLogicalPattern
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.LogError(request.error);
					callback.Invoke(null);
					return;
				}
				try
				{
					var response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
					var message = response.choices[0].message.content;
					try
					{
						callback.Invoke(message);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
					return;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					Debug.LogError(request.downloadHandler.text);
				}
				try
				{
					callback.Invoke(null);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}
