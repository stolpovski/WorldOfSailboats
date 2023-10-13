using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_2023_3_OR_NEWER || RENDER_GRAPH_ENABLED // RenderGraph
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
#endif

namespace WaterSystem.Rendering
{
    public class WaterCausticsPass : ScriptableRenderPass
    {
        private static Material _material;
        
        private class PassData
        {
            internal Material WaterCausticMaterial;
            internal Mesh m_mesh;
            internal Matrix4x4 matrix;
        }

        public WaterCausticsPass(Material material)
        {
            profilingSampler = new ProfilingSampler(GetType().Name);
            if (_material == null)
            {
                _material = material;
            }
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var passData = new PassData();
            SetupPassData(ref passData, renderingData.cameraData.worldSpaceCameraPos);
            
            // Stop the pass rendering in the preview and if material is missing
            if (!ExecutionCheck(renderingData.cameraData.camera, passData.WaterCausticMaterial)) return;
            
            var cmd = CommandBufferPool.Get();
#if UNITY_2023_3_OR_NEWER || RENDER_GRAPH_ENABLED // RenderGraph
            ExecutePass(passData, CommandBufferHelpers.GetRasterCommandBuffer(cmd));
#else
            ExecutePass(passData,cmd);
#endif

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
#if UNITY_2023_3_OR_NEWER || RENDER_GRAPH_ENABLED // RenderGraph
        public override void RecordRenderGraph(RenderGraph renderGraph, FrameResources frameResources, ref RenderingData renderingData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(nameof(WaterCausticsPass), out var passData, profilingSampler))
            {
                SetupPassData(ref passData, renderingData.cameraData.worldSpaceCameraPos);
                
                // Stop the pass rendering in the preview and if material is missing
                if (!ExecutionCheck(renderingData.cameraData.camera, passData.WaterCausticMaterial)) return;
                
                builder.UseTextureFragment(((UniversalRenderer)renderingData.cameraData.renderer).activeColorTexture, 0);
                builder.UseTexture(frameResources.GetTexture(UniversalResource.CameraDepthTexture));

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    ExecutePass(data, context.cmd);
                });
            }
        }
#endif
        
#if UNITY_2023_3_OR_NEWER || RENDER_GRAPH_ENABLED // RenderGraph
        static void ExecutePass(PassData data, RasterCommandBuffer cmd)
#else
        static void ExecutePass(PassData data, CommandBuffer cmd)
#endif
        {
            // Draw the mesh with the caustic material and matrix
            cmd.DrawMesh(data.m_mesh, data.matrix, data.WaterCausticMaterial, 0, 0);
        }

        private void SetupPassData(ref PassData data, Vector3 cameraPosition)
        {
            var sunMatrix = RenderSettings.sun != null
                ? RenderSettings.sun.transform.localToWorldMatrix
                : Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-45f, 45f, 0f), Vector3.one);
            data.WaterCausticMaterial = _material;
            data.WaterCausticMaterial.SetMatrix("_MainLightDir", sunMatrix);
            data.WaterCausticMaterial.SetFloat("_WaterLevel", Ocean.Instance.transform.position.y);
            // Create mesh if needed
            if (!data.m_mesh)
                data.m_mesh = PassUtilities.GenerateCausticsMesh(1000f);
            
            cameraPosition.y = Ocean.Instance.transform.position.y;
            data.matrix = Matrix4x4.TRS(cameraPosition, Quaternion.identity, Vector3.one);
        }

        private bool ExecutionCheck(Camera cam, Material mat)
        {
            if (cam.cameraType is not (CameraType.SceneView or CameraType.Game)) return false;
            return mat != null;
        }
    }
}