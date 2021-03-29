using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            ScreenCapture.CaptureScreenshot("Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".png", 2);
        }
    }
}