using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;

using Random = UnityEngine.Random;

public class GameController : MonoBehaviourSingleton<GameController> {

    public Game Game;
    public List<Desk> AllDesks;

    public int StartLevel = 0;
 
    public GameObject PopupPrefab;

	public float Interval = 0.1f;
    public float CurrentTimer = 0f;

    public float MaximumFixTime = 3f;

    public List<Level> Levels;

    public float WidthOfBar = 248f;
    public RectTransform ProgressBar;
    public RectTransform FixBar;
    public RectTransform TimeBar;

    public float TimeLeft = 0f;

    public float Delay = 1.5f;

    public CanvasGroup LevelComplete;
    public CanvasGroup LevelFailed;
    public CanvasGroup TitleScreen;

    public Desk SelectedDesk;

    public Level CurrentLevel;

    public TextMeshProUGUI LevelText;

    public int AdjustTimeBy = 0;

    public class Level {

        public int seconds;
        public int target;
        public int desks;
        public int chance;

    }


    // Start is called before the first frame update
    void Start()
    {
        CreateLevels ();
        ResetGame ();



 
    }

    public void LevelCompleted () {

        LevelComplete.alpha = 1;
        Game.Active = false;
        NextLevel ();
        SoundController.Instance.StopSounds ();
        SoundController.Instance.PlaySound ("Complete");
        Delay = 2f;


    }


    public void LevelFail () {

        LevelFailed.alpha = 1;
        Game.Active = false;
        SoundController.Instance.StopSounds ();
        SoundController.Instance.PlaySound ("Failed");
        Delay = 2f;


    }

    void CreateLevels () {

        Levels = new List<Level> ();

        Levels.Add (new Level { seconds = 20, target = 3000, desks = 3, chance = 12500 });
        Levels.Add (new Level { seconds = 20, target = 6500, desks = 6, chance = 15000 });
        Levels.Add (new Level { seconds = 25, target = 12000, desks = 9, chance = 25000});
        Levels.Add (new Level { seconds = 30, target = 18000, desks = 12, chance = 22500 });
        Levels.Add (new Level { seconds = 40, target = 24000, desks = 15, chance = 21500 });
        Levels.Add (new Level { seconds = 45, target = 28000, desks = 18, chance = 20000 });
        Levels.Add (new Level { seconds = 50, target = 32000, desks = 21, chance = 24000 });
        Levels.Add (new Level { seconds = 55, target = 40000, desks = 24, chance = 22500 });
        Levels.Add (new Level { seconds = 25, target = 20000, desks = 25, chance = 25000 });
        Levels.Add (new Level { seconds = 30, target = 25000, desks = 26, chance = 23000 });
        Levels.Add (new Level { seconds = 35, target = 30000, desks = 27, chance = 22000 });
		Levels.Add (new Level { seconds = 40, target = 35000, desks = 28, chance = 22000 });
        Levels.Add (new Level { seconds = 42, target = 37000, desks = 28, chance = 22500 });
        Levels.Add (new Level { seconds = 44, target = 39000, desks = 28, chance = 23000 });
        Levels.Add (new Level { seconds = 46, target = 40000, desks = 28, chance = 23500 });
        Levels.Add (new Level { seconds = 48, target = 41000, desks = 28, chance = 24000 });
        Levels.Add (new Level { seconds = 50, target = 42000, desks = 28, chance = 24500 });
        Levels.Add (new Level { seconds = 52, target = 43000, desks = 28, chance = 25000 });
        Levels.Add (new Level { seconds = 54, target = 44000, desks = 28, chance = 25500 });
        Levels.Add (new Level { seconds = 56, target = 45000, desks = 28, chance = 26000 });



    }

    // Update is called once per frame
    void Update()
    {



        if (Game != null && Game.Active) {

            CurrentTimer += Time.deltaTime;

            if (CurrentTimer >= Interval) {

                Process ();
            }


            TimeLeft -= Time.deltaTime;



            if (TimeLeft <= 0f) {
                TimeLeft = 0f;
                Game.Active = false;
                LevelFail ();
            }

            TimeBar.sizeDelta = new Vector2 (WidthOfBar / (CurrentLevel.seconds - AdjustTimeBy )
				* TimeLeft, TimeBar.sizeDelta.y);


            if (Input.GetMouseButton (0)) {

                RaycastHit [] hits;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                hits = Physics.RaycastAll (ray);

                SelectedDesk = null;

                foreach (RaycastHit hit in hits) {

                    if (hit.transform.gameObject.tag == "Monitor") {


                        SelectedDesk = hit.transform.GetComponentInParent<Desk> ();
                        break;
                    }


                }

                if (SelectedDesk != null) {

                    SelectedDesk.Fix ();

                    if (SelectedDesk.ComputerStatus != ComputerStatus.Normal) {

                        FixBar.parent.parent.gameObject.SetActive (true);
                        FixBar.sizeDelta = new Vector2 (WidthOfBar / GameController.Instance.MaximumFixTime * SelectedDesk.TimeToFix, FixBar.sizeDelta.y);
                    } else {
                        FixBar.parent.parent.gameObject.SetActive (false);
                    }

                } else {

                    FixBar.parent.parent.gameObject.SetActive (false);
                }



            } else {
                FixBar.parent.parent.gameObject.SetActive (false);
            }


        } else {

            Delay -= Time.deltaTime;

            if (Delay <= 0f && Input.GetMouseButtonDown (0)) {
                if (Game == null) {
                    ResetGame();
                } else {
                    NewLevel ();
                }
            }



        }
    }


	void Process() {

        CurrentTimer = -(CurrentTimer - Interval);

        foreach (Desk desk in Game.Desks) {
            desk.ProcessDay ();
        }

        ProgressBar.sizeDelta = new Vector2 (WidthOfBar * (float)Game.Productivity / (float)CurrentLevel.target, ProgressBar.sizeDelta.y);


    }


    void ResetGame () {

        Game = new Game ();

        Game.Level = StartLevel;

        NewLevel ();
        

    }


    void NewLevel () {

        foreach (Desk desk in AllDesks) {

            desk.gameObject.SetActive (false);
            desk.ComputerStatus = ComputerStatus.Normal;

        }



        LevelComplete.alpha = 0;
        LevelFailed.alpha = 0;
        TitleScreen.alpha = 0;

        Game.Desks.Clear ();


        CurrentLevel = Levels [Game.Level];

        for (int i = 0; i < Levels[Game.Level].desks; i++) {
            NewDesk ();
        }

        Game.Active = true;
        Game.Productivity = 0;
      
        TimeLeft = CurrentLevel.seconds - AdjustTimeBy;

   
        SoundController.Instance.PlaySound ("Loop", 0, 1f);

        LevelText.text = "LEVEL " + (Game.Level + 1).ToString ();

    }


	void NextLevel() {

            if (Game.Level <= Levels.Count - 1) {
                Game.Level++;
           
            }


    }




    public void ShowPopup (Desk desk, string text) {

        GameObject go = SimplePool.Spawn (PopupPrefab, Vector3.zero, Quaternion.identity);

        go.transform.SetParent (null);

        if (desk.transform.parent.eulerAngles.y > 170f) {
            go.transform.localScale = new Vector3 (1f, 1f, 1f);
        } else {
            go.transform.localScale = new Vector3 (-1f, 1f, 1f);
        }

        go.transform.SetParent (desk.transform);

        go.transform.localPosition = Vector3.zero;
        //go.transform.sc
        go.transform.DOLocalMoveY (35f, 2f);

    

        //go.transform.GetComponentInChildren<TextMeshProUGUI> ().text = text;

        go.transform.GetComponent<CanvasGroup> ().alpha = 0;

        go.transform.GetComponent<CanvasGroup> ().DOFade (1f, 1f).OnComplete (() => FadeOut (go));
       


    }

    void FadeOut (GameObject go) {
        go.transform.GetComponent<CanvasGroup> ().DOFade (0f, 1f).OnComplete (() => HidePopup (go));
    }

    void HidePopup (GameObject go) {

        SimplePool.Despawn (go);

    }


    public void SpreadInfection () {

        Desk newDesk = null;

        int attempts = 0;

        while (newDesk == null && attempts < 100) {

            newDesk = Game.Desks [Random.Range (0, Game.Desks.Count)];

            if (newDesk.ComputerStatus == ComputerStatus.Normal || newDesk.ComputerStatus == ComputerStatus.Obsolete) {
                newDesk.ComputerStatus = ComputerStatus.Infected;
                newDesk.TimeToFix = 1f;
                newDesk.ChangeScreen ();
            } else {
                newDesk = null;
            }

            attempts++;
        }


    }

    void NewDesk () {

        Desk newDesk = null;

        while (newDesk == null) {

            newDesk = AllDesks [Random.Range (0, AllDesks.Count)];

            if (Game.Desks.Contains (newDesk)) {
                newDesk = null;
                continue;
            }

            Game.Desks.Add (newDesk);
            newDesk.gameObject.SetActive (true);
            newDesk.InitialiseDesk ();
        }

    }


   
}
