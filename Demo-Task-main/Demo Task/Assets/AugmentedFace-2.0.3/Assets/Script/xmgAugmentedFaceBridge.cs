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
using System;
using System.Runtime.InteropServices;

/// <summary>
///  API bridge to native code
/// </summary>
public class xmgAugmentedFaceBridge
{

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgImage
    {
        public int m_width;
        public int m_height;

        public IntPtr m_imageData;

        /// 0: Black and White, 1: Color RGB, 2: Color BGR, 3: Color BGRA, 4: Color ARGB
        public int m_colorType;

        /// 0: unsigned char, 1: float, 2: double
        public int m_type;

        /// Horizontal flipping
        public bool m_flippedH;
    }

    static public void PrepareImage(
        ref xmgImage dstimage, int width, int height, IntPtr ptrdata)
    {
        dstimage.m_width = width;
        dstimage.m_height = height;
        dstimage.m_colorType = 3;
        dstimage.m_type = 0;
        dstimage.m_flippedH = true;     // webCamtexture pixels are upside down
        dstimage.m_imageData = ptrdata;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgPoint2
    {
        public double x, y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgPoint3
    {
        public double x, y, z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgSize
    {
        public int w, h;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgTrackingParams
    {
        public xmgSize m_processingSize;
        public int m_rotate_mode;
        public int m_detect_without_eyes;
        public System.IntPtr texturePtr;
    }

    static public void PrepareTrackingParamsDefault(
        ref xmgTrackingParams trackingParams)
    {
        trackingParams.m_processingSize.w = 320;
        trackingParams.m_processingSize.h = 240;
        trackingParams.m_rotate_mode = 0;
        trackingParams.m_detect_without_eyes = 1;
        trackingParams.texturePtr = System.IntPtr.Zero;
    }

    static public void PrepareTrackingParams(
        ref xmgTrackingParams trackingParams,
        int proc_w, int proc_h,
        int rotationMode = 0)
    {
        trackingParams.m_processingSize.w = proc_w;
        trackingParams.m_processingSize.h = proc_h;
        trackingParams.m_rotate_mode = rotationMode;
        trackingParams.m_detect_without_eyes = 1;
        trackingParams.texturePtr = System.IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgRigidFaceData
    {
        public double m_FaceCorner_x;
        public double m_FaceCorner_y;
        public double m_FaceSize_w;
        public double m_FaceSize_h;

        public int m_iPoseDetected;
        public Vector3 m_position;
        public Vector3 m_euler;
        public Quaternion m_quatRot;

        public xmgPoint3 m_leftEye;
        public xmgPoint3 m_rightEye;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xmgVideoCaptureParams
    {
        public int m_resolutionMode;                // 0 is 320x240; 1, is 640x480; 2 is 720p (-1 if no internal capture)
        public int m_frontal;                       // 0 is frontal; 1 is back
        public int m_focusMode;                     // 0 auto-focus now; 1 auto-focus continually; 2 locked; 3; focus to point
        public int m_exposureMode;                  // 0 auto-focus now; 1 auto-focus continually; 2 locked; 3; focus to point
        public int m_whileBalanceMode;              // 0 auto-focus now; 1 auto-focus continually; 2 locked; 3; focus to point
    }

    static public void PrepareNativeVideoCapture(
        ref xmgVideoCaptureParams videoCaptureParams,
        int resolutionMode,
        int frontal,
        int focusMode,
        int exposureMode,
        int whileBalanceMode)
    {
        videoCaptureParams.m_resolutionMode = resolutionMode;
        videoCaptureParams.m_frontal = frontal;
        videoCaptureParams.m_focusMode = focusMode;
        videoCaptureParams.m_exposureMode = exposureMode;
        videoCaptureParams.m_whileBalanceMode = whileBalanceMode;
    }

    static public void PrepareNativeVideoCaptureDefault(
        ref xmgVideoCaptureParams videoCaptureParams, int resolutionMode, int frontal)
    {
        videoCaptureParams.m_resolutionMode = resolutionMode;
        videoCaptureParams.m_frontal = frontal;
        videoCaptureParams.m_focusMode = 1;
        videoCaptureParams.m_exposureMode = 1;
        videoCaptureParams.m_whileBalanceMode = 1;

#if (UNITY_ANDROID)
        videoCaptureParams.m_focusMode = 2;
        videoCaptureParams.m_exposureMode = -1;
        videoCaptureParams.m_whileBalanceMode = -1;
#endif
    }

#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID)
    [DllImport("xzimgAugmentedFace")]
    public static extern void xzimgInitializeRigidTracking();

    [DllImport("xzimgAugmentedFace")]
    public static extern int xzimgRigidTracking(
        [In][Out] ref xmgImage imageIn,
        [In][Out] ref xmgTrackingParams trackingData,
        [In][Out] ref xmgRigidFaceData rigidData);

    [DllImport("xzimgAugmentedFace")]
    public static extern void xzimgSetCalibration(
        double fovx,
        int processingWidth, int processingHeight);

    [DllImport("xzimgAugmentedFace")]
    public static extern void xzimgReleaseRigidTracking();

    [DllImport("xzimgAugmentedFace")]
    public static extern void xzimgRestartTracking();

    [DllImport("xzimgAugmentedFace")]
    public static extern void xzimgPauseTracking(int pause);

#elif (UNITY_WEBGL || UNITY_IOS)
    [DllImport("__Internal")]
    public static extern void xzimgInitializeRigidTracking();
    
    [DllImport("__Internal")]
    public static extern int xzimgRigidTracking(
        [In][Out] ref xmgImage imageIn, 
        [In][Out] ref xmgTrackingParams trackingData, 
        [In][Out] ref xmgRigidFaceData rigidData);
    
    [DllImport("__Internal")]
    public static extern void xzimgSetCalibration(
        double fovx, 
        int processingWidth, int processingHeight);

    [DllImport("__Internal")]
    public static extern void xzimgReleaseRigidTracking();
    
    [DllImport("__Internal")]
    public static extern void xzimgRestartTracking();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject m_videoActivity = null;
    private static AndroidJavaObject m_activityContext = null;

    public static void xzimgCamera_create([In][Out] ref xmgVideoCaptureParams videoCaptureParams)
    {
        if (m_activityContext == null)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_activityContext = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }

        if (m_videoActivity == null)
        {
            AndroidJavaClass xzimg_video_plugin = new AndroidJavaClass("com.xzimg.videocapture.VideoCaptureAPI");
            if (xzimg_video_plugin != null)
            {
                m_videoActivity = xzimg_video_plugin.CallStatic<AndroidJavaObject>("instance");
            }
        }
        if (m_videoActivity != null)
            m_videoActivity.Call("xzimgCamera_create", 
                videoCaptureParams.m_resolutionMode, 
                videoCaptureParams.m_frontal,
                videoCaptureParams.m_focusMode,
                videoCaptureParams.m_whileBalanceMode);
    }

    //[DllImport("xzimgAugmentedFace")]
    //public static extern int xzimgCamera_create([In][Out] ref xmgVideoCaptureParams videoCaptureParams);
    //[DllImport("xzimgAugmentedFace")]
    //public static extern int xzimgCamera_delete();

    public static void xzimgCamera_delete()
    {
        if (m_videoActivity != null)
            m_videoActivity.Call("xzimgCamera_delete");
    }

    [DllImport("xzimgAugmentedFace")]
    public static extern int xzimgCamera_getCaptureWidth();
    [DllImport("xzimgAugmentedFace")]
    public static extern int xzimgCamera_getCaptureHeight();
    [DllImport("xzimgAugmentedFace")]
    public static extern int xzimgCamera_getImage(System.IntPtr rgba_frame);
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern int xzimgCamera_create([In][Out] ref xmgVideoCaptureParams videoCaptureParams);
    [DllImport("__Internal")]
    public static extern int xzimgCamera_delete();
    [DllImport("__Internal")]
    public static extern int xzimgCamera_getCaptureWidth();
    [DllImport("__Internal")]
    public static extern int xzimgCamera_getCaptureHeight();
    [DllImport("__Internal")]
    public static extern int xzimgCamera_getImage(System.IntPtr rgba_frame);
#endif

}
