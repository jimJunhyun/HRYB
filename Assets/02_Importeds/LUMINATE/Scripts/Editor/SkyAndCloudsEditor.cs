using GapperGames;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(SkyAndClouds))]
    sealed class SkyAndCloudsEditor : VolumeComponentEditor
    {
        SerializedDataParameter enabled;
        SerializedDataParameter compositing;
        SerializedDataParameter clouds;
        SerializedDataParameter sky;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<SkyAndClouds>(serializedObject);

            enabled = Unpack(o.Find(x => x.enabled));
            compositing = Unpack(o.Find(x => x.compositing));
            clouds = Unpack(o.Find(x => x.clouds));
            sky = Unpack(o.Find(x => x.sky));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(enabled); 
            PropertyField(compositing);
            PropertyField(clouds);
            PropertyField(sky);

            SkyAndClouds _sky = (SkyAndClouds)target;
            CloudShadows.enabledInPost = _sky.clouds.value.cloudShadows;
        }
    }
}
