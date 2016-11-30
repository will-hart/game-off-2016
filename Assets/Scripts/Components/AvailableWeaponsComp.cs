using System.Collections.Generic;
using GameGHJ.Common.ZenECS;

public class AvailableWeaponsComp : ComponentECS
{
    public List<WeaponComp> availableWeaponsList;

    public AvailableWeaponsComp() : base()
    {
    }
	
	public override void SetOwner(Entity _entity)
	{
		Owner = _entity;
		foreach (var cmp in availableWeaponsList)
		{
			cmp.SetOwner(_entity);
		}
	}

    public override ComponentTypes ComponentType => ComponentTypes.AvailableWeaponsComp;
}