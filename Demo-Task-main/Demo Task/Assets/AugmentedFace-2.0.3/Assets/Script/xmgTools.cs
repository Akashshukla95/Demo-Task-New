/**
*
* Copyright (c) 2018 XZIMG Limited , All Rights Reserved
* No part of this software and related documentation may be used, copied,
* modified, distributed and transmitted, in any form or by any means,
* without the prior written permission of XZIMG
*
* contact@xzimg.com, www.xzimg.com
*
*/

using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

public enum xmgOrientationMode
{
    LandscapeLeft = 0,
    Portrait = 1,
    LandscapeRight = 2,
    PortraitUpsideDown = 3,
};

/**
 * Common tool functions
 */
public class xmgTools : MonoBehaviour
{
    static public xmgOrientationMode GetRenderOrientation(bool isFrontalCamera = true)
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL)
        return xmgOrientationMode.LandscapeLeft;
#elif (UNITY_ANDROID)
		if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeLeft;
		else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.PortraitUpsideDown;
		else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeRight;
		else return xmgOrientationMode.Portrait;        
#elif (UNITY_IOS)
		if (isFrontalCamera)
		{
			if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeRight;
			else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.Portrait;
			else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeLeft;
			else return xmgOrientationMode.PortraitUpsideDown;
		}
		else
		{
		if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeLeft;
		else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.PortraitUpsideDown;
		else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeRight;
		else return xmgOrientationMode.Portrait;
		}
#endif
    }

    static public xmgOrientationMode GetVideoOrientation(bool useNativeCamera, bool isFrontalCamera = true)
    {
        //return xmgOrientationMode.Portrait;
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL)
        return xmgOrientationMode.LandscapeLeft;
#elif (UNITY_ANDROID) || (UNITY_IOS)
		if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeRight;
		else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.Portrait;
		else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeLeft;
		else return xmgOrientationMode.PortraitUpsideDown;
        
//#elif (UNITY_IOS)
		//if (isFrontalCamera)
		//{
		//	if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeLeft;
		//	else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.PortraitUpsideDown;
		//	else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeRight;
		//	else return xmgOrientationMode.Portrait;
		//}
		//else
		//{
		//	if (Screen.orientation == ScreenOrientation.LandscapeRight) return xmgOrientationMode.LandscapeRight;
		//	else if (Screen.orientation == ScreenOrientation.Portrait) return xmgOrientationMode.Portrait;
		//	else if (Screen.orientation == ScreenOrientation.LandscapeLeft) return xmgOrientationMode.LandscapeLeft;
		//	else return xmgOrientationMode.PortraitUpsideDown;
		//}
#endif
    }

    // -------------------------------------------------------------------------------------------------------------------

    static public xmgOrientationMode GetDeviceCurrentOrientation(int captureDeviceOrientation, bool isFrontalCamera = false)
    {
        xmgOrientationMode orientation = xmgOrientationMode.LandscapeLeft;// Default portrait
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL)
        orientation = (xmgOrientationMode)captureDeviceOrientation;
#elif (UNITY_ANDROID)
        orientation = xmgOrientationMode.Portrait; // Default
        DeviceOrientation deviceOrientation = Input.deviceOrientation;
        if (deviceOrientation == DeviceOrientation.LandscapeRight) orientation = xmgOrientationMode.LandscapeRight;
        if (deviceOrientation == DeviceOrientation.LandscapeLeft) orientation = xmgOrientationMode.LandscapeLeft;
        if (deviceOrientation == DeviceOrientation.PortraitUpsideDown) orientation = xmgOrientationMode.PortraitUpsideDown;
		if (!isFrontalCamera && deviceOrientation == DeviceOrientation.Portrait) orientation = xmgOrientationMode.PortraitUpsideDown;
		if (!isFrontalCamera && deviceOrientation == DeviceOrientation.PortraitUpsideDown) orientation = xmgOrientationMode.Portrait;
#elif (UNITY_IOS)
        orientation = xmgOrientationMode.PortraitUpsideDown; // Default
        DeviceOrientation deviceOrientation = Input.deviceOrientation;
        if (deviceOrientation == DeviceOrientation.LandscapeRight) orientation = xmgOrientationMode.LandscapeRight;
        if (deviceOrientation == DeviceOrientation.LandscapeLeft) orientation = xmgOrientationMode.LandscapeLeft;
        if (deviceOrientation == DeviceOrientation.PortraitUpsideDown) orientation = xmgOrientationMode.Portrait;
        if (!isFrontalCamera && deviceOrientation == DeviceOrientation.LandscapeRight) orientation = xmgOrientationMode.LandscapeLeft;
        if (!isFrontalCamera && deviceOrientation == DeviceOrientation.LandscapeLeft) orientation = xmgOrientationMode.LandscapeRight;

#endif
        return orientation;
    }

    // -------------------------------------------------------------------------------------------------------------------

    static public float ConvertToRadian(float degreeAngle)
    {
        return (degreeAngle * ((float)Math.PI / 180.0f));
    }
    static public double ConvertToRadian(double degreeAngle)
    {
        return (degreeAngle * (Math.PI / 180.0f));
    }
    static public float ConvertToDegree(float degreeAngle)
    {
        return (degreeAngle * (180.0f / (float)Math.PI));
    }
    static public double ConvertToDegree(double degreeAngle)
    {
        return (degreeAngle * (180.0f / Math.PI));
    }
    static public double ConvertHorizontalFovToVerticalFov(double radianAngle, double aspectRatio)
    {
        return (Math.Atan(1.0 / aspectRatio * Math.Tan(radianAngle / 2.0)) * 2.0);
    }

    static public double ConvertVerticalFovToHorizontalFov(double radianAngle, double aspectRatio)
    {
        return (Math.Atan(aspectRatio * Math.Tan(radianAngle / 2.0)) * 2.0);
    }

    static public double ConvertFov(double degreeAngle, double aspectRatio)
    {
        return ConvertToDegree(Math.Atan(aspectRatio * Math.Tan(ConvertToRadian(degreeAngle) / 2.0)) * 2.0);
    }

    static public double ConvertFovy2fovx(double degreeAngle)
    {
        float arScreen = (float)Screen.width / (float)Screen.height;
        return ConvertToDegree(Math.Atan(arScreen * Math.Tan(ConvertToRadian(degreeAngle) / 2.0)) * 2.0);
    }
}