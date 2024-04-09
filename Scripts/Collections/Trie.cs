using System;
using System.Collections;
using System.Collections.Generic;

namespace EthansGameKit.Collections
{
	public interface IReadOnlyTrie : IReadOnlyCollection<string>
	{
		Trie.Searcher GetSearcher();
		IEnumerable<string> Search(string prefix, int maxLength = 256);
		bool Contains(string word);
	}

	public class Trie : IReadOnlyTrie
	{
		public class Searcher
		{
			readonly Trie tree;
			Node node;
			char[] current = new char[1];
			int currentLength;
			public Searcher(Trie tree)
			{
				this.tree = tree;
				Reset();
			}
			public string Current => new(current, 0, currentLength);
			public void Reset()
			{
				node = tree.root;
				currentLength = 0;
			}
			public bool ValidNext(char character, out bool end)
			{
				if (node.dict is null)
				{
					end = false;
					return false;
				}
				if (node.dict.TryGetValue(character, out var next))
				{
					end = next.end;
					return true;
				}
				end = false;
				return false;
			}
			public bool ValidNext(char character)
			{
				return node?.dict?.ContainsKey(character) ?? false;
			}
			public bool ValidNext(string input, out bool end)
			{
				end = false;
				var node = this.node;
				foreach (var c in input)
				{
					if (node.dict is null || !node.dict.TryGetValue(c, out node))
						return false;
				}
				end = node.end;
				return true;
			}
			public bool ValidNext(string input)
			{
				var node = this.node;
				foreach (var c in input)
				{
					if (node.dict is null || !node.dict.TryGetValue(c, out node))
						return false;
				}
				return true;
			}
			public bool AddNext(char character)
			{
				return AddNext(character, out _);
			}
			public bool AddNext(char character, out bool end)
			{
				if (currentLength >= current.Length) Array.Resize(ref current, current.Length * 2);
				current[currentLength++] = character;
				if (node.dict is null)
				{
					node = null;
					end = false;
					return false;
				}
				if (node.dict.TryGetValue(character, out node))
				{
					end = node.end;
					return true;
				}
				end = false;
				return false;
			}
			public bool AddNext(string input)
			{
				return AddNext(input, out _);
			}
			public bool AddNext(string input, out bool end)
			{
				if (currentLength + input.Length >= current.Length) Array.Resize(ref current, current.Length * 2);
				input.CopyTo(0, current, currentLength, input.Length);
				end = false;
				foreach (var c in input)
				{
					if (!AddNext(c, out end))
						return false;
				}
				return true;
			}
		}

		class Node
		{
			public SortedDictionary<char, Node> dict;
			public bool end;
			public bool Add(string word, int index)
			{
				var length = word.Length;
				if (index >= length)
				{
					if (!end)
					{
						end = true;
						return true;
					}
					return false;
				}
				dict ??= new();
				var c = word[index];
				if (!dict.TryGetValue(c, out var node)) dict[c] = node = new();
				return node.Add(word, index + 1);
			}
			public bool Remove(string word, int index)
			{
				var length = word.Length;
				if (index >= length - 1)
				{
					if (end)
					{
						end = false;
						return true;
					}
					return false;
				}
				if (dict is null) return false;
				var c = word[index];
				if (!dict.TryGetValue(c, out var node)) return false;
				node.Remove(word, index + 1);
				if (node.dict is null && !node.end)
				{
					dict.Remove(c);
					if (dict.Count <= 0) dict = null;
					return true;
				}
				return false;
			}
			public IEnumerable<int> Collect(char[] buffer, int index)
			{
				if (end)
					yield return index;
				if (dict != null)
				{
					foreach (var pair in dict)
					{
						buffer[index] = pair.Key;
						foreach (var length in pair.Value.Collect(buffer, index + 1))
							yield return length;
					}
				}
			}
			public Node Find(string prefix)
			{
				var node = this;
				foreach (var c in prefix)
				{
					node.dict.TryGetValue(c, out node);
					if (node is null) return null;
				}
				return node;
			}
		}

		readonly Node root = new();
		Searcher innerSearcher;
		public Trie()
		{
			innerSearcher = new(this);
		}
		public int Count { get; private set; }
		public Searcher GetSearcher()
		{
			return new(this);
		}
		public IEnumerable<string> Search(string prefix, int maxLength = 256)
		{
			var buffer = new char[maxLength];
			var array = prefix.ToCharArray();
			array.CopyTo(buffer, 0);
			var node = root.Find(prefix);
			foreach (var length in node.Collect(buffer, prefix.Length))
				yield return new(buffer, 0, length);
		}
		public bool Contains(string word)
		{
			return root.Find(word) != null;
		}
		public IEnumerator<string> GetEnumerator()
		{
			return Search("").GetEnumerator();
		}
		public void Add(string word)
		{
			if (root.Add(word, 0))
				Count += 1;
		}
		public bool Remove(string word)
		{
			if (root.Remove(word, 0))
			{
				Count -= 1;
				return true;
			}
			return false;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Search("").GetEnumerator();
		}
	}
}
