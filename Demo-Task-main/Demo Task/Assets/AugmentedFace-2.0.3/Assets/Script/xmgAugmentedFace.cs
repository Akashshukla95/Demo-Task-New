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

/// <summary>
///  Main class to track faces
/// </summary>
public class xmgAugmentedFace : xmgAugmentedFaceBase
{
    private WebCamTexture m_webcamTexture;
    private xmgAugmentedFaceBridge.xmgTrackingParams trackingParams;
    private int m_detectedStatus = 0;

    void Start()
    {
        m_videoParameters.CheckVideoCaptureParameters();
        double fovy_degree = (double)m_videoParameters.CameraVerticalFOV;
        if (!m_videoParameters.useNativeCapture)
		{
            // -- Non native (Unity based) webcam engine
            if (m_webcamTexture == null)
            {
                m_capturePlane = (xmgVideoCapturePlane)gameObject.AddComponent(typeof(xmgVideoCapturePlane));
                m_webcamTexture = m_capturePlane.OpenVideoCapture(ref m_videoParameters);
                m_capturePlane.CreateVideoCapturePlane(
                    m_videoParameters.VideoPlaneScale, 
                    m_videoParameters);

                if (!m_webcamTexture)              
                    Debug.Log("==> (E) No camera detected by [webcamTexture] Unity Class");
            }
		}
        else
        {
            // -- Native camera capture using xzimgCamera
            xmgAugmentedFaceBridge.PrepareNativeVideoCaptureDefault(
                ref videoParams, 
                m_videoParameters.videoCaptureMode, 
                m_videoParameters.UseFrontal?1:0);
            
            m_capturePlane = (xmgVideoCapturePlane)gameObject.AddComponent(typeof(xmgVideoCapturePlane));
            m_capturePlane.CreateVideoCapturePlane(
                m_videoParameters.VideoPlaneScale,
                m_videoParameters);

#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
            xmgAugmentedFaceBridge.xzimgCamera_create(ref videoParams);
            m_camera_ready = true;
#endif
        }

        // -- Launch the tracking engine
        xmgAugmentedFaceBridge.PrepareTrackingParams(
            ref trackingParams,
            m_videoParameters.GetProcessingWidth(),
            m_videoParameters.GetProcessingHeight(),
            0);

        xmgAugmentedFaceBridge.xzimgInitializeRigidTracking();

        double arVideo = (double)m_capturePlane.m_captureWidth / m_capturePlane.m_captureHeight;
        xmgAugmentedFaceBridge.xzimgSetCalibration(
            xmgTools.ConvertFov(fovy_degree, arVideo) * 3.1415 / 180.0,             // we want fovx in rds
            m_videoParameters.GetProcessingWidth(),
            m_videoParameters.GetProcessingHeight());

        xmgAugmentedFaceBridge.PrepareImage(
            ref m_image, 
            m_capturePlane.m_captureWidth, 
            m_capturePlane.m_captureHeight, 
            m_capturePlane.m_PixelsHandle.AddrOfPinnedObject());

        // Find the parameters for Unity camera
        PrepareCamera();
    }

    void OnDisable()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
        if (m_videoParameters.useNativeCapture)
            xmgAugmentedFaceBridge.xzimgCamera_delete();
#endif
        xmgAugmentedFaceBridge.xzimgReleaseRigidTracking();
        m_capturePlane.ReleaseVideoCapturePlane();
        DisposeObjects();
    }

    void Update ()
    {
        if (!m_videoParameters.useNativeCapture)
        {
            if (m_capturePlane == null || !m_capturePlane.GetData()) return;
        }
        else
        {
#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
            int res = xmgAugmentedFaceBridge.xzimgCamera_getImage(m_capturePlane.m_PixelsHandle.AddrOfPinnedObject());
#endif
        }
        m_detectedStatus = xmgAugmentedFaceBridge.xzimgRigidTracking(ref m_image, ref trackingParams, ref rigidData);
        m_capturePlane.ApplyTexture();

        // -- Manage device rotations
        if (m_detectedStatus == 0)
        {
            trackingParams.m_rotate_mode = (int)xmgTools.GetDeviceCurrentOrientation((int)m_captureDeviceOrientation, m_videoParameters.UseFrontal) ;
        }
        
        if (m_detectedStatus > 0)
		{
			ShowObject(true);
			UpdateObjectPosition();
		}
		
		if (m_detectedStatus <= 0)
		{
			ShowObject(false);
		}		
	}
    

    void OnGUI()
    {
        if (m_videoParameters.ScreenDebug)
        {
#if (UNITY_STANDALONE || UNITY_EDITOR)
            if (m_webcamTexture!= null)
                GUILayout.Label(
                    "Screen: " + Screen.width + "x" + Screen.height + " - " + 
                    m_webcamTexture.requestedWidth + "x" + m_webcamTexture.requestedHeight);
#endif

            if (m_detectedStatus == -11)
                GUILayout.Label("Tracking Status: OFF - PROTECTION ACTIVATED (RELOAD)");
            else
                GUILayout.Label("Tracking Status: " + m_detectedStatus);
			GUILayout.Label("Screen Orientation = " + Screen.orientation + "(" + (int)Screen.orientation + ")");
			GUILayout.Label("Device Orientation = " + Input.deviceOrientation + "(" + (int)Input.deviceOrientation + ")");
            if (m_videoParameters.useNativeCapture)
                GUILayout.Label("Native Capture ON");
            else
                GUILayout.Label("Native Capture OFF");

#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
            if (m_videoParameters.useNativeCapture)
            {
                GUILayout.Label("Native resolution: " + 
                    xmgAugmentedFaceBridge.xzimgCamera_getCaptureWidth() + " x " + 
                    xmgAugmentedFaceBridge.xzimgCamera_getCaptureHeight());
            }
#endif
        }

    }
}
