interface IPoolRelease
{
	/// This method prepares object for deactivation before it is released back into the pool
	void DeactivateBeforeRelease();
}