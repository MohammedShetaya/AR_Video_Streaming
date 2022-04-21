# AR_Video_Streaming

## Table of Content:
 * [Overview](#overview)
 * [Dependencies](#dependencies)
   * [WebRTC](#webrtc)
   * [Render Streaming](#renderstreaming)
* [Streaming ARFoundation Camera to browser](#ar-camera-streaming)
* [Sending Web browser Input to Unity](#sending-web-browser-input-to-unity)

## Overview

This Project aims to apply remote guidance using Augmented Reality between the browser and the android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser, then the browser can annotate that stream with AR features. The project is based on the  [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) And [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) packages.


## Dependencies

>### [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
Allows real-time, peer-to-peer, media exchange between two devices. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address. The solution for this is to use a signaling server.

#### Signaling Server: 
The signaling server acts as an interface between the Unity Android application and the browser clients so they can Start sending signaling messages to each other. Using an HTTP server in this case would not help in the case of WebRTC, so it is optimal to use a WebSocket server. WebSocket connection is statefull (FullDuplex) unlike the HTTP connection where the server can not send responses to the client unless the client sends a request. Websocket servers can send and receive requests at any moment in the connection lifetime. In the case of WebRTC, the server will never know when a client will send a signaling message so it can be forwarded to the other client.

#### Singnaling Process:
1. The browser client sends and Offer message to the websocket server
2. The Signaling server receives the message from the broswer and forwards it to the Unity client
3. The Unity client receives the offer and set this Offer as its RemoteDescription
4. The Unity client create an Answer and set this Answer as its LocalDescription
5. The Unity client sends the Answer to the browser client
7. The Signaling server receives the message from the broswer and forwards it to the Unity client
8. The browser client receives the Answer and set this Answer as its RemoteDescription
9. The Two clients register to the `onIceCandidate` event and once the event handler is called, it should send the collected iceCandidate to the other Peer.
10. Once the other Peer receives an iceCandidate, it should call `AddIceCanidate` in order to set the [SDP](https://en.wikipedia.org/wiki/Session_Description_Protocol).

![Signaling Process Browser API](./webrtc_signaling_diagram.svg) 

#### Peer Connection:
Once the two peers set their `LocalDescription` and `RemoteDescription` They can start exchaning real-time data (Video, Audio, etc..). A `MediaStream` object can be sent over the `RTCPeerConnection` using the `AddTrack` method. The other peer can register to the `OnTrack` event which is will be called once a track is received.

##### Remarks:
1. The `MediaStream` object should be added using the `AddTrack` method before sending an Offer/Answer to the other peer. Adding a track should be followed by new Offer/Answer in order for the other peer to have an updated SDP.
2. The `IceCandidate` should be handled on the remote peer after the `RemoteDescritption` is set.
3. The sending peer needs to register to `OnNegotiationNeeded` event which is called once `AddTrack` finishes execution. The handler of this event should send a new Offer to the remote peer with the new SDP. 

&nbsp;
&nbsp;
&nbsp;

>### [RenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html)
Unity Render streaming is based on the WebRTC protocol. It allows streaming real-time data on a peer to peer connection using WebRTC. This package also allows sending input data from the browser to Unity.

#### Streaming ARFoundation Camera:
To stream the arCamera to the browser this project used `RenderStreaming` and `Broadcast` scripts.
`RenderStreaming`: is the base class for the Unity RenderStreaming package. It is responsible for creating a connection to the Web server. It allows two types of communication to the server. In our project we used the WebSocket signaling.

## AR Camera Streaming 

