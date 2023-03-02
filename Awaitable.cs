using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace EthansGameKit
{
	public interface IAwaitable : INotifyCompletion
	{
		public static IAwaitable Create(out IAsyncHandle handle)
		{
			return Awaiter<object>.Create(out handle);
		}
		public static IAwaitable Create(out IAsyncTrigger trigger, out IAsyncStopper stopper)
		{
			var awaitable = Awaiter<object>.Create(out IAsyncHandle handle);
			trigger = handle;
			stopper = handle;
			return awaitable;
		}
		StateCode State { get; }
		IAwaiter GetAwaiter();
	}

	// ReSharper disable once TypeParameterCanBeVariant
	public interface IAwaitable<T> : IAwaitable
	{
		public static IAwaitable<T> Create(out IAsyncHandle<T> handle)
		{
			return Awaiter<T>.Create(out handle);
		}
		public static IAwaitable<T> Create(out IAsyncTrigger<T> trigger, out IAsyncStopper stopper)
		{
			var awaitable = Awaiter<T>.Create(out IAsyncHandle<T> handle);
			trigger = handle;
			stopper = handle;
			return awaitable;
		}
		new IAwaiter<T> GetAwaiter();
	}

	public interface IAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		object GetResult();
	}

	public interface IAwaiter<out T> : IAwaiter
	{
		new T GetResult();
	}

	public interface IAsyncTrigger
	{
		void Set();
	}

	public interface IAsyncTrigger<in T>
	{
		void Set(T result);
	}

	public interface IAsyncStopper
	{
		void Cancel();
	}

	public interface IAsyncHandle : IAsyncTrigger, IAsyncStopper
	{
	}

	public interface IAsyncHandle<in T> : IAsyncTrigger<T>, IAsyncStopper
	{
	}

	public enum StateCode
	{
		Inactive,
		Awaiting,
		Completed,
		Canceled,
	}

	class Awaiter<T>
		: IAsyncHandle, IAwaiter<T>, IAwaitable<T>, IAsyncHandle<T>
	{
		public static IAwaitable Create(out IAsyncHandle trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}
		public static IAwaitable<T> Create(out IAsyncHandle<T> trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}
		public static Awaiter<T> Generate()
		{
			return new();
		}
		Action continuation;
		T result;
		public bool IsCompleted => State is StateCode.Completed or StateCode.Canceled;
		public StateCode State { get; private set; }
		public void Cancel()
		{
			Assert.IsTrue(!IsCompleted, $"already completed. {this}");
			State = StateCode.Canceled;
			continuation?.Invoke();
			continuation = null;
		}
		public void Set()
		{
			Set(default);
		}
		public void Set(T result)
		{
			//Debug.Log($"{nameof(Set)}({result})");
			Assert.IsTrue(!IsCompleted, $"already completed. {this}");
			State = StateCode.Completed;
			this.result = result;
			continuation?.Invoke();
			continuation = null;
		}
		IAwaiter IAwaitable.GetAwaiter()
		{
			return this;
		}
		IAwaiter<T> IAwaitable<T>.GetAwaiter()
		{
			return this;
		}
		public void OnCompleted(Action continuation)
		{
			//Debug.Log($"{nameof(OnCompleted)}({continuation})");
			Assert.IsTrue(State == StateCode.Inactive);
			State = StateCode.Awaiting;
			this.continuation = continuation;
		}
		object IAwaiter.GetResult()
		{
			return null;
		}
		T IAwaiter<T>.GetResult()
		{
			return result;
		}
		public override string ToString()
		{
			return $"{GetType().FullName}({nameof(State)}={State})";
		}
	}
}
