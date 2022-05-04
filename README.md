# AR_Video_Streaming

## Table of Content:
 * [Overview](#overview)
 * [Dependencies](#dependencies)
   * [WebRTC](#webrtc)
   * [Render Streaming](#renderstreaming)
* [Streaming ARFoundation Camera to browser](#ar-camera-streaming)
* [Sending Web browser Input to Unity](#sending-web-browser-input-to-unity)
* [Applying RayCasting in Unity](#RayCasting)
* [Web Server](#webserver)
* [Browser Client](#browser-client)

>## Overview

This Project aims to apply remote guidance using Augmented Reality annotation between the web browser and an android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser which is responsible for plane detection, then the browser can annotate that stream with AR objects e.g. Arrows. 


&nbsp;
&nbsp;

>## Installation and setup



&nbsp;
&nbsp;

>## Dependencies
The project is based on:
- [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) protocol for the browser and unity (version `2.4.0-exp.6`). 
- [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) package (version `3.1.0-exp.3`).


### [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
This package is an API for WebRTC protocol, but in unity with the same browser implementation which gives a great benifit in using this protocol in unity AR and VR applications. This package is compitble with the browser API so it can be used to allow real-time, peer-to-peer, media exchange between unity-unity application or unity-browser application. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address therefore each peer cannot give his IP to the other peer. The solution for this is to use a signaling server. 

#### Signaling Server: 
The signaling server acts as an interface between the Unity Android application and the browser clients so they can Start sending signaling messages to each other. Using an HTTP server in this case would not help in the case of WebRTC as the signaling messages are being generated asyncrounosly, so it is optimal to use a WebSocket server. WebSocket connection is statefull (FullDuplex) connection. Unlike the HTTP connection where the server cannot send responses to the client unless the client sends a request. Websocket servers can send and receive requests at any moment in the connection lifetime. In the case of WebRTC, the server will never know when a client will send a signaling message so it can be forwarded to the other client.

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

#### P2P Connection:
Once the two peers set their `LocalDescription` and `RemoteDescription` They can start exchaning real-time data (Video, Audio, etc..). A `MediaStream` object can be sent over the `RTCPeerConnection` using the `AddTrack` method. The other peer can register to the `OnTrack` event which is will be called once a track is received.

##### Remarks:
1. The `MediaStream` object should be added using the `AddTrack` method before sending an Offer/Answer to the other peer. Adding a track should be followed by new Offer/Answer in order for the other peer to have an updated SDP.
2. The `IceCandidate` should be handled on the remote peer after the `RemoteDescritption` is set.
3. The sending peer needs to register to `OnNegotiationNeeded` event which is called once `AddTrack` finishes execution. The handler of this event should send a new Offer to the remote peer with the new SDP. 

&nbsp;
&nbsp;

### [RenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html)
Unity Render streaming is based on the WebRTC protocol. It allows streaming real-time data on a peer to peer connection using WebRTC. This package also allows sending input data from the browser to Unity. It supports Windows and Android applications.

&nbsp;
&nbsp;

>## AR Camera Streaming 

Render Streaming is used to stream the camera to arCamer to the browser. In order to do this the following scripts were used:
#### `RenderStreaming`: 
This is the base class for the Unity render streaming package. It is responsible for connecting to the browser. It supports two types of Signaling (HTTP/WebSocket). In this project the Websocket signaling were used.

#### `Broadcast`: 
This script is responsible for handling the singlaing messages.

#### `ARCameraSender`: 
This script extends from Unity.RenderStreaming.VideoStreamSender class. It is responsible for sending the video stream as a `RenderTexture`. This script should be attached to the arCamera object.

#### `CameraTextureMixer`: 
This script is responsible for creating a copy from the Rendered image from the camera and print this image to a `RenderTexture` which will be used in the `ARCameraSender` script.This script should be attached to the arCamera object.

#### `WSClient`
This script is sample script that holds all the logic for sending and receiving a messages between the Unity client and the Web Server.

&nbsp;
&nbsp;

>## Sending Web browser Input To Unity:


&nbsp;
&nbsp;

>## RayCasting:

&nbsp;
&nbsp;

>## Web Server:

&nbsp;
&nbsp;


>## Browser Client

