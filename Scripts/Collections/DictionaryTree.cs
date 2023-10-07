using System;
using System.Collections.Generic;
using System.Text;
using EthansGameKit.CachePools;

namespace EthansGameKit.Collections
{
	public class DictionaryTree<T>
	{
		class Node : IDisposable
		{
			static Node Generate()
			{
				if (GlobalCachePool<Node>.TryGenerate(out var node)) return node;
				return new();
			}
			readonly SortedDictionary<char, Node> children = new();
			public bool HasValue { get; private set; }
			public T Value { get; private set; }
			public IReadOnlyDictionary<char, Node> Children => children;
			bool Empty => !HasValue && children.Count == 0;
			public void Dispose()
			{
				Clear();
				GlobalCachePool<Node>.Recycle(this);
			}
			public void Clear()
			{
				Value = default;
				foreach (var child in children.Values) child.Dispose();
				children.Clear();
				HasValue = false;
			}
			public void Set(string text, T value, int index)
			{
				if (index >= text.Length)
				{
					HasValue = true;
					Value = value;
					return;
				}
				var character = text[index];
				if (!children.TryGetValue(character, out var child))
					children[character] = child = Generate();
				child.Set(text, value, index + 1);
			}
			public void Add(string text, T value, int index)
			{
				if (index >= text.Length)
				{
					if (HasValue) throw new ArgumentException($"Duplicate key: {text}");
					HasValue = true;
					Value = value;
					return;
				}
				var character = text[index];
				if (!children.TryGetValue(character, out var child))
					children[character] = child = Generate();
				child.Set(text, value, index + 1);
			}
			public bool Remove(string text, int index)
			{
				if (index >= text.Length)
				{
					if (HasValue)
					{
						HasValue = false;
						Value = default;
						return true;
					}
					return false;
				}
				var character = text[index];
				if (children.TryGetValue(character, out var child))
				{
					if (child.Remove(text, index + 1))
					{
						if (child.Empty)
						{
							children.Remove(character);
							child.Dispose();
						}
						return true;
					}
					return false;
				}
				return false;
			}
			public bool Contains(string text, int index)
			{
				if (index >= text.Length) return HasValue;
				var character = text[index];
				if (children.TryGetValue(character, out var child))
					return child.Contains(text, index + 1);
				return false;
			}
			public T GetValue(string text, int index)
			{
				if (index >= text.Length)
				{
					return HasValue ? Value : throw new KeyNotFoundException(text);
				}
				var character = text[index];
				return children[character].GetValue(text, index + 1);
			}
			public bool TryGetValue(string text, int index, out T value)
			{
				if (index >= text.Length)
				{
					value = Value;
					return HasValue;
				}
				var character = text[index];
				if (children.TryGetValue(character, out var child))
					return child.TryGetValue(text, index + 1, out value);
				value = default;
				return false;
			}
			public IEnumerable<Node> IterChildNodes(StringBuilder builder)
			{
				yield return this;
				foreach (var (c, node) in children)
				{
					builder.Append(c);
					foreach (var n in node.IterChildNodes(builder))
						yield return n;
				}
			}
		}

		readonly Node root = new();
		public T this[string key]
		{
			get => root.GetValue(key, 0);
			set => root.Set(key, value, 0);
		}
		public void Add(string key, T value)
		{
			root.Add(key, value, 0);
		}
		public bool ContainsKey(string key)
		{
			return root.Contains(key, 0);
		}
		public bool Remove(string key)
		{
			return root.Remove(key, 0);
		}
		public bool TryGetValue(string key, out T value)
		{
			return root.TryGetValue(key, 0, out value);
		}
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			var builder = StringBuilderPool.Generate();
			foreach (var node in root.IterChildNodes(builder))
			{
				if (node.HasValue)
				{
					yield return new(builder.ToString(), node.Value);
				}
			}
			StringBuilderPool.ClearAndRecycle(ref builder);
		}
		public void Clear()
		{
			root.Clear();
		}
	}
}
