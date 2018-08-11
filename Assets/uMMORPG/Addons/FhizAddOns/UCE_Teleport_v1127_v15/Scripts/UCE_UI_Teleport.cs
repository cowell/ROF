using UnityEngine;
using UnityEngine.UI;

	// ===================================================================================
	// TELEPORT UI
	// ===================================================================================
	public partial class UCE_UI_Teleport : MonoBehaviour {
	
		public GameObject panel;
		public Button teleportButton;
		public Transform content;
		public ScrollRect scrollRect;
		public GameObject textPrefab;
   	
   		private const string MSG_HEADING		= "Teleportation available:";
   		private const string MSG_TELEPORT		= "Teleport to: ";
		private const string MSG_MINLEVEL 		= " - Required minimum Level: ";
		private const string MSG_MAXLEVEL 		= " - Required maximum level: ";
		private const string MSG_GOLDCOST 		= " - Gold cost per use: ";
		private const string MSG_COINCOST 		= " - Coins cost per use: ";
		private const string MSG_REQCLASS 		= " - Required class to use: ";

		private const string MSG_REQQUEST 		= " - Complete quest to use: ";
		private const string MSG_REQGUILD		= " - Requires guild membership!";
		private const string MSG_REQITEM		= " - Required item: ";
		private const string MSG_DESTROY		= "[Destroyed on use]";
		private const string MSG_REQSKILL		= " - Required Skill: ";

		private bool okTeleport 				= false;
		private bool okMinLevel 				= false;
		private bool okMaxLevel 				= false;
		private bool okGoldCost 				= false;
		private bool okCoinCost 				= false;
		private bool okRequiredClass 			= false;
		private bool okRequiresGuild 			= false;
		private bool okRequiresQuest 			= false;
		private bool okRequiredItem 			= false;
		private bool okRequiredSkill			= false;
 
    	// -----------------------------------------------------------------------------------
		// validateTeleport
		// -----------------------------------------------------------------------------------
		private void validateSimpleTeleport() {
			var player = Utils.ClientLocalPlayer();
			if (!player) return;
			
			var teleport = player.UCE_myTeleport;
			
			if (teleport) {
							
				okTeleport 		= (teleport.teleportTo != null) ? true : false;
				okMinLevel 		= (teleport.MinLevel == 0 || player.level >= teleport.MinLevel) ? true : false;
				okMaxLevel 		= (teleport.MaxLevel == 0 || player.level <= teleport.MaxLevel) ? true : false;
				okGoldCost 		= (teleport.GoldCost == 0 || player.gold >= teleport.GoldCost) ? true : false;
				okCoinCost 		= (teleport.CoinCost == 0 || player.coins >= teleport.CoinCost) ? true : false;
				okRequiredClass = (teleport.requiredClass == null || player.classIcon == teleport.requiredClass.GetComponent<Player>().classIcon) ? true : false;
				okRequiresQuest	= (teleport.requiredQuest == null || player.HasCompletedQuest(teleport.requiredQuest.name)) ? true : false;
				okRequiresGuild = (!teleport.requiresGuild || player.InGuild()) ? true : false;
				okRequiredItem	= (teleport.requiredItem == null || player.InventoryCount(new Item(teleport.requiredItem)) >= teleport.itemAmount) ? true : false;
				okRequiredSkill = (player.UCE_Teleport_checkHasSkill(teleport.requiredSkill, teleport.requiredSkillLevel)) ? true : false;
				
				okTeleport = okMinLevel && okMaxLevel && okGoldCost && okCoinCost && okRequiredClass && okRequiresQuest && okRequiresGuild && okRequiredItem && okRequiredSkill;
			
			}
		}
		
	   	// -----------------------------------------------------------------------------------
		// updateTextbox
		// -----------------------------------------------------------------------------------
		private void updateTextbox() {	
			var player = Utils.ClientLocalPlayer();
			if (!player) return;
			
			var teleport = player.UCE_myTeleport;
			
			if (teleport) {
				AddMessage(MSG_HEADING, Color.white);
				if (teleport.MinLevel != 0) 			AddMessage(MSG_MINLEVEL + teleport.MinLevel.ToString(), okMinLevel ? Color.green : Color.red);
				if (teleport.MaxLevel != 0) 			AddMessage(MSG_MAXLEVEL + teleport.MaxLevel.ToString(), okMaxLevel ? Color.green : Color.red);
				if (teleport.GoldCost != 0) 			AddMessage(MSG_GOLDCOST + teleport.GoldCost.ToString(), okGoldCost ? Color.green : Color.red);
				if (teleport.CoinCost != 0) 			AddMessage(MSG_COINCOST + teleport.CoinCost.ToString(), okCoinCost ? Color.green : Color.red);
				if (teleport.requiredClass != null) 	AddMessage(MSG_REQCLASS + teleport.requiredClass.GetComponent<Player>().name, okRequiredClass ? Color.green : Color.red);
				if (teleport.requiredQuest != null)		AddMessage(MSG_REQQUEST + teleport.requiredQuest.name, okRequiresQuest ? Color.green : Color.red);
				if (teleport.requiresGuild)				AddMessage(MSG_REQGUILD, okRequiresGuild ? Color.green : Color.red);
				var itemDestroyed = teleport.destroyItemOnTeleport ? MSG_DESTROY : "";
				if (teleport.requiredItem != null)		AddMessage(MSG_REQITEM + teleport.requiredItem.name + " x"+ teleport.itemAmount.ToString() + itemDestroyed, okRequiredItem ? Color.green : Color.red);

				if (teleport.requiredSkill != null)		AddMessage(MSG_REQSKILL + teleport.requiredSkill.name + " [L"+ teleport.requiredSkillLevel.ToString(), okRequiredSkill ? Color.green : Color.red);
			
			}
			
		}
		
		// -----------------------------------------------------------------------------------
		// Show
		// -----------------------------------------------------------------------------------
		public void Show() {
			var player = Utils.ClientLocalPlayer();
			if (!player) return;
   			
			for (int i = 0; i < content.childCount; ++i) {
				Destroy(content.GetChild(i).gameObject);
			}
			
			var teleport = player.UCE_myTeleport;
			
			if (teleport) {
			
				validateSimpleTeleport();
				if (okTeleport) teleportButton.GetComponentInChildren<Text>().text = MSG_TELEPORT + teleport.teleportTo.name;
				teleportButton.interactable = okTeleport;
		
				teleportButton.onClick.SetListener(() => { 
					player.Cmd_UCE_Teleport();
				});
			
			}
			
			updateTextbox();
			panel.SetActive(true);
		
		}

		// -----------------------------------------------------------------------------------
		// Hide
		// -----------------------------------------------------------------------------------
		public void Hide() {
			panel.SetActive(false);
		}

		// -----------------------------------------------------------------------------------
		// Update
		// -----------------------------------------------------------------------------------
		private void Update() {
			var player = Utils.ClientLocalPlayer();
			if (!player) return;
			if (player.UCE_myTeleport == null) {
				Hide();
			} else {
				validateSimpleTeleport();
			}	
		}
	
		// -----------------------------------------------------------------------------------
		// AutoScroll
		// -----------------------------------------------------------------------------------
		private void AutoScroll() {
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0;
		}
	
		// -----------------------------------------------------------------------------------
		// AddMessage
		// -----------------------------------------------------------------------------------
		private void AddMessage(string msg, Color color) {
			var go = Instantiate(textPrefab);
			go.transform.SetParent(content.transform, false);
			go.GetComponent<Text>().text = msg;
			go.GetComponent<Text>().color = color;
			AutoScroll();
		}
	}

// =======================================================================================
