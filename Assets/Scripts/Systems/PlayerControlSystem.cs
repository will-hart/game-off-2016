#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using GUIns;
using UnityEngine;

namespace GameGHJ.Systems
{
	public class PlayerControlSystem : ZenSingleton<PlayerControlSystem>, IOnAwake, IOnUpdate
	{
		public override int ExecutionPriority => -50;
		public override Type ObjectType => typeof(PlayerControlSystem);

		private bool isSelecting;
		private Vector3 mousePositionStart;

		private Camera cam;
		private ZenBehaviourManager zbm;
		public List<SelectableUnitComp> selectedObjects = new List<SelectableUnitComp>();
		private List<SelectableUnitComp> newSelections = new List<SelectableUnitComp>();
		private GameObject selectionCirclePrefab;
		private int selectableLayerMask;
		private int terrainLayerMask;
		private bool selectionCombineOn;
		private bool hoveringUI;

		public CursorSelectStates selectState;

		[SerializeField] private Texture2D cursorNormalTex, cursorPositionTex, cursorTargetTex;

		[SerializeField] private GameObject uiRoot;

		public void OnAwake()
		{
			zbm = ZenBehaviourManager.Instance;
			cam = Camera.main;
			//selectableLayerMask = LayerMask.GetMask("Selectable");//, "SelectableObstacles");
			selectableLayerMask = LayerManager.SelectableMask;
			terrainLayerMask = LayerManager.TerrainMask;
			selectionCirclePrefab = Resources.Load("Prefabs/UI/SelectionCircle") as GameObject;
			selectState = CursorSelectStates.Normal;
			Cursor.SetCursor(cursorNormalTex, Vector2.zero, CursorMode.Auto );
			
			//haha FML
			if (!uiRoot) uiRoot = GameObject.Find("UI Root");
		}

		public void OnUpdate()
		{
			if (hoveringUI) return;
			if (selectState == CursorSelectStates.Normal)
			{
				////removed drag select for time being
				////Start of selection
				//if (Input.GetMouseButtonDown(0))
				//{
				//	StartNewSelectionDrag();
				//}
				//
				//// Ended selection drag
				//if (Input.GetMouseButtonUp(0))
				//{
				//	EndSelectionDrag();
				//}

				//Right click movement
				if (Input.GetMouseButtonDown(1) && selectedObjects.Count > 0)
				{
					Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
					RaycastHit hitInfo;
					if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, terrainLayerMask))
					{
						//Debug.Log($"Rmb clicked, hit at {hitInfo.point}");
						foreach (var sel in selectedObjects)
						{
							if (sel.Owner.HasComponent(ComponentTypes.HeroComp)) // selected hero
							{
								var tai = sel.Owner.GetComponent<TacticalAiStateComp>();
								tai.NavigationTarget = hitInfo.point;
								tai.NavigationTargetUpdated = true;
							}
						}
					}
				}

				if (Input.GetMouseButtonUp(0))
					RaycastClickNormal();
			} else if (selectState == CursorSelectStates.PositionPick)
			{
				if (Input.GetMouseButtonUp(0))
				{
					RaycastClickPosition();
				}
				if (Input.GetMouseButtonUp(1))
				{
					SetState(CursorSelectStates.Normal);
				}
			}
			else if (selectState == CursorSelectStates.TargetPick)
			{
				if (Input.GetMouseButtonUp(0))
				{
					RaycastClickTarget();
				}

				if (Input.GetMouseButtonUp(1))
				{
					SetState(CursorSelectStates.Normal);
				}
			} else if (selectState == CursorSelectStates.Construction)
			{
				if (Input.GetMouseButtonUp(1))
				{
					BuildingConstructionController.Instance.CancelBuildProcess();
				}
			}

			
		}

		private void RaycastClickPosition()
		{
			Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, terrainLayerMask))
			{
				Debug.Log($"Clicked Terrain for position at : {hitInfo.point}");
			

				foreach (var sel in selectedObjects)
				{
					if (sel.Owner.HasComponent(ComponentTypes.HeroComp)) // selected hero
					{
						sel.Owner.GetComponent<TacticalAiStateComp>().SetNewMovementTarget(hitInfo.point);
						
					}
				}
				SetState(CursorSelectStates.Normal);
			}
		}

		private void RaycastClickTarget()
		{
			Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			
			if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, selectableLayerMask, QueryTriggerInteraction.Ignore))
			{
				Debug.Log($"Targeted {hitInfo.transform.gameObject.name}");
				var ew = hitInfo.transform.gameObject.GetComponent<EntityWrapper>();

				var hc = ew.entity.GetComponent<HealthComp>();

				foreach (var sel in selectedObjects)
				{
					if (sel.Owner.HasComponent(ComponentTypes.HeroComp)) // selected hero
					{
						sel.Owner.GetComponent<TacticalAiStateComp>().SetNewAttackTarget(hc);
					}
				}
				SetState(CursorSelectStates.Normal);
			}
			
		}

		private void StartNewSelectionDrag()
		{
			isSelecting = true;
			mousePositionStart = Input.mousePosition;
		}

		private void EndSelectionDrag()
		{
			//Toggle for adding new selection to current rather than replacing it
			selectionCombineOn = Input.GetButton("SelectionCombine");

			//Select highlighted
			newSelections.Clear();
			foreach (var selectObj in zbm.Get<SelectableUnitComp>(ComponentTypes.SelectableUnitComp))
			{
				// No drag selecting buildings, have to manually click them
				if (IsWithinSelectionBounds(selectObj) && !selectObj.Owner.HasComponent(ComponentTypes.BuildingComp))
				{
					newSelections.Add(selectObj);
					AddSelectedCircle(selectObj);
				}
			}

			if (newSelections.Count > 0) // Made new selection, clear the old
			{
				if (!selectionCombineOn)
					ResetCurrentSelections();

				foreach (var so in newSelections)
				{
					AddSelectedObject(so);
				}
				TriggerSelectionChanged();
			}
			else // No drag, check for direct click hit
			{
				RaycastClickNormal();
			}

			isSelecting = false;
		}

		private void RaycastClickNormal()
		{
			Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, selectableLayerMask))
			//if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue))
			{
				Debug.Log($"Clicked : {hitInfo.transform.gameObject.name}");
				//Clicked on game object
				if (!selectionCombineOn)
					ResetCurrentSelections();

				//Ensure player's object
				var ent = hitInfo.transform.GetComponent<EntityWrapper>().entity;

				if (ent.GetComponent<UnitPropertiesComp>().teamID == 1)
				{
					AddSelectedObject(ent.GetComponent<SelectableUnitComp>());
				}
			}
			else // Something unselectable clicked, clear selections entirely
			{
				if (!selectionCombineOn)
					ResetCurrentSelections();
			}
			TriggerSelectionChanged();
		}

		private void ResetCurrentSelections()
		{
			//Remove all selections currently existing
			foreach (var selectObj in selectedObjects)
			{
				if (selectObj.selectionCircle != null)
				{
					selectObj.selectionCircle.gameObject.Release();
					selectObj.selectionCircle = null;
				}
			}
			selectedObjects.Clear();

			GUIManagementSystem.Instance.IsDirty = true;
		}

		private void AddObjectToSelectionList(SelectableUnitComp selectableObject)
		{
			selectedObjects.Add(selectableObject);
		}

		private void AddSelectedCircle(SelectableUnitComp selectableObject)
		{
			if (selectableObject.selectionCircle == null)
			{
				selectableObject.selectionCircle = selectionCirclePrefab.InstantiateFromPool();
				selectableObject.selectionCircle.transform.SetParent(selectableObject.Owner.GetComponent<PositionComp>().transform,
					false);
				selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);
			}
		}

		public void AddSelectedObject(SelectableUnitComp selectableObject)
		{
			if (selectedObjects.Contains(selectableObject)) // Don't select twice
				return;

			AddSelectedCircle(selectableObject);
			AddObjectToSelectionList(selectableObject);
			GUIManagementSystem.Instance.IsDirty = true;
		}

		public void MakeSingleSelection(SelectableUnitComp selectableObject)
		{
			ResetCurrentSelections();
			AddSelectedObject(selectableObject);
		}

		private void RemoveSelectedCircle(SelectableUnitComp selectableObject)
		{
			if (selectableObject.selectionCircle != null)
			{
				selectableObject.selectionCircle.Release();
				selectableObject.selectionCircle = null;
			}
		}

		public bool IsWithinSelectionBounds(SelectableUnitComp selectedUnit)
		{
			if (!isSelecting)
				return false;

			var viewportBounds = Utils.GetViewportBounds(cam, mousePositionStart, Input.mousePosition);
			return viewportBounds.Contains(cam.WorldToViewportPoint(selectedUnit.Owner.GetComponent<PositionComp>().position));
		}

		public void SetState(CursorSelectStates css)
		{
			selectState = css;
			Cursor.visible = true;
			if (css == CursorSelectStates.Normal)
				Cursor.SetCursor(cursorNormalTex, Vector2.zero, CursorMode.Auto);
			else if (css == CursorSelectStates.PositionPick)
				Cursor.SetCursor(cursorPositionTex, Vector2.zero, CursorMode.Auto);
			else if (css == CursorSelectStates.TargetPick)
				Cursor.SetCursor(cursorTargetTex, new Vector2(32, 32), CursorMode.Auto); //32,32 to hit middle of cursor
			else if (css == CursorSelectStates.Construction)
				Cursor.visible = false;
		}

		void OnGUI()
		{
			//GO AWAY buggy box
			if (UICamera.hoveredObject != null && UICamera.hoveredObject != cam.gameObject && UICamera.hoveredObject != uiRoot)
			{
				// don't handle events via OnGUI while hovering NGUI items
				hoveringUI = true;
				//Debug.Log($"hovering true: {UICamera.hoveredObject.name}");
				return;
			}
			else
			{
				hoveringUI = false;
			}
			//if (isSelecting && selectState == CursorSelectStates.Normal)
			//{
			//	// Create a rect from both mouse positions
			//	var rect = Utils.GetScreenRect(mousePositionStart, Input.mousePosition);
			//	Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
			//	Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
			//}
		}

		public event Action OnSelectionChanged;

		public void TriggerSelectionChanged()
		{
			OnSelectionChanged?.Invoke();
		}
	}
}

public enum CursorSelectStates
{
	Normal,
	TargetPick,  //For attack/heal, gets unit selection
	PositionPick, //For movement, gets terrain position
	Construction
}