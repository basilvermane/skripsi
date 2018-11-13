using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public enum ShootMode {
	IDLE,
	AIM,
	POWER,
	Length
};

public class GameplayManager : MonoBehaviour {

	//singleton
	private static GameplayManager _instance;
	public static GameplayManager Instance {
		get {
			return _instance;
		}
	}

	private bool inMenu = true;

	private void OnEnable () {
		SceneManager.sceneLoaded += SceneLoadCheck;
	}

	private void SceneLoadCheck (Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals ("main") && PlayerPrefs.HasKey ("level")) {
			StartNewStage (PlayerPrefs.GetInt ("level"));

			inMenu = false;
		} else if (scene.name.Equals("menu") && LoadingManager.Instance != null) {
			LoadingManager.Instance.SetLoadingWall (false);
			LoadingManager.Instance.SetWinWall (false);

			inMenu = true;
			ResetAll ();
		}
	}

	private void ResetAll () {
		timeFreeze = false;
		physicsVision = false;
		shootMode = ShootMode.IDLE;
		pvFrameCount = 0;
	}

	private void Awake () {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad (this);
			//debugging purposes only
			//LockCursor ();
		} else {
			Destroy (gameObject);
		}
	}

	private bool timeFreeze;
	public bool TimeFreeze {
		get {
			return timeFreeze;
		}
	}

	private ShootMode shootMode;
	public ShootMode ShootMode {
		get {
			return shootMode;
		}
	}

	private bool physicsVision;
	public bool PhysicsVision {
		get {
			return physicsVision;
		}
	}

	private bool isSandbox;

	public BallController stageBall;
	public Transform stageGoal;

	public GameObject ballPrefab;
	public GameObject goalPrefab;
	public PowerMeterController powerControl;

	public Text testText;

	private LevelParam currentLevel;
	private PerlinNoise perlin;

	private int shotCount;

	public float startingDist = 2.0f;

	private Vector3 ballToGoalDist;

	private CameraController player;

	private int pvFrameCount;
	public int pvFrameThreshold = 30;

	private int tfFrameCount;
	public int tfFrameThreshold = 30;

	public void SandboxChange (float grav, int mapLength, int mapWidth, int mapHeight, float mass) {
		if (!isSandbox) return;

		//persiapan loading
		LoadingManager.Instance.SetLoadingWall (true, Camera.main.transform.position, Camera.main.transform.rotation);
		timeFreeze = true;
		stageBall.TimeFreeze (true);

		StartCoroutine (SandboxRoutine (grav, mapLength, mapWidth, mapHeight, mass));
	}
	
	private IEnumerator SandboxRoutine (float grav, int mapLength, int mapWidth, int mapHeight, float mass) {
		//generate terrain baru
		perlin.GenerateNewNoise (mapLength, mapWidth, mapHeight, false);
		Physics.gravity = Vector3.down * grav;

		//tempatkan bola dan gol
		Vector3 ballPos, goalPos;
		Vector2 goalPosTemp;
		Vector2 ballPosTemp = new Vector2 (
			Random.Range (10.0f, mapLength - 10.0f),
			Random.Range (10.0f, mapWidth - 10.0f));
		do {
			goalPosTemp = new Vector2 (
				Random.Range (10.0f, mapLength - 10.0f),
				Random.Range (10.0f, mapWidth - 10.0f));
		} while ((ballPosTemp - goalPosTemp).magnitude < 1.0f /*!!!!*/);
		float ballY = perlin.GetCurrentHeightAt (ballPosTemp.x, ballPosTemp.y);
		float goalY = perlin.GetCurrentHeightAt (goalPosTemp.x, goalPosTemp.y);
		ballPos = new Vector3 (ballPosTemp.x, ballY + 10.0f, ballPosTemp.y);
		goalPos = new Vector3 (goalPosTemp.x, goalY + 10.0f, goalPosTemp.y);
		stageBall.SetPosition (ballPos);
		stageBall.SetMass (mass);
		stageGoal.position = goalPos;

		//tempatkan player
		Vector3 playerPos = stageBall.transform.position - player.getForward ();
		playerPos.y = perlin.GetCurrentHeightAt (playerPos.x, playerPos.z) + 10.0f;
		player.Teleport (playerPos);
		LoadingManager.Instance.SetLoadingWall (true, playerPos, player.transform.rotation);

		//selesai loading
		perlin.ShowTerrain ();
		stageBall.TimeFreeze (false);
		timeFreeze = false;
		LoadingManager.Instance.SetLoadingWall (false);

		yield return null;
	}

	public void StartNewStage (int level) {
		pvFrameCount = 0;
		tfFrameCount = 0;

		//nonaktifkan loading
		LoadingManager.Instance.SetLoadingWall (false);
		LoadingManager.Instance.SetWinWall (false);
		
		//jika level negatif maka sandbox mode
		if (level < 0) {
			isSandbox = true;
			print ("sandbox mode");
			level = LevelParameters.SandboxDefaultLevel;
		} else {
			isSandbox = false;
			print ("level " + level);
		}

		//ambil data per level
		currentLevel = LevelParameters.LevelParams[level];

		//generate terrain
		perlin = GameObject.FindGameObjectWithTag ("Terrain").GetComponent<PerlinNoise> ();
		perlin.GenerateNewNoise (currentLevel.HeightMapWidth, currentLevel.HeightMapHeight, currentLevel.MaxHeight, true);

		//cari player
		player = GameObject.Find ("Player").GetComponent<CameraController> ();

		shotCount = 0;
		timeFreeze = false;
	}

	public void SetBallGoal () {
		//tempatkan bola dan gol
		Vector3 ballPos, goalPos;
		Vector2 goalPosTemp;
		Vector2 ballPosTemp = new Vector2 (
			Random.Range (10.0f, currentLevel.HeightMapWidth - 10.0f),
			Random.Range (10.0f, currentLevel.HeightMapHeight - 10.0f));
		do {
			goalPosTemp = new Vector2 (
				Random.Range (10.0f, currentLevel.HeightMapWidth - 10.0f),
				Random.Range (10.0f, currentLevel.HeightMapHeight - 10.0f));
		} while ((ballPosTemp - goalPosTemp).magnitude < currentLevel.MinDistance);
		float ballY = perlin.GetCurrentHeightAt (ballPosTemp.x, ballPosTemp.y);
		print ("ball height " + ballY);
		float goalY = perlin.GetCurrentHeightAt (goalPosTemp.x, goalPosTemp.y);
		ballY += 5.0f;
		goalY += 5.0f;

		//cek apakah bola / gol dibawah terrain
		while (!Physics.Raycast (
			new Vector3 (ballPosTemp.x, ballY, ballPosTemp.y), Vector3.down)) {
			//terrain belum di bawah bola
			ballY += 2.0f;
		}
		while (!Physics.Raycast (
			new Vector3 (goalPosTemp.x, goalY, goalPosTemp.y), Vector3.down)) {
			//terrain belum di bawah bola
			goalY += 2.0f;
		}

		ballPos = new Vector3 (ballPosTemp.x, ballY, ballPosTemp.y);
		goalPos = new Vector3 (goalPosTemp.x, goalY, goalPosTemp.y);
		stageBall = Instantiate (ballPrefab, ballPos, Quaternion.identity).GetComponentInChildren<BallController> ();
		stageBall.SetPowerMeter (powerControl);
		stageBall.SetMass (currentLevel.BallMass);
		CanvasController.Instances[(int) CanvasType.GAME].SetTargetObject (stageBall.transform);
		CanvasController.Instances[(int) CanvasType.VELO_ARROW].SetTargetObject (stageBall.veloArrows.transform);
		CanvasController.Instances[(int) CanvasType.DISTANCE].SetTargetObject (stageBall.goalArrow.transform);
		stageGoal = Instantiate (goalPrefab, goalPos, Quaternion.identity).GetComponent<Transform> ();

		//tempatkan player
		CameraController player = GameObject.Find ("Player").GetComponent<CameraController> ();
		Vector3 playerPos = stageBall.transform.position - (player.getForward () * startingDist);
		//playerPos.y = perlin.GetCurrentHeightAt (playerPos.x, playerPos.z) + 10.0f;
		player.Teleport (playerPos);
		CalculationManager.Instance.SetCamera (player);
		CalculationManager.Instance.SetBall (stageBall);
		CalculationManager.Instance.SetGoal (stageGoal);
	}

	private void Update () {
		if (LoadingManager.Instance.IsLoading) {
			return;
		}

		if (SandBoxCanvasController.instance.IsActive) {
			return;
		}

		if (inMenu) {
			return;
		}

		//input
		//masuk sandbox canvas
		if ((Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.X)) && isSandbox && shootMode == ShootMode.IDLE) {
			if (!FormulaCanvasController.GameInstance.IsShown) {
				SandBoxCanvasController.instance.SetPlayer (player.transform);
				SandBoxCanvasController.instance.ShowCanvas (Physics.gravity.y, perlin.TLength, perlin.TWidth, perlin.THeight, stageBall.GetMass ());
			}
		}

		//time freeze
		if (Input.GetButtonUp ("TimeFreeze") || Input.GetKeyUp (KeyCode.JoystickButton2)) {
			if (tfFrameCount < tfFrameThreshold) {
				//single press - toggle time freeze
				timeFreeze = !timeFreeze;
				stageBall.TimeFreeze (timeFreeze);
			} else {
				//held, released - deactivate formula canvas
				FormulaCanvasController.GameInstance.SetVisible (false);
			}

			tfFrameCount = 0;
		} else if (Input.GetButton ("TimeFreeze") || Input.GetKey (KeyCode.JoystickButton2)) {
			if (tfFrameCount < tfFrameThreshold) {
				//held, start counting frames
				tfFrameCount++;
			} else {
				//held, show formula canvas
				FormulaCanvasController.GameInstance.SetPlayer (player.transform);
				FormulaCanvasController.GameInstance.SetVisible (true);
			}
		}

		//physics vision
		if (Input.GetButtonUp ("PhysicsVision") || Input.GetKeyUp (KeyCode.JoystickButton1)) {
			if (pvFrameCount < pvFrameThreshold) {
				//single press - toggle physics vision
				physicsVision = !physicsVision;
				stageBall.TogglePhysicsVision (physicsVision);
			} else {
				//held, released - deactivate calculation panel
				CalculationManager.Instance.Show (false);
			}

			pvFrameCount = 0;
		} else if (Input.GetButton ("PhysicsVision") || Input.GetKey (KeyCode.JoystickButton1)) {
			if (pvFrameCount < pvFrameThreshold) {
				//held, start counting frames
				pvFrameCount++;
			} else {
				//held, show calculation panel
				CalculationManager.Instance.Show (true);
			}
		}

		//ganti shoot mode
		if (Input.GetButtonDown ("ShootMode") || Input.GetKeyDown (KeyCode.JoystickButton0)) {
			if (shootMode == ShootMode.IDLE) {
				ChangeShootMode (ShootMode.AIM);
			} else {
				//tembak, kembali ke idle
				float force = powerControl.GetForce ();
				stageBall.Shoot (force);
				shotCount++;
				ChangeShootMode (ShootMode.IDLE);
			}
			stageBall.ChangeShootMode (shootMode);
			//aimTest1.SetVisible (shootMode);
		}

		if (Input.GetButtonDown ("Cancel") || Input.GetKeyDown (KeyCode.JoystickButton3)) {
			if (!FormulaCanvasController.GameInstance.IsShown) {
				if (shootMode == ShootMode.AIM || shootMode == ShootMode.POWER) {
					ChangeShootMode (ShootMode.IDLE);
				}
			}
		}
	}

	private void ChangeShootMode (ShootMode mode) {
		shootMode = mode;
		switch (shootMode) {
			case ShootMode.IDLE: {
					powerControl.Deactivate ();
					CanvasController.Instances[(int) CanvasType.GAME].SetVisible (false);
					stageBall.ChangeShootMode (ShootMode.IDLE);
				}
				break;
			case ShootMode.AIM:
			case ShootMode.POWER: {
					CanvasController.Instances[(int) CanvasType.GAME].SetVisible (true);
					powerControl.Activate ();
				} break;
			/*case ShootMode.AIM: {
					aimTest1.SetVisible (true);
					powerControl.Deactivate ();
					powerTest1.SetVisible (false);
				}
				break;
			case ShootMode.POWER: {
					aimTest1.SetVisible (false);
					powerTest1.SetVisible (true);
					powerControl.Activate ();
				}
				break;*/
			default: {
					//should never reach here
				}
				break;
		}
	}

	private void LateUpdate () {
		if (LoadingManager.Instance.IsLoading) {
			return;
		}

		if (stageBall != null && stageGoal != null)
			stageBall.SetGoalArrow (stageGoal.position);
	}

	//debugging purposes only
	/*private void LockCursor () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}*/

	public void FinishGame () {
		ResetAll ();

		//placeholder
		//ShowPlaceholderText ("Game finished!");

		//hide everything else
		perlin.HideTerrain ();
		stageBall.gameObject.SetActive (false);
		stageGoal.gameObject.SetActive (false);

		//show game stats and reset to menu
		StartCoroutine (LoadingManager.Instance.WinScreen (shotCount));
	}

	public void ShowPlaceholderText (string text) {
		testText.text = text;
	}

	public void FreezeBall (bool f) {
		stageBall.TimeFreeze (TimeFreeze, true, f);

		if (f) {
			perlin.HideTerrain ();
			stageBall.gameObject.SetActive (false);
			stageGoal.gameObject.SetActive (false);
		} else {
			perlin.ShowTerrain ();
			stageBall.gameObject.SetActive (true);
			stageGoal.gameObject.SetActive (true);
		}
	}
}
