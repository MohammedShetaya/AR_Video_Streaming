using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using WebSocketSharp;

namespace Unity.ARVideoStreaming
{
	public delegate void DelegateOnIceCandidateMessage(SignalingMessage m);
}