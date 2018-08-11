////////////////////////
// View Player Behind //
//     Object By:     //
//     RCTesting      //
//       A.K.A.       //
//    (KD, DirtyD)    //
////////////////////////

//////
//View Player Behind Object - Paid version 1.0
//////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public partial class behindObjectPaid : MonoBehaviour {
Player player;
	[Header("Layer/GameObject To Change Opacity %")]
	[Header("Set opacity from 0 to 100")]
    public GameObject OpacityObject;
	[Range(0.0f,100.0f)]
	public int OpacityObjectSet = 75;
	[Header("Player Opacity %")]
	[Range(0.0f,100.0f)]
	public int OpacityPlayerSet = 50;
	
	private BoxCollider2D collider;
	 public void OnTriggerEnter2D(Collider2D other){
		  player = Utils.ClientLocalPlayer();
		  if (!player || player.name != other.name) return;
		 try{
			 float OpacityObjectSetPerc = OpacityObjectSet / 100f;
			 float OpacityPlayerSetPerc = OpacityPlayerSet / 100f;
			 OpacityObject.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, OpacityObjectSetPerc);
			 
			 player.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, OpacityPlayerSetPerc);
		 }catch{
			Debug.Log("Something went wrong! Did you set the object to change opacity for?");
		 }
	}
	
	public void OnTriggerExit2D(Collider2D other){
		player = Utils.ClientLocalPlayer();
		if (!player || player.name != other.name) return;
		try{
			OpacityObject.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			player.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			Debug.Log(player.name);
		}catch{
			Debug.Log("Something went wrong! Did you set the object to change opacity for?");
		}
	}
}
