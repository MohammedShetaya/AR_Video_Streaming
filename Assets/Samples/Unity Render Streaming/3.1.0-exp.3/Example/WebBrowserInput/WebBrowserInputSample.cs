using UnityEngine;
using UnityEngine.UI;

namespace Unity.RenderStreaming.Samples
{
    class WebBrowserInputSample : MonoBehaviour
    {
        [SerializeField] RenderStreaming renderStreaming;
        [SerializeField] Transform[] cameras;
        [SerializeField] CopyTransform copyTransform;

        // Start is called before the first frame update
        void Start()
        {
            if (!renderStreaming.runOnAwake)
            {
                renderStreaming.Run(
                    hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
                    signaling: RenderStreamingSettings.Signaling);
            }
        }

    }
}
