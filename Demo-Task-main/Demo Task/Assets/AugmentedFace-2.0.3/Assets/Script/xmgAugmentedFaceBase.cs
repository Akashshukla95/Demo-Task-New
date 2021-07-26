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
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class xmgAugmentedFaceBase : MonoBehaviour
{

    public xmgVideoCaptureParameters m_videoParameters;
    protected xmgAugmentedFaceBridge.xmgImage m_image;
    protected xmgAugmentedFaceBridge.xmgVideoCaptureParams videoParams;
    protected xmgVideoCapturePlane m_capturePlane;

    protected xmgAugmentedFaceBridge.xmgRigidFaceData rigidData;
    protected GameObject m_facePivot, m_faceObject, m_faceMask;
    protected string m_debugStatus = ""; 
	protected Texture2D imgTexture;
    protected Texture2D uvTexture;

    [Tooltip("Default Orientation for the capture device (on kiosk")]
    public xmgOrientationMode m_captureDeviceOrientation = xmgOrientationMode.LandscapeLeft;

#if (!UNITY_EDITOR && UNITY_ANDROID)
    protected bool m_camera_ready = false;
    private bool m_has_lost_focus = false;
#endif

    public void Awake()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID)
        m_camera_ready = false;
        // -- Camera permission for Android
        GameObject dialog = null;
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            dialog = new GameObject();
        }
#endif
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // -- this is to avoid loosing the video capture when switching apps
#if (!UNITY_EDITOR && UNITY_ANDROID)
        if (m_camera_ready)
        {
            Debug.Log("==> Focus " + hasFocus);
            if (hasFocus == false)
            {
                xmgAugmentedFaceBridge.xzimgCamera_delete();
                m_has_lost_focus = true;
            }
            else if (m_has_lost_focus)
            {
                xmgAugmentedFaceBridge.xzimgCamera_create(ref videoParams);
                m_has_lost_focus = false;
            }
        }
#endif
    }


    protected void CheckParameters()
    {
#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS))
        if (m_videoParameters.useNativeCapture)
            m_captureDeviceOrientation = xmgOrientationMode.LandscapeLeft;
#endif
    }
   

    public void ShowObject(bool state)
    {
        CreateObjects();
        Renderer[] renderers = m_facePivot.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) r.enabled = state;
    }
    

    public void CreateObjects()
    {
        if (m_facePivot == null)
        {
            m_facePivot = new GameObject("Face Pivot");

            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject g in gos)
            {
                if (g.name == "faceObject")
                {
                    m_faceObject = g;
                    m_facePivot.transform.parent = m_faceObject.transform.parent;
                    m_faceObject.transform.parent = m_facePivot.transform;
                    Renderer[] renderers = m_faceObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in renderers)
                    {
                        r.material.renderQueue = 3020;
                    }
                }
                if (g.name == "faceMask")
                {
                    m_faceMask = g;
                    m_faceMask.transform.parent = m_facePivot.transform;
                }
            }
        }
    }

   public  void UpdateObjectPosition()
    {
        Quaternion quatRot = Quaternion.Euler(0, 0, 0);
        bool mirror_pose = m_videoParameters.MirrorVideo;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        int rotation = 0;
        if (Screen.orientation == ScreenOrientation.LandscapeRight) rotation = 2;
        else if (Screen.orientation == ScreenOrientation.Portrait) rotation = 3;
        else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) rotation = 1;
		if (rotation == 1) 
            quatRot = Quaternion.Euler(0, 0, 90);
        else if (rotation == 2)
            quatRot = Quaternion.Euler(0, 0, 180);
        else if (rotation == 3)
			quatRot = Quaternion.Euler(0, 0, -90);
		else 
			quatRot = Quaternion.Euler(0, 0, 0);


#endif
#if (UNITY_IOS) && !UNITY_EDITOR
        mirror_pose = !m_videoParameters.MirrorVideo; // ios camera image is inverted
        if (!m_videoParameters.UseFrontal)
        {
            if (rotation == 1)
                quatRot = Quaternion.Euler(0, 0, -90);
            if (rotation == 2)
                quatRot = Quaternion.Euler(0, 0, 0);
            if (rotation == 3)
                quatRot = Quaternion.Euler(0, 0, 90);
            if (rotation == 0)
                quatRot = Quaternion.Euler(0, 0, 180);
        }
#endif

        CreateObjects();
        if (m_facePivot)
        {
            Vector3 position = rigidData.m_position; position.y = -position.y;
            Vector3 euler = rigidData.m_euler;
            Quaternion quat = Quaternion.Euler(euler);  
            if (mirror_pose)
            {
                quat.y = -quat.y;
                quat.z = -quat.z;
                position.x = -position.x;
            }
            
            position.x *= m_videoParameters.VideoPlaneScale;
            position.y *= m_videoParameters.VideoPlaneScale;
            m_facePivot.transform.position = position;
            m_facePivot.transform.rotation = quat;

            m_facePivot.transform.localPosition = quatRot * position;
            m_facePivot.transform.localRotation = quatRot * quat;

            m_facePivot.transform.localScale = new Vector3(
                m_videoParameters.VideoPlaneScale, m_videoParameters.VideoPlaneScale, m_videoParameters.VideoPlaneScale);
        }
    }

    public void DisposeObjects()
    {
        if (m_facePivot && m_facePivot.gameObject)
            Destroy(m_facePivot.gameObject);
    }

    public void ResetTracking()
    {
        xmgAugmentedFaceBridge.xzimgRestartTracking();
    }


    public void PrepareCamera()
    {
        // Compute correct focal length according to video capture crops and different available modes
        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally &&
            (xmgTools.GetRenderOrientation() == xmgOrientationMode.LandscapeLeft || xmgTools.GetRenderOrientation() == xmgOrientationMode.LandscapeRight))
        {
            float fovx = (float)xmgTools.ConvertFov(m_videoParameters.CameraVerticalFOV, m_videoParameters.GetVideoAspectRatio());
            Camera.main.fieldOfView = (float)xmgTools.ConvertFov(fovx, 1.0f / m_videoParameters.GetScreenAspectRatio());
        }
        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenVertically &&
            (xmgTools.GetRenderOrientation() == xmgOrientationMode.LandscapeLeft || xmgTools.GetRenderOrientation() == xmgOrientationMode.LandscapeRight))
        {
            //float scaleY = (float)xmgVideoCapturePlane.GetScaleY(m_videoParameters);
            Camera.main.fieldOfView = m_videoParameters.CameraVerticalFOV;// / scaleY;
        }

        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenHorizontally &&
            (xmgTools.GetRenderOrientation() == xmgOrientationMode.Portrait || xmgTools.GetRenderOrientation() == xmgOrientationMode.PortraitUpsideDown))
        {
            Camera.main.fieldOfView = (float)xmgTools.ConvertFov(m_videoParameters.CameraVerticalFOV, m_videoParameters.GetVideoAspectRatio());
        }

        if (m_videoParameters.videoPlaneFittingMode == xmgVideoPlaneFittingMode.FitScreenVertically &&
            (xmgTools.GetRenderOrientation() == xmgOrientationMode.Portrait || xmgTools.GetRenderOrientation() == xmgOrientationMode.PortraitUpsideDown))
        {
            Camera.main.fieldOfView = (float)xmgTools.ConvertFov(m_videoParameters.CameraVerticalFOV, m_videoParameters.GetScreenAspectRatio());
        }
        //Debug.Log("fovy = "+ Camera.main.fieldOfView);

        Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }


    public void SwitchCamera()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID) || (!UNITY_EDITOR && UNITY_IOS)
        int front = 1 - videoParams.m_frontal;
        xmgAugmentedFaceBridge.xzimgCamera_delete();
        videoParams.m_frontal = front;
        m_videoParameters.MirrorVideo = front==0?false:true;
        m_videoParameters.UseFrontal = front==0?false:true;    
        m_capturePlane.SetMirror(front);
        xmgAugmentedFaceBridge.xzimgCamera_create(ref videoParams);
#endif

    }
};

    