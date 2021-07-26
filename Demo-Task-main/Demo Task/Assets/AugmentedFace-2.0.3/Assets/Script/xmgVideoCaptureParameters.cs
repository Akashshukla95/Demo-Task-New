/**
*
* Copyright (c) 2018 XZIMG , All Rights Reserved
* No part of this software and related documentation may be used, copied,
* modified, distributed and transmitted, in any form or by any means,
* without the prior written permission of xzimg
*
* contact@xzimg.com, www.xzimg.com
*
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public enum xmgVideoPlaneFittingMode
{
    FitScreenHorizontally,
    FitScreenVertically,
};

[System.Serializable]
public class xmgVideoCaptureParameters
{
    [Tooltip("Use Native Capture or Unity WebCameraTexture class - Should be activated for mobiles")]
    public bool useNativeCapture = true;

    [Tooltip("Video device index \n -1 for automatic research")]
    public int videoCaptureIndex = -1;

    [Tooltip("Video capture mode \n 1 is VGA \n 2 is 720p \n 3 is 1080p")]
    public int videoCaptureMode = 2;
    
    [Tooltip("Use frontal camera (for mobiles only)")]
    public bool UseFrontal = false;

    [Tooltip("Mirror the video")]
    public bool MirrorVideo = false;

    [Tooltip("Choose if the video plane should fit  horizontally or vertically the screen (only relevent in case screen aspect ratio is different from video capture aspect ratio)")]
    public xmgVideoPlaneFittingMode videoPlaneFittingMode = xmgVideoPlaneFittingMode.FitScreenHorizontally;

    [Tooltip("To scale up/down the rendering plane")]
    public float VideoPlaneScale = 1.0f;

    [Tooltip("Camera vertical FOV \nThis value will change the main camera vertical FOV")]
    public float CameraVerticalFOV = 50f;

    [Tooltip("Display debug information")]
    public bool ScreenDebug = false;

    // image is flipped upside down (depending on pixel formats and devices)
    private bool m_isVideoVerticallyFlipped = false;

    public void CheckVideoCaptureParameters()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
        if (useNativeCapture)
            Debug.Log("xmgVideoCaptureParameters (useNativeCapture) - Video Capture cannot be set to native for PC/MAC platforms => forcing to FALSE");
        if (UseFrontal)
            Debug.Log("xmgVideoCaptureParameters (UseFrontal) - Frontal mode option is not available for PC/MAC platforms - Use camera index edit box instead => forcing to FALSE");
        useNativeCapture = false;
        UseFrontal = false;
#endif
#if (UNITY_ANDROID || UNITY_IOS) && !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
        if (!useNativeCapture)
            Debug.Log("xmgVideoCaptureParameters (useNativeCapture) - Warning: it is advise to set video capture to native when deploying on Android or iOS");
#endif

#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS))
        useNativeCapture = true;        // This is not advise to change that
        if (UseFrontal && !MirrorVideo)
        {
            MirrorVideo = true;
            Debug.Log("xmgVideoCaptureParameters (MirrorVideo) - Mirror mode is forced on mobiles when using frontal camera => forcing to TRUE");       
        }
        if (!UseFrontal && MirrorVideo)
        {
            MirrorVideo = false;
            Debug.Log("xmgVideoCaptureParameters (MirrorVideo) - Mirror mode is deactivate on mobiles when using back camera => forcing to FALSE");       
        }
#endif

#if (!UNITY_EDITOR && (UNITY_ANDROID))
        if (useNativeCapture)
            m_isVideoVerticallyFlipped = true;
#endif

#if (!UNITY_EDITOR && (UNITY_IOS))
        if (useNativeCapture && UseFrontal) 
            m_isVideoVerticallyFlipped = false;
        else 
        if (useNativeCapture)
            m_isVideoVerticallyFlipped = true;
#endif
        if (videoCaptureMode == 0)
            videoCaptureMode = 1;
#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS))
        if (videoCaptureMode == 3)
            videoCaptureMode = 2;       // Still HD but not Full HD
#endif

#if (UNITY_EDITOR || UNITY_STANDALONE) && UNITY_EDITOR_OSX
        if (videoCaptureMode < 2)
            videoCaptureMode = 2;       // Video Capture on MACOS through Unity is tricky
#endif
    }

    public bool GetVerticalMirror() {
        return m_isVideoVerticallyFlipped;
    }

     public static int getVideoCaptureMode(int width, int height)
    {
        if (width == 320 && height == 240) return 0;
        if (width == 640 && height == 480) return 1;
        if (width == 1280 && height == 720) return 2;
        if (width == 1920 && height == 1080) return 4;
        return -1;
    }

    public int GetVideoCaptureWidth()
    {
        if (videoCaptureMode == 0) return 320;
        if (videoCaptureMode == 2) return 1280;
        if (videoCaptureMode == 3) return 1920;
        return 640;
    }
    public int GetVideoCaptureHeight()
    {
        if (videoCaptureMode == 0) return 240;
        if (videoCaptureMode == 2) return 720;
        if (videoCaptureMode == 3) return 1080;
        return 480;
    }
    public int GetProcessingWidth()
    {
        if (videoCaptureMode == 0) return 320;
        if (videoCaptureMode == 2) return 320;
        if (videoCaptureMode == 3) return 480;
        return 320;
    }
    public int GetProcessingHeight()
    {
        if (videoCaptureMode == 0) return 240;
        if (videoCaptureMode == 2) return 180;
        if (videoCaptureMode == 3) return 270;
        return 240;
    }

    // -------------------------------------------------------------------------------------------------------------

    public int GetProcessingWidth(int videoCaptureWidth)
    {
        if (videoCaptureWidth > 640)
            return videoCaptureWidth / 4;
        else if (videoCaptureWidth > 320)
            return videoCaptureWidth / 2;
        return videoCaptureWidth;
    }

    // -------------------------------------------------------------------------------------------------------------

    public int GetProcessingHeight(int videoCaptureHeight)
    {
        if (videoCaptureHeight > 640)
            return videoCaptureHeight / 4;
        else if (videoCaptureHeight > 320)
            return videoCaptureHeight / 2;
        return videoCaptureHeight;
    }

    public float GetVideoAspectRatio()
    {
        return (float)GetVideoCaptureWidth() / (float)GetVideoCaptureHeight();
    }

    public float GetScreenAspectRatio()
    {
        float screen_AR = (float)Screen.width / (float)Screen.height;
#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS))
        if (Screen.width < Screen.height)
            screen_AR = 1.0f / screen_AR;
#endif
        return screen_AR;

    }
    public double GetMainCameraFovV()
    {
        float video_AR = (float)GetVideoAspectRatio();
        float screen_AR = GetScreenAspectRatio();
        double trackingCamera_fovh_radian = xmgTools.ConvertToRadian((double)CameraVerticalFOV);
        double trackingCamera_fovv_radian;
        if (videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally)
            trackingCamera_fovv_radian = xmgTools.ConvertHorizontalFovToVerticalFov(trackingCamera_fovh_radian, (double)screen_AR);
        else
            trackingCamera_fovv_radian = xmgTools.ConvertHorizontalFovToVerticalFov(trackingCamera_fovh_radian, (double)video_AR);
        return xmgTools.ConvertToDegree(trackingCamera_fovv_radian);
    }

    // Usefull for portrait and reverse protraits modes
    public double GetPortraitMainCameraFovV()
    {
        float video_AR = (float)GetVideoAspectRatio();
        float screen_AR = GetScreenAspectRatio();

        double trackingCamera_fovh_radian = xmgTools.ConvertToRadian((double)CameraVerticalFOV);
        double trackingCamera_fovv_radian;
        if (videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally)
            trackingCamera_fovv_radian = trackingCamera_fovh_radian;
        else
        {
            trackingCamera_fovv_radian = xmgTools.ConvertHorizontalFovToVerticalFov(trackingCamera_fovh_radian, (double)video_AR);
            trackingCamera_fovv_radian = xmgTools.ConvertVerticalFovToHorizontalFov(trackingCamera_fovv_radian, (double)screen_AR);
        }

        return xmgTools.ConvertToDegree(trackingCamera_fovv_radian);
    }


    public double[] GetVideoPlaneScale(double videoPlaneDistance)
    {
        double[] ret = new double[2];

        float video_AR = (float)GetVideoAspectRatio();
        float screen_AR = GetScreenAspectRatio();
        double scale_u, scale_v;

        if (videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally)
        {
            double mainCamera_fovv_radian = xmgTools.ConvertToRadian((double)GetMainCameraFovV());
            double mainCamera_fovh_radian = xmgTools.ConvertVerticalFovToHorizontalFov(mainCamera_fovv_radian, (double)screen_AR);
            scale_u = (videoPlaneDistance * Math.Tan(mainCamera_fovh_radian / 2.0));
            scale_v = (videoPlaneDistance * Math.Tan(mainCamera_fovh_radian / 2.0) * 1.0 / video_AR);
        }
        else
        {
            double mainCamera_fovv_radian = xmgTools.ConvertToRadian((double)GetMainCameraFovV());
            scale_u = (videoPlaneDistance * Math.Tan(mainCamera_fovv_radian / 2.0) * video_AR);
            scale_v = (videoPlaneDistance * Math.Tan(mainCamera_fovv_radian / 2.0));
        }
        ret[0] = scale_u;
        ret[1] = scale_v;
        return ret;
    }
}


class xmgDebug
{
    public static string m_debugMessage = "";
}