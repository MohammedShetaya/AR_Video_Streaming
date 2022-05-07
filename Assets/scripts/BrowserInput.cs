using System;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.RenderStreaming;

namespace Unity.ARVideoStreaming
{
    public class BrowserInput : InputChannelReceiverBase
    {

        private ARRayCasting rayCasting;
        private RTCDataChannel channel;

		private void Awake()
		{
            rayCasting = GameObject.FindGameObjectWithTag("CameraSessionOrigin").GetComponent<ARRayCasting>();
		}

		public override void SetChannel(string connectionId, RTCDataChannel channel)
        {
            channel.OnMessage += handleDataChannelMessages;
            this.channel = channel;
            base.SetChannel(connectionId, channel);
        }

        private void handleDataChannelMessages(byte[] bytes)
        {
            string s = "";
            foreach(var  b in bytes) {
                s += b +" "; 
            }

            Debug.Log(s); 
            float x = BitConverter.ToSingle(bytes, 0);
            float y = BitConverter.ToSingle(bytes, 4);
            Debug.Log(x);
            Debug.Log(y);

            rayCasting.shootArrow( new Vector2(x,y));
        }

    }
}