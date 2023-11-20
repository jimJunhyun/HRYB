using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GapperGames
{
    public class CloudRenderPass : ScriptableRenderPass
    {
        private static Mesh mesh;

        private Material material;

        private const string profilerTag = "Test Pass";

        public CloudRenderPass(Material _mat)
        {
            material = _mat;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //Setup variables and do null checks
            var cam = renderingData.cameraData.camera;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            if (!mesh) mesh = GenerateQuad(10000000f);

            //Matrices -> ew
            float nearClipPlane = cam.nearClipPlane;
            cam.nearClipPlane = 50;
            var position = cam.transform.position + (cam.transform.forward * cam.farClipPlane * 0.9f);
            var matrix = Matrix4x4.TRS(position, cam.transform.rotation, Vector3.one);
            cmd.DrawMesh(mesh, matrix, material, 0, 0);

            cam.nearClipPlane = nearClipPlane;

            //Execute command buffer
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");
        }

        //Generates a circle mesh (except it has 4 vertices all at 90 degrees in a square shape)
        private static Mesh GenerateQuad(float size)
        {
            var m = new Mesh();

            size *= 0.5f;

            var verts = new[]
            {
                new Vector3(-size, -size, 0),
                new Vector3(size, -size, 0),
                new Vector3(-size, size, 0),
                new Vector3(size, size, 0)
            };

            var tris = new[]
            {
                0, 2, 1,
                2, 3, 1
            };

            m.vertices = verts;
            m.triangles = tris;

            return m;
        }
    }

}