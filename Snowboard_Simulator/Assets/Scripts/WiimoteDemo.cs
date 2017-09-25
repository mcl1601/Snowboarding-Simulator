using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using WiimoteApi;

public class WiimoteDemo : MonoBehaviour {

	public GameObject board;
	public GameObject board2;
    private Quaternion initial_rotation;

    private Wiimote wiimote;

    private Vector2 scrollPosition;

    private Vector3 wmpOffset = Vector3.zero;
	private Vector3 lastOffset = Vector3.zero;

	private bool menu = true;
	public bool game = false;
    void Start() {
		initial_rotation = board.transform.localRotation;
    }

	void Update () {
		// toggle the debug menu
		if (Input.GetKeyDown (KeyCode.D))
			menu = !menu;
		
		// skip update if no wiimote connected
        if (!WiimoteManager.HasWiimote()) { return; }

		// set the wiimote to the first in the list
        wiimote = WiimoteManager.Wiimotes[0];

		// read data
        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();

			// read orientation data
            if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS) {
                Vector3 offset = new Vector3(  wiimote.MotionPlus.RollSpeed * 1.25f,
                                                -wiimote.MotionPlus.YawSpeed * 1.25f,
                                                -wiimote.MotionPlus.PitchSpeed * 1.25f) / 95f; // Divide by 95Hz (average updates per second from wiimote)

				// check for slows
				/*if(!wiimote.MotionPlus.RollSlow)
				{
					MotionPlusData data = wiimote.MotionPlus;
					wmpOffset = Vector3.zero;
					data.SetZeroValues ();
				}
				if(!wiimote.MotionPlus.YawSlow)
				{
					MotionPlusData data = wiimote.MotionPlus;
					wmpOffset = Vector3.zero;
					data.SetZeroValues ();
				}
				if(!wiimote.MotionPlus.PitchSlow)
				{
					MotionPlusData data = wiimote.MotionPlus;
					wmpOffset = Vector3.zero;
					data.SetZeroValues ();
				}*/

				// prevents tiny movements from affecting rotation
				if(Math.Abs(offset.x - lastOffset.x) > 0.05f || Math.Abs(offset.y - lastOffset.y) > 0.05f || Math.Abs(offset.z - lastOffset.z) > 0.05f)
				{
                	wmpOffset += offset;
					board.transform.Rotate(offset, Space.Self);
					lastOffset = offset;
				}
            }
        } while (ret > 0);

		// if disconnected
        if (wiimote.current_ext != ExtensionController.MOTIONPLUS)
			board.transform.localRotation = initial_rotation;

		// reset orientation when the remote passes through the zero point
		/*if (Math.Abs(lastOffset.x) < 0.25f && Math.Abs(lastOffset.y) < 0.25f && Math.Abs(lastOffset.z) < 0.25f && game) 
		{
			Reset ();
			Debug.Log ("Reset");
		}*/
		// manually reset orientation
		if (Input.GetKeyDown (KeyCode.Z))
			Reset ();

		// start snowboarding
		if (Input.GetKeyDown (KeyCode.Return)) 
		{
			board2.GetComponent<Rigidbody> ().isKinematic = false;
			game = true;
		}

		wiimote.SendPlayerLED (true, false, false, false);
	}

    void OnGUI()
    {
		if (menu) {
			GUI.Box (new Rect (0, 0, 320, Screen.height), "");

			GUILayout.BeginVertical (GUILayout.Width (300));
			GUILayout.Label ("Wiimote Found: " + WiimoteManager.HasWiimote ());
			if (GUILayout.Button ("Find Wiimote"))
				WiimoteManager.FindWiimotes ();

			if (GUILayout.Button ("Cleanup")) {
				WiimoteManager.Cleanup (wiimote);
				wiimote = null;
			}

			if (wiimote == null)
				return;

			GUILayout.Label ("Extension: " + wiimote.current_ext.ToString ());

			GUILayout.Label ("LED Test:");
			GUILayout.BeginHorizontal ();
			for (int x = 0; x < 4; x++)
				if (GUILayout.Button ("" + x, GUILayout.Width (300 / 4)))
					wiimote.SendPlayerLED (x == 0, x == 1, x == 2, x == 3);
			GUILayout.EndHorizontal ();

			GUILayout.Label ("Set Report:");
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("But/Acc", GUILayout.Width (300 / 4)))
				wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_ACCEL);
			if (GUILayout.Button ("But/Ext8", GUILayout.Width (300 / 4)))
				wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_EXT8);
			if (GUILayout.Button ("B/A/Ext16", GUILayout.Width (300 / 4)))
				wiimote.SendDataReportMode (InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
			if (GUILayout.Button ("Ext21", GUILayout.Width (300 / 4)))
				wiimote.SendDataReportMode (InputDataType.REPORT_EXT21);
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Request Status Report"))
				wiimote.SendStatusInfoRequest ();

			GUILayout.Label ("IR Setup Sequence:");
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Basic", GUILayout.Width (100)))
				wiimote.SetupIRCamera (IRDataType.BASIC);
			if (GUILayout.Button ("Extended", GUILayout.Width (100)))
				wiimote.SetupIRCamera (IRDataType.EXTENDED);
			if (GUILayout.Button ("Full", GUILayout.Width (100)))
				wiimote.SetupIRCamera (IRDataType.FULL);
			GUILayout.EndHorizontal ();

			GUILayout.Label ("WMP Attached: " + wiimote.wmp_attached);
			if (GUILayout.Button ("Request Identify WMP"))
				wiimote.RequestIdentifyWiiMotionPlus ();
			if ((wiimote.wmp_attached || wiimote.Type == WiimoteType.PROCONTROLLER) && GUILayout.Button ("Activate WMP"))
				wiimote.ActivateWiiMotionPlus ();
			if ((wiimote.current_ext == ExtensionController.MOTIONPLUS ||
			         wiimote.current_ext == ExtensionController.MOTIONPLUS_CLASSIC ||
			         wiimote.current_ext == ExtensionController.MOTIONPLUS_NUNCHUCK) && GUILayout.Button ("Deactivate WMP"))
				wiimote.DeactivateWiiMotionPlus ();

			GUILayout.Label ("Calibrate Accelerometer");
			GUILayout.BeginHorizontal ();
			for (int x = 0; x < 3; x++) {
				AccelCalibrationStep step = (AccelCalibrationStep)x;
				if (GUILayout.Button (step.ToString (), GUILayout.Width (100)))
					wiimote.Accel.CalibrateAccel (step);
			}
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Print Calibration Data")) {
				StringBuilder str = new StringBuilder ();
				for (int x = 0; x < 3; x++) {
					for (int y = 0; y < 3; y++) {
						str.Append (wiimote.Accel.accel_calib [y, x]).Append (" ");
					}
					str.Append ("\n");
				}
				Debug.Log (str.ToString ());
			}

			if (wiimote != null && wiimote.current_ext != ExtensionController.NONE) {
				scrollPosition = GUILayout.BeginScrollView (scrollPosition);
				GUIStyle bold = new GUIStyle (GUI.skin.button);
				bold.fontStyle = FontStyle.Bold;
				if (wiimote.current_ext == ExtensionController.NUNCHUCK) {
					GUILayout.Label ("Nunchuck:", bold);
					NunchuckData data = wiimote.Nunchuck;
					GUILayout.Label ("Stick: " + data.stick [0] + ", " + data.stick [1]);
					GUILayout.Label ("C: " + data.c);
					GUILayout.Label ("Z: " + data.z);
				} else if (wiimote.current_ext == ExtensionController.CLASSIC) {
					GUILayout.Label ("Classic Controller:", bold);
					ClassicControllerData data = wiimote.ClassicController;
					GUILayout.Label ("Stick Left: " + data.lstick [0] + ", " + data.lstick [1]);
					GUILayout.Label ("Stick Right: " + data.rstick [0] + ", " + data.rstick [1]);
					GUILayout.Label ("Trigger Left: " + data.ltrigger_range);
					GUILayout.Label ("Trigger Right: " + data.rtrigger_range);
					GUILayout.Label ("Trigger Left Button: " + data.ltrigger_switch);
					GUILayout.Label ("Trigger Right Button: " + data.rtrigger_switch);
					GUILayout.Label ("A: " + data.a);
					GUILayout.Label ("B: " + data.b);
					GUILayout.Label ("X: " + data.x);
					GUILayout.Label ("Y: " + data.y);
					GUILayout.Label ("Plus: " + data.plus);
					GUILayout.Label ("Minus: " + data.minus);
					GUILayout.Label ("Home: " + data.home);
					GUILayout.Label ("ZL: " + data.zl);
					GUILayout.Label ("ZR: " + data.zr);
					GUILayout.Label ("D-Up: " + data.dpad_up);
					GUILayout.Label ("D-Down: " + data.dpad_down);
					GUILayout.Label ("D-Left: " + data.dpad_left);
					GUILayout.Label ("D-Right: " + data.dpad_right);
				} else if (wiimote.current_ext == ExtensionController.MOTIONPLUS) {
					GUILayout.Label ("Wii Motion Plus:", bold);
					MotionPlusData data = wiimote.MotionPlus;
					GUILayout.Label ("Pitch Speed: " + data.PitchSpeed);
					GUILayout.Label ("Yaw Speed: " + data.YawSpeed);
					GUILayout.Label ("Roll Speed: " + data.RollSpeed);
					GUILayout.Label ("Pitch Slow: " + data.PitchSlow);
					GUILayout.Label ("Yaw Slow: " + data.YawSlow);
					GUILayout.Label ("Roll Slow: " + data.RollSlow);
					if (GUILayout.Button ("Zero Out WMP")) {
						data.SetZeroValues ();
						board.transform.rotation = Quaternion.FromToRotation (board.transform.rotation * GetAccelVector (), Vector3.up) * board.transform.rotation;
						board.transform.rotation = Quaternion.FromToRotation (board.transform.forward, Vector3.forward) * board.transform.rotation;
					}
					if (GUILayout.Button ("Reset Offset"))
						wmpOffset = Vector3.zero;
					GUILayout.Label ("Offset: " + wmpOffset.ToString ());
				} else if (wiimote.current_ext == ExtensionController.WIIU_PRO) {
					GUILayout.Label ("Wii U Pro Controller:", bold);
					WiiUProData data = wiimote.WiiUPro;
					GUILayout.Label ("Stick Left: " + data.lstick [0] + ", " + data.lstick [1]);
					GUILayout.Label ("Stick Right: " + data.rstick [0] + ", " + data.rstick [1]);
					GUILayout.Label ("A: " + data.a);
					GUILayout.Label ("B: " + data.b);
					GUILayout.Label ("X: " + data.x);
					GUILayout.Label ("Y: " + data.y);

					GUILayout.Label ("D-Up: " + data.dpad_up);
					GUILayout.Label ("D-Down: " + data.dpad_down);
					GUILayout.Label ("D-Left: " + data.dpad_left);
					GUILayout.Label ("D-Right: " + data.dpad_right);

					GUILayout.Label ("Plus: " + data.plus);
					GUILayout.Label ("Minus: " + data.minus);
					GUILayout.Label ("Home: " + data.home);

					GUILayout.Label ("L: " + data.l);
					GUILayout.Label ("R: " + data.r);
					GUILayout.Label ("ZL: " + data.zl);
					GUILayout.Label ("ZR: " + data.zr);
				}
				GUILayout.EndScrollView ();
			} else {
				scrollPosition = Vector2.zero;
			}
			GUILayout.EndVertical ();
		}
    }

    private Vector3 GetAccelVector()
    {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData();
        accel_x = accel[0];
        accel_y = -accel[2];
        accel_z = -accel[1];

        return new Vector3(accel_x, accel_y, accel_z).normalized;
    }

    [System.Serializable]
    public class WiimoteModel
    {
        public Transform rot;
        public Renderer a;
        public Renderer b;
        public Renderer one;
        public Renderer two;
        public Renderer d_up;
        public Renderer d_down;
        public Renderer d_left;
        public Renderer d_right;
        public Renderer plus;
        public Renderer minus;
        public Renderer home;
    }

	void OnApplicationQuit() {
		if (wiimote != null) {
			WiimoteManager.Cleanup(wiimote);
	        wiimote = null;
		}
	}

	public void Reset()
	{
		MotionPlusData data = wiimote.MotionPlus;
		board.transform.rotation = board2.GetComponent<Transform>().rotation;
		wmpOffset = Vector3.zero;
		data.SetZeroValues ();
	}

	public bool getGame()
	{
		return game;
	}
}
