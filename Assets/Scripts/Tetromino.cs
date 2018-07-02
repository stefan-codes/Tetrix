using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour {

	float lastFall = 0;
	public float fallInterval = 1;
	public bool allowRotation = true;
	public bool limitRotation = false;

	public AudioClip moveSound; //- Sound for move
	public AudioClip landSound;

	public AudioSource audioSource;

	// Use this for initialization
	void Start () {
	
		audioSource = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {

        CheckUserInput ();
	}
    
	//Player controlls
    void CheckUserInput(){
        if(Input.GetKeyDown(KeyCode.RightArrow)){

			transform.position += new Vector3(1,0,0);
			if(CheckIsValidPosition()){
				FindObjectOfType<Game>().UpdateGrid(this);
				PlayMoveAudio ();

			} else {
				transform.position += new Vector3(-1,0,0);
			}

		} else if(Input.GetKeyDown(KeyCode.LeftArrow)){

			transform.position += new Vector3(-1,0,0);
			if(CheckIsValidPosition()){
				FindObjectOfType<Game>().UpdateGrid(this);
				PlayMoveAudio();

			} else {
				transform.position += new Vector3(1,0,0);
			}

		} else if(Input.GetKeyDown(KeyCode.UpArrow)){

			if(allowRotation){
				if(limitRotation){
					if(transform.rotation.eulerAngles.z >= 90) {
						transform.Rotate(0,0,-90);
					} else {
						transform.Rotate(0,0,90);
					}
				} else {
					transform.Rotate(0,0,90);
				}
					
				if(CheckIsValidPosition()){
					FindObjectOfType<Game>().UpdateGrid(this);

				} else {
					if(limitRotation){ 
						if(transform.rotation.eulerAngles.z >= 90){
							transform.Rotate(0,0,-90);
						} else {
							transform.Rotate(0,0,90);
						}
					} else {
						transform.Rotate(0,0,-90);
					}
				}
			}
		} else if(Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= fallInterval - 0.1*FindObjectOfType<Game>().currentLevel) {
			lastFall = Time.time;
			transform.position += new Vector3(0,-1,0);
			if(CheckIsValidPosition()){
				FindObjectOfType<Game>().UpdateGrid(this);
				if(Input.GetKeyDown(KeyCode.DownArrow)){
					PlayMoveAudio();
				}
			} else {
				transform.position += new Vector3(0,1,0);
				FindObjectOfType<Game>().DeleteRow();
				if(FindObjectOfType<Game>().CheckIsAboveGrid(this)){
					FindObjectOfType<Game>().GameOver();
				}
				enabled = false;
				FindObjectOfType<Game>().SpawnTetromino();
			}
		}
    }

	bool CheckIsValidPosition (){
		foreach (Transform mino in transform){
			Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
			if(FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false){
				return false;
			}

			if(FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform) {
				return false;
			}
		}
		return true;
	}
			
	void PlayMoveAudio () {

		audioSource.PlayOneShot (moveSound);
	}

	void PlayLandAudio () {

		audioSource.PlayOneShot (landSound);

	}
}













