 using UnityEngine;
using System.Collections;

// The Camera is located on the backside of the Gear VR. It's resolution is 3840 x 2160.

public class magicCamera : MonoBehaviour
{
	
	// Define a camera texture
	WebCamTexture BackCamera;
	// Define an array to save the value of color 
	Color32[] data;
	// Defie the size of camera
	public float width = 1.0f;
	public float height = 1.0f;
	// Shader mode (0: nothing, 1: bright, 2: dark, 3: black n white, 4: ripple
	public int shader = 0;
	// help control user input
	bool manipulating = true;
	bool stop = true;
	bool next = true;


    // Use this for initialization
    void Start ()
	{
		
		// Android defaults to back camera	
		BackCamera = new WebCamTexture();
		// Set the texture of the gameobject to the camera texture		
		gameObject.GetComponent<Renderer>().material.mainTexture = BackCamera;
		// Keep the texture updating
		BackCamera.Play();
		
	}
    public bool backButtonIsClicked()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
    // Update is called once per frame
    void Update ()
	{

		// Flip the texture upright
		gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1.0f, 1.0f);
		// Move the texture down
		gameObject.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.0f, 0.0f);
		// check if the video buffer has changed since the last frame
		if (BackCamera.didUpdateThisFrame)
			// Get the pixels of the frame
			data = BackCamera.GetPixels32();
		// Reverse the data 
		// System.Array.Reverse (data);
		// Define a empty  texture2D
		int factor = 2;
		
		
		Texture2D destTex = new Texture2D(BackCamera.width, BackCamera.height);
		Texture2D Tex = new Texture2D (BackCamera.width / factor, BackCamera.height / factor);
		destTex.SetPixels32 (data);
		for (int y = 0; y < Tex.height; y++) {
			for (int x = 0; x < Tex.width; x++) {
				Tex.SetPixel(x,y,destTex.GetPixel(x*factor,y*factor));
			}
		}
		// getting user inputs
		float SwipeX = Input.GetAxis ("Mouse X");
		float SwipeY = Input.GetAxis ("Mouse Y");

		// swipe backward
		if (SwipeX > 0.5f && manipulating)
			{
				shader--;
			}
			// swipe forward
		else if (SwipeX < -0.5f && manipulating)
			{
				shader++;
			}
			// swipe up - move screen away from camera
		else if (SwipeY > 0.5f)
			{
			}
			// swipe down - move screen closer to camera
		else if (SwipeY < -0.5f)
			{
			}
		// control the shader mode
		if (shader < 0)
		{
			shader = 4;
		}
		else if (shader > 4)
		{
			shader = 0;
		}
        // control the user input

        if(Input.GetMouseButton(1))

        {
           Input.GetKeyDown(KeyCode.Escape);
        }
        else
        {
        }
        if (Input.GetMouseButton (0))
		{
			manipulating = true;
		}
		else
		{
			manipulating = false;
		}

		// control the ripple script activation
		if (stop)
		{
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.enabled = false;
		}


		// bright shader
		if (shader == 1)
		{
			// turn brightness up
			Light myLight = GameObject.Find ("Light").GetComponent<Light>();
			myLight.intensity = 2;

			// turn ripple script off
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.dampner = 0;
			stop = true;
			//rippleScript.enabled = false;
		}
		// dark shader
		else if (shader == 2)
		{
			// turn brightness down
			Light myLight = GameObject.Find ("Light").GetComponent<Light>();
			myLight.intensity = 0;

			// turn ripple script off
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.dampner = 0;
			stop = true;
			//rippleScript.enabled = false;
		}
		// black and white shader
		else if (shader == 3)
		{
			// set the light back
			Light myLight = GameObject.Find ("Light").GetComponent<Light>();
			myLight.intensity = 1;

			// turn ripple script off
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.dampner = 0;
			stop = true;
			//rippleScript.enabled = false;

			Color32 color32;
			for (int y = 0; y < Tex.height; y++) {
				for (int x = 0; x < Tex.width; x++) {
					color32 = Tex.GetPixel (x, y);
					// shader - black and white
					if ((color32.r + color32.g + color32.b) > 255 * 2) {
						color32.r = 255;
						color32.g = 255;
						color32.b = 255;
					} 
					else if ((color32.r + color32.g + color32.b) < 255)
					{
						color32.r = 0;
						color32.g = 0;
						color32.b = 0;
					}
					else {
						color32.r = 123;
						color32.g = 123;
						color32.b = 123;
					}
					
					Tex.SetPixel (x, y, color32);	
				}
			}
		}
		// ripple shader
		else if (shader == 4)
		{
			// set the light back
			Light myLight = GameObject.Find ("Light").GetComponent<Light>();
			myLight.intensity = 1;

			// turn ripple script on
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.dampner = 0.990f;
			rippleScript.enabled = true;
			stop = false;

		}
		// if no shader
		else
		{
			// set the light back
			Light myLight = GameObject.Find ("Light").GetComponent<Light>();
			myLight.intensity = 1;

			// turn ripple script off
			var rippleScript = gameObject.GetComponent<rippleSharp>();
			rippleScript.dampner = 0;
			stop = true;
		}

        // back to oculus store
        /*
                private void HandleGearVRInput(InputDevice device)
            {
                    if (device.Action1.WasPressed)
                    {
                        if (_backButtonTimeSinceLastTap < _doubleTapDelay)
                        {
                            _tapCount++;
                            if (_tapCount == 2)
                            {
                                // Double tap happened
                                MainMenuManager.Instance.ToggleMainMenu();
                                _tapCount = 0;
                            }
                        }
                        else
                        {
                            _tapCount = 0;
                        }

                        _backButtonTimeSinceLastTap = 0.0f;
                        _currentBackButtonDownTime = 0.0f;
                    }
                    else
                    {
                        // no new tap this frame
                        _backButtonTimeSinceLastTap += Time.unscaledDeltaTime;

                        if (device.Action1.IsPressed)
                        {
                            _currentBackButtonDownTime += Time.unscaledDeltaTime;

                        }
                        else if (device.Action1.WasReleased)
                        {
                            if (_currentBackButtonDownTime >= _backButtonLongPressDelay)
                            {
                                // show the platform UI
                                OVRPlugin.ShowUI(OVRPlugin.PlatformUI.GlobalMenu);
                            }
                            _currentBackButtonDownTime = 0.0f;
                        }
                    }
                }
            */

        //True if the user has clicked the back button



    //  Run;
    Tex.Apply ();
		// Make renderer display the texture;
		GetComponent<Renderer> ().material.mainTexture = Tex;

    }
    // use back button back to Oculus Store


}


