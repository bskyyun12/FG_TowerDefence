namespace Tools
{
	public interface IPool<T>
	{
		T Get();
		void ReturnToPool(T objectToReturn);
	}
}
