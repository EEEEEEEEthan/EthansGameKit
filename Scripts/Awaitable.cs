using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace EthansGameKit
{
	public interface IAwaitable : INotifyCompletion
	{
		public static IAwaitable Generate(out IAsyncHandle handle) => Awaiter<object>.Generate(out handle);

		public static IAwaitable<T> Generate<T>(out IAsyncHandle<T> handle) => Awaiter<T>.Generate(out handle);

		StateCode State { get; }

		IAwaiter GetAwaiter();
	}

	public interface IAwaitable<out T> : IAwaitable
	{
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

	public interface IAsyncHandle : IAsyncTrigger
	{
	}

	public interface IAsyncHandle<in T> : IAsyncTrigger<T>
	{
	}

	public enum StateCode
	{
		Inactive,
		Awaiting,
		Completed
	}

	internal class Awaiter<T> : IAsyncHandle, IAwaiter<T>, IAwaitable<T>, IAsyncHandle<T>
	{
		internal static IAwaitable Generate(out IAsyncHandle trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}

		internal static IAwaitable<T> Generate(out IAsyncHandle<T> trigger)
		{
			var awaitable = Generate();
			trigger = awaitable;
			return awaitable;
		}

		static Awaiter<T> Generate() => new();

		event Action continuation;
		T result;
		public bool IsCompleted => State == StateCode.Completed;
		public StateCode State { get; private set; }

		public void OnCompleted(Action continuation)
		{
			Assert.AreNotEqual(State, StateCode.Completed, "already completed");
			State = StateCode.Awaiting;
			this.continuation += continuation;
		}

		public void Set()
		{
			Assert.AreNotEqual(State, StateCode.Completed, "already completed");
			State = StateCode.Completed;
			continuation?.TryInvoke();
			continuation = null;
		}

		public void Set(T result)
		{
			Assert.AreNotEqual(State, StateCode.Completed, "already completed");
			State = StateCode.Completed;
			this.result = result;
			continuation?.TryInvoke();
			continuation = null;
		}

		IAwaiter IAwaitable.GetAwaiter() => this;

		object IAwaiter.GetResult() => null;

		IAwaiter<T> IAwaitable<T>.GetAwaiter() => this;

		T IAwaiter<T>.GetResult() => result;
	}
}