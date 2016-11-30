public interface IPoolInit 
{
	/// This method runs after the object is retrieved from the pool and should be used to re-initialize it
	void InitFromPool();
}
