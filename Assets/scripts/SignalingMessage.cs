using Unity.WebRTC ;
using Newtonsoft.Json.Linq; 

namespace Unity.ARVideoStreaming
{
	public class SignalingMessage
	{
		public string type;
		public RTCSessionDescription sessionDescription;
		public RTCIceCandidateInit incommingIceCandidate;
        public RTCIceCandidate outcommingIceCandidate;


    }

}

