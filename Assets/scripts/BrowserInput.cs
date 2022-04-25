using System;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.RenderStreaming;

namespace Unity.ARVideoStreaming
{
    public class BrowserInput : InputChannelReceiverBase
    {
        private RTCDataChannel channel;
        public override void SetChannel(string connectionId, RTCDataChannel channel)
        {
            channel.OnMessage += handleDataChannelMessages;
            this.channel = channel;
            base.SetChannel(connectionId, channel);
        }

        private void handleDataChannelMessages(byte[] bytes)
        {

            Debug.Log(System.Text.Encoding.Default.GetString(bytes));
        }

    }
}