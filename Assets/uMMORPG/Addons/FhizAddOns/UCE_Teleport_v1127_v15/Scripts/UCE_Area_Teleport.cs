using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

	// ===================================================================================
	// TELEPORT AREA
	// ===================================================================================
	[RequireComponent(typeof(BoxCollider))]
	public class UCE_Area_Teleport : NetworkBehaviour {
	
		[Tooltip("One click deactivation")]
		public bool isActive	= true;
		[Tooltip("Same scene target with transform component (required!)")]
		public Transform teleportTo;
		[Tooltip("Triggers teleportation automatically if requirements are met")]
		public bool autoTrigger	= false;
		[Tooltip("Min player level to access this teleport")]
		public int MinLevel = 1;
		[Tooltip("Max player level to access this teleport")]
		public int MaxLevel = 10;
		[Tooltip("Gold cost to access this teleport")]
		public long GoldCost = 0;
		[Tooltip("Coins cost to access this teleport")]
		public long CoinCost = 0;
		[Tooltip("Required class to access this teleport (Player Prefab)")]
		public GameObject requiredClass;
		
		[Tooltip("Player must be in a guild (any) ?")]
		public bool requiresGuild = false;
		[Tooltip("This quest must be completed first")]
		public ScriptableQuest requiredQuest;
		[Tooltip("This item must be in the players inventory")]
		public ScriptableItem requiredItem;
		[Tooltip("Destroy the item when using the teleporter?")]
		public bool destroyItemOnTeleport = false;
		[Tooltip("How many copies of the items will be destroyed?")]
		public int itemAmount = 1;
		
		[Tooltip("Required skill to teleport")]
		public ScriptableSkill requiredSkill;
		[Tooltip("Required minimum skill level to teleport")]
		public int requiredSkillLevel;
				
		// -------------------------------------------------------------------------------
		// OnTriggerEnter
		// -------------------------------------------------------------------------------
		void OnTriggerEnter(Collider co) {
			var e = co.GetComponentInParent<Player>();
			if (e && e.health > 0 && isActive && teleportTo != null) {
				e.UCE_myTeleport = this;
				if (!autoTrigger) {
					FindObjectOfType<UCE_UI_Teleport>().Show();
				} else {
					e.UCE_AutoTeleport();
				}
			}
		}
		
		// -------------------------------------------------------------------------------
		// OnTriggerExit
		// -------------------------------------------------------------------------------
		void OnTriggerExit(Collider co) {
			var e = co.GetComponentInParent<Player>();
			if (e && isActive && teleportTo != null) {
				e.UCE_myTeleport = null;
				if (!autoTrigger) {
					FindObjectOfType<UCE_UI_Teleport>().Hide();
				}
			}
		}

	}

// =======================================================================================