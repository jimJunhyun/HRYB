using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GapperGames
{

    [ExecuteAlways, DisallowMultipleComponent]
    public class RenderSky : MonoBehaviour
    {
        [Range(0.01f, 1f)] public static float renderScale = 1f;
        public static LayerMask skyLayer = -1;
        private static Camera _skyCamera;
        private static RenderTexture _skyTexture;
        RenderTextureDescriptor previousDescriptor;
        private readonly int _skyTextureID = Shader.PropertyToID("_SkyTexture");
        public static event Action<ScriptableRenderContext, Camera> BeginPlanarReflections;
        public bool hideReflectionCamera = true;
        private Material skyMaterial;


        //Helper Functions and Setup//

        void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += DoRenderSky;
            //reflectionLayer = ~(1 << 4);
        }

        void OnDisable()
        {
            CleanUp();
            RenderPipelineManager.beginCameraRendering -= DoRenderSky;
        }

        void OnDestroy()
        {
            CleanUp();
            RenderPipelineManager.beginCameraRendering -= DoRenderSky;
        }

        void CleanUp()
        {
            if (_skyCamera)
            {
                _skyCamera.targetTexture = null;
                SafeDestroyObject(_skyCamera.gameObject);
            }

            if (_skyTexture) RenderTexture.ReleaseTemporary(_skyTexture);

        }

        void SafeDestroyObject(UnityEngine.Object obj)
        {
            if (Application.isEditor) DestroyImmediate(obj);
            else Destroy(obj);
        }


        //Actual Code//


        private void UpdateSkyCamera(Camera realCamera)
        {
            if (_skyCamera == null) _skyCamera = InitializeReflectionCamera();

            UpdateCamera(realCamera, _skyCamera);
            _skyCamera.gameObject.hideFlags = (hideReflectionCamera) ? HideFlags.HideAndDontSave : HideFlags.DontSave;
#if UNITY_EDITOR
            EditorApplication.DirtyHierarchyWindowSorting();
#endif

            _skyCamera.cullingMask = skyLayer;
            realCamera.clearFlags = CameraClearFlags.SolidColor;
            realCamera.backgroundColor = Color.black;
        }

        private void UpdateCamera(Camera src, Camera dest)
        {
            //Set up render targets
            if (dest == null) return;

            dest.CopyFrom(src);
            dest.useOcclusionCulling = false;

            if (dest.gameObject.TryGetComponent(out UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData))
            {
                camData.renderShadows = false;
                camData.renderPostProcessing = false;
                dest.clearFlags = CameraClearFlags.Skybox;
            }
        }

        //Hidden rendering camera for sky - allows sky to be rendered at lower resolution
        private Camera InitializeReflectionCamera()
        {
            var go = new GameObject("", typeof(Camera));
            go.name = "Sky Camera [" + go.GetInstanceID() + "]";
            var camData = go.AddComponent(typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)) as UnityEngine.Rendering.Universal.UniversalAdditionalCameraData;

            camData.requiresColorOption = CameraOverrideOption.On;
            camData.requiresDepthOption = CameraOverrideOption.On;
            camData.SetRenderer(0);

            var t = transform;
            var reflectionCamera = go.GetComponent<Camera>();
            //reflectionCamera.transform.SetPositionAndRotation(t.position, t.rotation);
            reflectionCamera.depth = -10;
            reflectionCamera.enabled = false;

            return reflectionCamera;
        }

        //Create render texture descriptor at correct resolution
        RenderTextureDescriptor GetDescriptor(Camera camera, float pipelineRenderScale)
        {
            var width = (int)Mathf.Max(camera.pixelWidth * pipelineRenderScale * renderScale);
            var height = (int)Mathf.Max(camera.pixelHeight * pipelineRenderScale * renderScale);
            var hdr = camera.allowHDR;
            var renderTextureFormat = hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

            return new RenderTextureDescriptor(width, height, renderTextureFormat, 16)
            {
                autoGenerateMips = true,
                useMipMap = true
            };
        }

        private void CreateSkyTexture(Camera camera)
        {
            var descriptor = GetDescriptor(camera, UniversalRenderPipeline.asset.renderScale);

            if (_skyTexture == null)
            {
                _skyTexture = RenderTexture.GetTemporary(descriptor);
                _skyTexture.filterMode = FilterMode.Trilinear;
                previousDescriptor = descriptor;
            }
            else if (!descriptor.Equals(previousDescriptor))
            {
                if (_skyTexture) RenderTexture.ReleaseTemporary(_skyTexture);

                _skyTexture = RenderTexture.GetTemporary(descriptor);
                _skyTexture.filterMode = FilterMode.Trilinear;
                previousDescriptor = descriptor;
            }
            _skyCamera.targetTexture = _skyTexture;
        }

        //Renders Sky
        private void DoRenderSky(ScriptableRenderContext context, Camera camera)
        {
            SkyAndClouds sac = VolumeManager.instance.stack.GetComponent<SkyAndClouds>();

            if (sac.IsActive())
            {
                if (camera.cameraType == CameraType.Reflection || camera.cameraType == CameraType.Preview) return;

                if (skyMaterial == null)
                {
                    skyMaterial = (Material)Resources.Load("Rendering/SkyRenderer");
                }
                else
                {
                    RenderSettings.skybox = skyMaterial;
                }

                UpdateSkyCamera(camera);
                CreateSkyTexture(camera);


                //if (_skyCamera.gameObject.TryGetComponent(out UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData))
                //{
                //    camData.renderPostProcessing = false;
                //}
                //_skyCamera.allowHDR = false;

                //BeginPlanarReflections?.Invoke(context, _skyCamera);
                UniversalRenderPipeline.RenderSingleCamera(context, _skyCamera);


                //if (_skyCamera.gameObject.TryGetComponent(out UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData1))
                //{
                //    camData1.renderPostProcessing = true;
                //}

                //SubmitRenderRequest(_skyCamera, context);
                Shader.SetGlobalTexture(_skyTextureID, _skyTexture);
            }
            else
            {
                return;
            }
        }

        //Just makes the code a bit cleaner
        public static bool Compare(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }

}