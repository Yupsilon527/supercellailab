using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public GameObject endSection,hpbarfull,hpbarhurt;
	int currentScore=0;
	static GameManager myslf;
	public bool gameOver=false;
	int enemyCount;
	void Awake(){
		myslf = this;

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver && Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
		if (PlayerBehavior.instance != null)
		{
            hpbarhurt?.gameObject?.SetActive(PlayerBehavior.instance.damaged);
			hpbarfull?.gameObject?.SetActive(!PlayerBehavior.instance.damaged);
        }
	}
	public static void AddScore(int pointsAdded){
		myslf.currentScore += pointsAdded;
	}
	public static void RegisterPlayerDeath(){
		myslf.gameOver = true;
	}
	public static void SelectWeapon(PlayerWeaponType weaponType){
		switch (weaponType) {
			case PlayerWeaponType.KNIFE:
			break;
			case PlayerWeaponType.PISTOL:
			break;
		}

	}
	public static void AddToEnemyCount(){
		myslf.enemyCount++;
	}
	public static void RemoveEnemy(){
		myslf.enemyCount--;
		if (myslf.enemyCount <= 0) {
			myslf.endSection.SetActive(true);
		}

	}
}

