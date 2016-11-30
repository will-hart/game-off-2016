using GameGHJ.Common.ZenECS;
using UnityEngine;

public class SelectableUnitComp : ComponentECS
{
    //#DEBUG
    public GameObject selectionCircle;

	public bool IsSelected { get; set; }

	//UISprite sprite = GetComponent<UISprite>();
	public GuiIcons GuiIconType;

    public SelectableUnitComp() : base()
    {
    }

    public override ComponentTypes ComponentType => ComponentTypes.SelectableUnitComp;
}
