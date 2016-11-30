public interface IDataManager
{
	void SaveToJSON<T>(T classToSave, string optionalInstanceID = "");
}