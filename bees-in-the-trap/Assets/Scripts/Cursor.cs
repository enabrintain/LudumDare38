﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour {

	public enum Direction
	{
		LEFT, UPLEFT, UPRIGHT, RIGHT, BACK
	}

	private BoardGeneration b;
	public CameraManager camera;
	public Text itemTitleText;
	public Text itemDescriptionText;

	private string moves;
	private float backPressTimestamp;
	private Hex selectedHex;

	public bool inhibitKeys = false;

	void Start () {
		b = GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoardGeneration>();

		moves = "";
		backPressTimestamp = 0f;
		selectedHex = null;

		//im' sorry these are here, they're just to work around me not wanting to edit the scene while jake edits it
		itemTitleText.text = "Throne Room"; 
		itemDescriptionText.text = "God save the queen.";
	}

	void Update () {

		if (inhibitKeys)
			return;

		// we usually would let people configure the keys,
		// but not for this hex-oriented control scheme.

		// also, right now if you release multiple keys
		// simultaneously (same frame), it'll prioritize 
		// clockwise starting left, and only accept the 
		// first of those keys as valid input.
		if (Input.GetKeyUp ("v"))
			Move (Direction.LEFT);
		else if (Input.GetKeyUp ("g"))
			Move (Direction.UPLEFT);
		else if (Input.GetKeyUp ("h"))
			Move (Direction.UPRIGHT);
		else if (Input.GetKeyUp ("n"))
			Move (Direction.RIGHT);
		else if (backPressTimestamp > 0f) {
			if (Time.time - backPressTimestamp > 0.5f) {
				backPressTimestamp = 0f;
				moves = "";
				Move (Direction.LEFT); // whatever, direction doesn't matter
				Move (Direction.BACK); // just resets everything
			} else if (Input.GetButtonUp ("Back")) {
				backPressTimestamp = 0f;
				Move (Direction.BACK);
			}
		}


		if (Input.GetButtonDown ("Back"))
			backPressTimestamp = Time.time;
	}

	void Move (Direction d) {
		if (selectedHex == null)
			selectedHex = b.GetHexAtCursorPosition (moves);

		if (b.isLegalMovement (moves, d)) {
			if (d == Direction.BACK) {
				moves = moves.Substring (0, moves.Length - 1);
			} else {
				moves += ConvertDirectionToChar (d);
			}
		}
		transform.position = b.SetCursorPosition (moves);
		selectedHex.transform.GetChild (0).gameObject.GetComponent<Animator> ().SetBool ("IsFocused", false);
		
		camera.scootTo (this.transform.position);

		selectedHex = b.GetHexAtCursorPosition (moves);
		selectedHex.transform.GetChild (0).gameObject.GetComponent<Animator> ().SetBool ("IsFocused", true);

		LoadShitFromSelectedHex ();

	}

	public void LoadShitFromSelectedHex() {
		itemTitleText.text = selectedHex.upgradeTitle;
		if (selectedHex.isActive) {
			itemDescriptionText.text = selectedHex.upgradeDescription;
		} else {
			itemDescriptionText.text = "?";
		}
	}

	public void FixCamera() {
		//this is stupid
		camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
	}

	char ConvertDirectionToChar (Direction d) {
		if (d == Direction.LEFT)
			return 'v';
		if (d == Direction.UPLEFT)
			return 'g';
		if (d == Direction.UPRIGHT)
			return 'h';
		if (d == Direction.RIGHT)
			return 'n';
		throw new UnityException ("Given direction has no associated character mapping. Direction: " + d);
	}

	public Hex GetSelectedHex() {
		return selectedHex;
	}
}
