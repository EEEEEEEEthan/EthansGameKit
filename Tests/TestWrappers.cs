using System.Collections;
using System.Collections.Generic;
using EthansGameKit.Collections.Wrappers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EthansGameKit.Tests
{
	class TestWrappers
	{
		[Test]
		public void ListToDict()
		{
			for (var i = 0; i < 100; ++i)
				test(i);

			void test(int length)
			{
				var list = new List<float>();
				for (var i = 0; i < length; ++i)
					list.Add(Random.value);
				var wrapped = list.WrapAsDictionary();
				Assert.AreEqual(list.Count, wrapped.Count);
				for (var i = 0; i < length; ++i)
					Assert.AreEqual(list[i], wrapped[i]);
				foreach (var (key, value) in wrapped)
					Assert.AreEqual(list[key], value);
			}
		}
		[Test]
		public void ConvertedEnumerator()
		{
			for (var i = 0; i < 100; ++i)
				test(i);

			void test(int length)
			{
				var list = new List<float>();
				for (var i = 0; i < length; ++i)
					list.Add(Random.value);
				using var enumerator = list.GetEnumerator().WrapAsConvertedEnumerator(value => value);
				using var rawEnumerator = list.GetEnumerator();
				for (var i = 0; i < length; ++i)
				{
					var a = rawEnumerator.MoveNext();
					var b = enumerator.MoveNext();
					Assert.AreEqual(a, b);
					if (a && b)
						Assert.AreEqual(rawEnumerator.Current, enumerator.Current);
				}
			}
		}

		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		[UnityTest]
		public IEnumerator TestWrappersWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// Use yield to skip a frame.
			yield return null;
		}
	}
}
