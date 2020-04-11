using System.Collections.Generic;
using Tools;
using UnityEngine;

public class ObjectPool<T> : IPool<T> where T : IResettable
{
	private Queue<T> objs = new Queue<T>();
	IFactory<T> factory;

	public ObjectPool(IFactory<T> factory)
	{
		this.factory = factory;
	}

	public T Get()
	{
		if (objs.Count == 0)
		{
			var newObj = factory.Create();
			objs.Enqueue(newObj);	// inactive
		}

		return objs.Dequeue();	// active
	}


	public void ReturnToPool(T objectToReturn)
	{
		objectToReturn.Reset(); // IResettable
		objs.Enqueue(objectToReturn);// inactive
	}
}

public interface IResettable
{
	void Reset();
}

public interface IFactory<T>
{
	T Create();
}

public class PrefabFactory<T> : IFactory<T>
{
	public GameObject Prefab { get; set; }

	public PrefabFactory(GameObject prefab/*, string name*/)
	{
		this.Prefab = prefab;
	}

	public T Create()
	{
		GameObject tempGameObject = GameObject.Instantiate(Prefab) as GameObject;
		T objectOfType = tempGameObject.GetComponent<T>();
		return objectOfType;
	}
}