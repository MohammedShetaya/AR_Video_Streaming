using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using Unity.WebRTC;
using WebSocketSharp;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace Unity.ARVideoStreaming
{
    public class WSClient : MonoBehaviour
    {


        List<IEnumerator> threadPumpList = new List<IEnumerator>();

        private Camera arCamera;
        private WebSocket socket;

        private VideoStreamTrack track;
        private RTCPeerConnection localConnection;
        private List<RTCIceCandidate> iceCandidateBuffer = new List<RTCIceCandidate>();
        bool remoteDescSet = false ;

        private void Awake()
        {
            //connect to the WebServer through websocket
            socket = new WebSocket("ws://localhost?sender=unity");
            socket.OnMessage += (o,e) => {
                threadPumpList.Add(handleIncommingMessages(o, e));
            };

            socket.Connect();
            Debug.Log(socket.ReadyState);
                
            WebRTC.WebRTC.Initialize(EncoderType.Software);
            arCamera = GetComponent<Camera>();

        }

        private void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            while (threadPumpList.Count > 0)
            {
                StartCoroutine(threadPumpList[0]);
                threadPumpList.RemoveAt(0);
            }

            if (remoteDescSet) {
                while(iceCandidateBuffer.Count > 0)
                {
                    RTCIceCandidate iceCandidate = iceCandidateBuffer[0];
                    localConnection.AddIceCandidate(iceCandidate);
                    iceCandidateBuffer.RemoveAt(0);
                    Debug.Log("ice candidate msg is handled");
                }
            } 
		}

       
        public IEnumerator handleIncommingMessages(object sender, MessageEventArgs e)
        {

            Debug.Log("Got message");
            Debug.Log(e.Data);
            SignalingMessage message = JsonConvert.DeserializeObject<SignalingMessage>(e.Data);


            switch (message.type)
            {
                case "offer": yield return handleOffer(message); break;
                case "answer": yield return handleAnswer(message); break;
                case "ice-candidate": handleIceCandidateMessage(message); break;
                case "user-joined": callJoinedUser(); break;
            }
        }

        private void callJoinedUser()
        {
            //Create local peer
            RTCConfiguration config = default;
            config.iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } };
            localConnection = new RTCPeerConnection(ref config);

            localConnection.OnNegotiationNeeded = () => { Debug.Log("negotiation needed"); StartCoroutine(handleNegotiationNeeded()); };;
            localConnection.OnIceCandidate += handleIceCandidate;
            localConnection.OnIceConnectionChange = handleIceConnectionChange;


                RenderTexture rt;
                if (arCamera.targetTexture != null)
                {
                    rt = arCamera.targetTexture;
                    RenderTextureFormat supportFormat = WebRTC.WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
                    GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(supportFormat, RenderTextureReadWrite.Default);
                    GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, FormatUsage.Render);
                    GraphicsFormat format = graphicsFormat == compatibleFormat ? graphicsFormat : compatibleFormat;
                    
                    if (rt.graphicsFormat != format)
                    {
                        Debug.LogWarning(
                            $"This color format:{rt.graphicsFormat} not support in unity.webrtc. Change to supported color format:{format}.");
                        rt.Release();
                        rt.graphicsFormat = format;
                        rt.Create();
                    }

                    arCamera.targetTexture = rt;
                }
                else
                {
                    RenderTextureFormat format = WebRTC.WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
                    rt = new RenderTexture(1270, 720, 0, format)
                    {
                        antiAliasing = 1
                    };
                    rt.Create();
                    arCamera.targetTexture = rt;
                }
                track = new VideoStreamTrack(rt);
                Debug.Log(rt.format);
                Debug.Log(track.Texture.graphicsFormat);
                Debug.Log(track.IsEncoderInitialized);
                localConnection.AddTrack(track);
                Debug.Log("track added");

        }

        private IEnumerator handleNegotiationNeeded()
        {
            var offer = localConnection.CreateOffer();
            yield return offer;

            var offerDesc = offer.Desc;
            var setLocalDesc = localConnection.SetLocalDescription(ref offerDesc);
            yield return setLocalDesc;

            socket.Send(JsonConvert.SerializeObject(new SignalingMessage
            {
                type = "offer",
                sessionDescription = offerDesc
            }));
        }

        private void handleIceCandidate(RTCIceCandidate candidate)
        {
            if (!string.IsNullOrEmpty(candidate.Candidate))
            {
                socket.Send(JsonConvert.SerializeObject(new SignalingMessage
                {
                    type = "ice-candidate",
                    outcommingIceCandidate = candidate
                }));
            }
        }
        private void handleIceConnectionChange(RTCIceConnectionState state)
        {
            Debug.Log("IceCandidate state is " + state.ToString());
        }


        public IEnumerator handleOffer(SignalingMessage offerMessage)
        {

            var setRemoteDesc = localConnection.SetRemoteDescription(ref offerMessage.sessionDescription);
            yield return setRemoteDesc;

            var answer = localConnection.CreateAnswer();
            yield return answer;

            var answerDesc = answer.Desc;
            var setLocalDesc = localConnection.SetLocalDescription(ref answerDesc);
            yield return setLocalDesc;

            SignalingMessage answerMessage = new SignalingMessage
            {
                type = "answer",
                sessionDescription = answerDesc
            };

            socket.Send(JsonConvert.SerializeObject(answerMessage));

            Debug.Log("offer is handled");
        }

        public IEnumerator handleAnswer(SignalingMessage answerMessage)
        {
            var setRemoteDesc = localConnection.SetRemoteDescription(ref answerMessage.sessionDescription);
            yield return setRemoteDesc;
            remoteDescSet = true;

            Debug.Log("Answer is handled");

        }

        public void handleIceCandidateMessage(SignalingMessage candidateMessage)
        {
            
            RTCIceCandidate iceCandidate = new RTCIceCandidate(candidateMessage.incommingIceCandidate);
            iceCandidateBuffer.Add(iceCandidate);
        }

    }

}