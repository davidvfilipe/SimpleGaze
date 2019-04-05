using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Raycast based interaction system
/// Instantiates a 3D sphere in front of the camera that sticks to surfaces and interacts with with some based on their tag
/// </summary>
public class SimpleGaze: MonoBehaviour
{
	[Header("Player")]
	[SerializeField,Tooltip("As to be child of the player because in VR we cant apply transforms to the camera")] private Camera _viewCamera;
	// Height of the player
	private float _height;

	[Header("Cursor")]
	[SerializeField] private GameObject _cursorPrefab;
	[SerializeField,Tooltip("Distance that the cursor appears away from the camera")] private float _cursorDistance;
	[SerializeField,Tooltip("Time in seconds that we have to be looking for it to interact")] private float _gazeTime;
	[SerializeField,Tooltip("Cursor Material to change to when its looking at an interactable item")] private Material _interactable;
	[SerializeField,Tooltip("Cursor Material to change to when its looking at an not interactable item")] private Material _notInteractable;

	
	// Cursor prefab instance mesh renderer
	private MeshRenderer _meshRenderer;
	private GameObject cursorInstance;

	// Time that changes once we are interacting
	private float _timer;
	private bool _isInteracting;

	/// <summary>
	/// This function only runs once at the start
	/// </summary>
	private void Start ()
	{
		// Instantiate the cursor object
		cursorInstance = Instantiate(_cursorPrefab);
		// Change the cursor to the unselected material
		_meshRenderer = cursorInstance.GetComponent<MeshRenderer>();
		_meshRenderer.material = _notInteractable;
		// Save the height of the player
		_height = gameObject.transform.position.y;
		// Reset the timer
		_timer = _gazeTime;
	}
	
	/// <summary>
	/// This function runs once per frame
	/// </summary>
	private void Update () 
	{
		// Each frame update the cursor based on were the player is looking at
		CursorLocation();
		
		// Make the X button exit the application
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
		
		// Start the countdown timer if we are looking to an interactable object
		if (_isInteracting){
			
			// Subtract time from the timer
			// This is the same as ( _timer = _timer - Time.deltaTime )
			_timer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Updates the cursor based on what the camera is pointed at.
	/// </summary>
	private void CursorLocation()
	{
		// Create a gaze ray pointing forward from the camera
		var ray = new Ray(_viewCamera.transform.position, _viewCamera.transform.rotation * Vector3.forward);
		RaycastHit hit;
		
		// Change the cursor position to hit the surface we are looking at
		if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
				// If the ray hits something, set the position to the hit point
				cursorInstance.transform.position = hit.point;

				// This can be later replaced with a switch case
			
				// If it interacts with a teleport pad change te player location to the pads location ( notice that the tag "Teleport" needs to be crated )
				if (hit.transform.CompareTag("Teleport")){
					// Change the cursor material
					_meshRenderer.material = _interactable;
					_isInteracting = true;

					if (_timer <= 0){
						// Change the players position to the hit point position
						transform.position = hit.point;
						// Because the height changes in the line before, change it to the start height
						transform.position = new Vector3(transform.position.x,_height,transform.position.z);
						_isInteracting = false;
						_timer = _gazeTime;
					}
				} 
				/*
				else if (hit.transform.CompareTag("Button")){
					// Change the cursor material
					_meshRenderer.material = _interactable;
					_isInteracting = true;
					
					if (_timer <= 0){
						
						// Code used in class 
						hit.transform.gameObject.GetComponent<MyButton>().Acender();
						
						_isInteracting = false;
						_timer = _gazeTime;
					}
				} 
				*/
				
				// If we are not looking at an interactable object reset the timer and change the cursor material
				else{
					_timer = _gazeTime;
					_isInteracting = false;
					_meshRenderer.material = _notInteractable;
				}
			
		} else{
			// If the ray doesn't hit anything change the cursor position to the _cursorDistance
			cursorInstance.transform.position = ray.origin + ray.direction.normalized * _cursorDistance;
			
			// Change the cursor material and reset the timer
			_meshRenderer.material = _notInteractable;
			_timer = _gazeTime;
			_isInteracting = false;
		}
	}
}
