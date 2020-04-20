using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComputerStatus { Normal, BlueScreen, Obsolete, Infected}

public class Desk : MonoBehaviour
{

	
	public ComputerStatus ComputerStatus;

	public MeshRenderer Monitor;

	public Game Game;

	public float chanceOfBlueScreen = 100f;
	public float chanceOfInfection = 10f;
	public float totalChance = 20000f;
	public float chanceOfObsoletion = 50f;

	public Material BlueScreen;
	public Material NormalScreen;
	public Material InfectedScreen;
	public Material ObsoleteScreen;

	public Dictionary<ComputerStatus, Material> ScreenMaterials;

	public float TimeToFix = 0f;


	public void Awake () {

		ScreenMaterials = new Dictionary<ComputerStatus, Material> ();

		ScreenMaterials.Add (ComputerStatus.BlueScreen, BlueScreen);
		ScreenMaterials.Add (ComputerStatus.Normal, NormalScreen);
		ScreenMaterials.Add (ComputerStatus.Infected, InfectedScreen);
		ScreenMaterials.Add (ComputerStatus.Obsolete, ObsoleteScreen);


	}


	public void InitialiseDesk () {

		Game = GameController.Instance.Game;

		ComputerStatus = ComputerStatus.Normal;

		ChangeScreen ();

	}


	public void ChangeScreen () {

		//Debug.Log (transform.parent.name + " " + name);

		Monitor.sharedMaterial = ScreenMaterials [ComputerStatus];


	}


	void CheckWhetherToBlueScreen () {

		if (ComputerStatus != ComputerStatus.Normal) {
			return;
		}

		float chanceOfBlueScreenNow = chanceOfBlueScreen * 1f + ((float)Game.Level / 1f);

		float chance = Random.Range (0, GameController.Instance.CurrentLevel.chance);


		if (chance < chanceOfBlueScreenNow) {
			ComputerStatus = ComputerStatus.BlueScreen;
			ChangeScreen ();
			TimeToFix = 0.5f;
			SoundController.Instance.PlaySound ("Crash", 0, 1f);
		}

	}


	public void Fix () {

		if (TimeToFix > 0f && ComputerStatus != ComputerStatus.Normal) {

			TimeToFix -= Time.deltaTime;

			if (TimeToFix <= 0f) {
				TimeToFix = 0f;
				ComputerStatus = ComputerStatus.Normal;
				ChangeScreen ();
				SoundController.Instance.PlaySound ("Restart", 0, 1f);
			}
		}


	}

	void CheckWhetherToInfect () {

		if (ComputerStatus != ComputerStatus.Normal) {
			return;
		}

		float chanceNow = chanceOfInfection * 1f + ((float)Game.Level / 1f);

		float chance = Random.Range (0, GameController.Instance.CurrentLevel.chance);

		if (chance < chanceNow) {
			ComputerStatus = ComputerStatus.Infected;
			ChangeScreen ();
			TimeToFix = 1f;
			SoundController.Instance.PlaySound ("Virus", 0, 1f);
		}

	}


	void CheckWhetherObsolete () {

		if (ComputerStatus != ComputerStatus.Normal) {
			return;
		}

		float chanceNow =  chanceOfObsoletion * 1f + ((float)Game.Level / 1f);

		float chance = Random.Range (0, GameController.Instance.CurrentLevel.chance);


		if (chance < chanceNow) {

			ComputerStatus = ComputerStatus.Obsolete;
			ChangeScreen ();
			TimeToFix = 0.75f;
			SoundController.Instance.PlaySound ("Obsolete", 0, 1f);
		}

	}

	public void ProcessDay () {

		if (Game != null && Game.Active) {

			int production = 4;

			CheckWhetherToBlueScreen ();
			CheckWhetherToInfect ();
			CheckWhetherObsolete ();


			if (ComputerStatus == ComputerStatus.BlueScreen) {
				production = 0;
				TimeToFix += Time.deltaTime / 35f;
			}

			if (ComputerStatus == ComputerStatus.Obsolete) {
				production = 1;
				TimeToFix += Time.deltaTime / 25f;
			}

			if (ComputerStatus == ComputerStatus.Infected) {
				production = -2;
				TimeToFix += Time.deltaTime / 20f;

				if (Random.Range (0, 400) == 0) {
					GameController.Instance.SpreadInfection ();
				}
			}

			TimeToFix = Mathf.Min (GameController.Instance.MaximumFixTime, TimeToFix);

			Game.Productivity += production;

			if (Game.Productivity >= GameController.Instance.CurrentLevel.target) {
				GameController.Instance.LevelCompleted ();
			}

		}


	}

	
}
