using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[InitializeOnLoad]
[ExecuteInEditMode]
public class NamingOverride
{
	//Register callback the minute project loads
	static NamingOverride()
	{
		EditorApplication.hierarchyWindowChanged += RenameCallback;
	}
	
	static void RenameCallback()
	{
		if (Application.isPlaying) return;

		bool doubleDigits = false;
		bool noNumberAtAll = false;
		Object[] all = Resources.FindObjectsOfTypeAll(typeof(Object));
		for (int allIdx = 0; allIdx < all.Length; allIdx++)
		{
			if (all[allIdx].name.Contains("(1)"))
			{
				String newName = all[allIdx].name.Replace("(1)", "");
				newName = newName.Trim();

				//Get last 2 chars
				string endTwoStr = newName.Substring(newName.Length - 2, 2);
				int index = -1;

				try
				{
					index = Convert.ToInt32(endTwoStr);
					index++;
					//newName = newName.Substring(0, newName.Length - 2) + (index + 1);
					doubleDigits = true;
					//D.Log("Double digits true: " + index);
				}
				catch (FormatException) // Last 2 chars is not a number
				{
					//Get last character
					string numString = newName[newName.Length - 1].ToString();
					index = -1;

					try
					{
						//see if last character is a number
						index = Convert.ToInt32(numString);
						index++;
						//newName = newName.Substring(0, newName.Length - 1) + (index + 1);
					}
					catch (FormatException) // Last char is not a number
					{
						//all[allIdx].name = newName + "_2";
						index = 1;
						noNumberAtAll = true;
						//break;
					}
				}

				if (doubleDigits)
				{
					do
					{
						if (newName.Substring(newName.Length - 3, 1) != "_")
						{
							//string oldName = newName;
							newName = newName + "_2";

							//foreach (var o in all.Where(x => x.name.Equals(oldName)))
							//{
							//	o.name = oldName + "_1";
							//}
						}
						else
							newName = newName.Substring(0, newName.Length - 2) + index;
						index++;
					} while (all.Count(n => n.name.Contains(newName)) > 0);
				}
				else if (! noNumberAtAll)
				{
					do
					{
						//D.Log("check for _ finds: " + newName.Substring(newName.Length - 2, 1));
						if (newName.Substring(newName.Length - 2, 1) != "_")
						{
							//string oldName = newName;
							newName = newName + "_2";

							//foreach (var o in all.Where(x => x.name.Equals(oldName)))
							//{
							//	o.name = oldName + "_1";
							//}
						}
						else
							newName = newName.Substring(0, newName.Length - 1) + index;
						index++;
					} while (all.Count(n => n.name.Contains(newName)) > 0);
				}
				else //has no number
				{
					//string oldName = newName;
					do
					{
						newName = newName + "_" + (index+1).ToString();
						index++;
					} while (all.Count(n => n.name.Contains(newName)) > 0);

					//foreach (var o in all.Where(x => x.name.Equals(oldName)))
					//{
					//	o.name = oldName + "_1";
					//}
				}


				all[allIdx].name = newName;
				break;
			}
		}
	}
}