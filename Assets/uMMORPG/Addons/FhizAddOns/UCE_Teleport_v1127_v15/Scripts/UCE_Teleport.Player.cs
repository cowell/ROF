using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Linq;

// =======================================================================================
// PLAYER
// =======================================================================================
public partial class Player {
	
	[HideInInspector] public UCE_Area_Teleport UCE_myTeleport;
	
	// -----------------------------------------------------------------------------------
	// Cmd_UCE_Teleport
	// -----------------------------------------------------------------------------------
    [Command(channel=Channels.DefaultUnreliable)] // unimportant => unreliable
    public void Cmd_UCE_Teleport() {
        if (UCE_ValidateTeleport() ) {
        	gold 	-= UCE_myTeleport.GoldCost;
        	coins 	-= UCE_myTeleport.CoinCost;
        	if (UCE_myTeleport.requiredItem && UCE_myTeleport.destroyItemOnTeleport) InventoryRemove(new Item(UCE_myTeleport.requiredItem), UCE_myTeleport.itemAmount);
            agent.Warp(UCE_myTeleport.teleportTo.position);
        }
    }
    
	// -----------------------------------------------------------------------------------
	// UCE_ValidateTeleport
	// -----------------------------------------------------------------------------------
	private bool UCE_ValidateTeleport() {
		var valid		= false;
		if (UCE_myTeleport) {
			
			valid = (UCE_myTeleport.teleportTo != null) ? true : false;
			valid = (health > 0) ? true : false;
			valid = (UCE_myTeleport.MinLevel == 0 || level >= UCE_myTeleport.MinLevel) ? true : false;
			valid = (UCE_myTeleport.MaxLevel == 0 || level <= UCE_myTeleport.MaxLevel) ? true : false;
			valid = (UCE_myTeleport.GoldCost == 0 || gold >= UCE_myTeleport.GoldCost) ? true : false;
			valid = (UCE_myTeleport.CoinCost == 0 || coins >= UCE_myTeleport.CoinCost) ? true : false;
			valid = (UCE_myTeleport.requiredClass == null || classIcon == UCE_myTeleport.requiredClass.GetComponent<Player>().classIcon) ? true : false;
			valid = (UCE_myTeleport.requiredQuest == null || HasCompletedQuest(UCE_myTeleport.requiredQuest.name)) ? true : false;
			valid = (!UCE_myTeleport.requiresGuild || InGuild()) ? true : false;
			valid = (UCE_myTeleport.requiredItem == null || InventoryCount(new Item(UCE_myTeleport.requiredItem)) >= UCE_myTeleport.itemAmount) ? true : false;
			valid = (UCE_Teleport_checkHasSkill(UCE_myTeleport.requiredSkill, UCE_myTeleport.requiredSkillLevel)) ? true : false;
		
		}
		return valid;
	} 

	// -----------------------------------------------------------------------------------
	// UCE_Teleport_checkHasSkill
	// -----------------------------------------------------------------------------------
	public bool UCE_Teleport_checkHasSkill(ScriptableSkill skill, int level) {
		if (skill == null || level <= 0) return true;
		return skills.Any(s => s.name == skill.name && s.level >= level );
	}
	
	// -----------------------------------------------------------------------------------
	// UCE_AutoTeleport
	// -----------------------------------------------------------------------------------
	[Client]
	public void UCE_AutoTeleport() {
		Cmd_UCE_Teleport();
	}
	
	// -----------------------------------------------------------------------------------
	
}

// =======================================================================================