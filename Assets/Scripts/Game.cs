//- Imports
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//- Class Game
public class Game : MonoBehaviour {

	//- Size of the grid
	public static int gridWidth = 10;
	public static int gridHeight = 20;

	//- 2d array for the play filed
	public static Transform[,]grid = new Transform[gridWidth, gridHeight];

	//- Base score points for completing a row
	public int scoreOneLine = 10;
	public int scoreTwoLine = 20;
	public int scoreThreeLine = 40;
	public int scoreFourLine = 80;

	//- object texts on the Canvas_HUD
	public Text hud_score;
	public Text hud_lines;
	public Text hud_level;

	//- Line clear sound
	public AudioClip lineClearAudio;

	//- help variables for score
	private int numberOfRowsCleared = 0;
	private int currentScore = 0;

	//- help variable for lines
	private int numberOfLines = 0;

	//- help variable for Level
	public int currentLevel = 1;
	private int levelInterval = 3; //- how often you level up
	private int whenToLevelUp = 3; //- initially to be = to levelInterval!!

	private AudioSource audioSource;

	//- To hold our preview and current to drop tetro
	private GameObject previewTetromino;
	private GameObject nextTetromino;

	//- Initial positions
	private Vector2 previewTetrominoPosition = new Vector2(16f, 13.5f);
	private Vector2 nextTetrominoPosition = new Vector2(5.0f, 20.0f);

	//- SpwnTetromino();
	//- assign audioSource
	void Start () {

		NewPreviewTetromino (); //- it will be discarted but we need it to run the first next
		SpawnTetromino ();
		audioSource = GetComponent<AudioSource>();

	}
	
	//- UpdateScore();
	void Update () {

		GiveClearLinesPoints ();
		LevelUp ();

	}

	//- Updates the score and updates the text
	public void UpdateScore (int byHowMany) {

		currentScore += byHowMany;
		hud_score.text = currentScore.ToString ();

	}
		
	//- Updates the number of lines completed and updates the text
	public void UpdateLines (int byHowMany) {

		numberOfLines += byHowMany;
		hud_lines.text = numberOfLines.ToString ();

	}

	//- Updates the level and updates the text
	public void UpdateLevel (int byHowMany){

		currentLevel += byHowMany;
		hud_level.text = currentLevel.ToString ();

	}

	//- Checks if to give points for line-clear
	public void GiveClearLinesPoints () {

		if(numberOfRowsCleared > 0){

			if (numberOfRowsCleared == 1) {

				UpdateScore(scoreOneLine*currentLevel);

			} else if (numberOfRowsCleared == 2) {

				UpdateScore(scoreTwoLine*currentLevel);

			} else if (numberOfRowsCleared == 3) {

				UpdateScore(scoreThreeLine*currentLevel);

			} else if (numberOfRowsCleared == 4) {

				UpdateScore(scoreFourLine*currentLevel);

			}

			UpdateLines(numberOfRowsCleared);
			PlayLineClearSound ();

		}

		numberOfRowsCleared = 0;

	}

	//- Checks if you should Level Up
	public void LevelUp () {
		if (numberOfLines >= whenToLevelUp) {
			whenToLevelUp += levelInterval;
			UpdateLevel(1);
			LevelUp();
		}
	}

	public void PlayLineClearSound () {
		audioSource.PlayOneShot (lineClearAudio);
	}

	public bool CheckIsAboveGrid(Tetromino tetromino){
		for(int x = 0; x < gridWidth; x++){
			foreach(Transform mino in tetromino.transform){
				Vector2 pos = Round (mino.position);

				if(pos.y > gridHeight - 1){
					return true;
				}
			}
		}
		return false;
	}

	public bool IsFullRowAt (int y){
		for (int i = 0; i < gridWidth; i++) {
			if(grid[i, y] == null){

				return false;
			}
		}

		numberOfRowsCleared++;
		return true;
	}

	public void DeleteMinoAt(int y){
		for (int x = 0; x < gridWidth; x++) {
			Destroy(grid[x,y].gameObject);

			grid[x,y] = null;
		}
	}

	public void MoveRowDown (int y) {
		for(int x = 0; x < gridWidth; x++){
			if(grid[x,y] != null){

				grid[x,y-1] = grid[x,y];
				grid[x,y] = null;
				grid[x,y-1].position += new Vector3(0,-1,0);
			}
		}
	}

	public void MoveAllRowsDown (int y){
		for(int i = y; i < gridHeight; i++){
			MoveRowDown(i);
		}
	}

	public void DeleteRow(){
		for (int y = 0; y < gridHeight; y++) {
			if(IsFullRowAt(y)){
				DeleteMinoAt(y);
				MoveAllRowsDown(y+1);
				y--;
			}
		}
	}

	public Transform GetTransformAtGridPosition (Vector2 pos){

		if (pos.y > gridHeight - 1) {
			return null;
		} else {
			return grid[(int)pos.x,(int)pos.y];
		}
	}

	public void UpdateGrid (Tetromino tetromino){

		for(int y = 0; y < gridHeight; y++){
			for(int x = 0; x < gridWidth; x++){

				if(grid[x,y] != null){
					if(grid[x,y].parent == tetromino.transform){
						grid[x,y] = null;
					}
				}
			}
		}

		foreach (Transform mino in tetromino.transform) {
			Vector2 pos = Round(mino.position);
			if(pos.y < gridHeight){
				grid[(int)pos.x,(int)pos.y] = mino;
			}
		}
	}

	public bool CheckIsInsideGrid (Vector2 pos) {

		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
	}

	public Vector2 Round (Vector2 pos){

		return new Vector2 (Mathf.Round(pos.x), Mathf.Round(pos.y));
	}

	public void SpawnTetromino (){

		previewTetromino.transform.localPosition = nextTetrominoPosition;
		nextTetromino = previewTetromino;
		nextTetromino.GetComponent<Tetromino> ().enabled = true;
		NewPreviewTetromino ();
	}

	public void	NewPreviewTetromino(){

		previewTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
		previewTetromino.GetComponent<Tetromino> ().enabled = false;

	}

	string GetRandomTetromino(){
		int rand = Random.Range (1, 8);
		string randomTetroName = "Prefabs/Tetromino_T";

		switch (rand){

		case 1:
			randomTetroName = "Prefabs/Tetromino_J";
			break;

		case 2:
			randomTetroName = "Prefabs/Tetromino_L";
			break;

		case 3:
			randomTetroName = "Prefabs/Tetromino_Long";
			break;

		case 4:
			randomTetroName = "Prefabs/Tetromino_S";
			break;

		case 5:
			randomTetroName = "Prefabs/Tetromino_Square";
			break;

		case 6:
			randomTetroName = "Prefabs/Tetromino_T";
			break;

		case 7:
			randomTetroName = "Prefabs/Tetromino_Z";
			break;
		}

		return randomTetroName;
	}

	public void GameOver(){
		Application.LoadLevel ("GameOver");
	}

} //- End of class
