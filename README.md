# AR_Video_Streaming

## Table of Content:
 * [Overview](#overview)
 * [Installation](#installation)
 * [Dependencies](#dependencies)
   * [WebRTC](#webrtc)
   * [Render Streaming](#renderstreaming)
* [Streaming ARFoundation Camera to browser](#ar-camera-streaming)
* [Sending Web browser Input to Unity](#sending-web-browser-input-to-unity)
* [Applying RayCasting in Unity](#RayCasting)
* [Object detection on the broser client](#object-detection-localy-on-the-browser-client)
* [Web Server](#webserver)

>## Overview

This Project aims to apply remote guidance using Augmented Reality annotation on a Video stream between the web browser and an android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser which is responsible for plane detection, then the browser can annotate that stream with AR objects e.g. Arrows. 


&nbsp;
&nbsp;

>## Installation
#### Building the Unity application:
1. Clone this repository `https://github.com/MohammedShetaya/AR_Video_Streaming.git`
2. Open project from disk using Unity Hub
3. After the project is open, check if the Build setting is using Android platform
4. On the player settings, check if the Auto graphics API is checked, buIld the project for ARM64, and Android version is 7.0 or above.
5. On the mobile phone, Allow developer mode, then allow USB debugging and Connect the device to the labtop.
6. On Build settings, click build and run.

#### Setting up the Web application:
1. Navigate to the project directory using powershell, then `cd WebApp`, then run the command `npm run dev -- -w`
2. the console will show a link to the local web application

#### Using the Application:
There are two modes, the first one is receiving a video steam from unity and showing this stream on the browser:
1. Open the application on mobile and allow access to the camera.
2. Then go back to the local web app on the browser and click receiver sample.
3. Click on play video button, then the stream should be started.
 
The second mode is video receiver with AR annotation:
1. Open the application on mobile and allow access to the camera.
2. Then go back to the local web app on the browser and click video player.
3. Click on play video button, then the stream should be started.
4. By now the mobile application should have started detecting the planes and the video stream is sent to the browser.
5. Click on any of the detected planes on the browser, then an arrow should point to the click position on both clients.

&nbsp;
&nbsp;

>## Dependencies
The project is based on:
- [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) protocol for the browser and unity (version `2.4.0-exp.6`). 
- [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) package (version `3.1.0-exp.3`).


### [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
This package is an API for WebRTC protocol, but in unity with the same browser implementation which gives a great benifit in using this protocol in unity AR and VR applications. This package is compitble with the browser API so it can be used to allow real-time, peer-to-peer, media exchange between unity-unity application or unity-browser application. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address therefore each peer cannot give his IP to the other peer. The solution for this is to use a signaling server. 

#### Signaling Server: 
The signaling server acts as an interface between the Unity Android application and the browser clients so they can Start sending signaling messages to each other. Using an HTTP server would not help in the case of WebRTC as the signaling messages are being generated asyncrounosly, so it is optimal to use a WebSocket server. WebSocket connection is statefull (FullDuplex) connection. Unlike the HTTP connection where the server cannot send responses to the client unless the client sends a request. Websocket servers can send and receive requests at any moment in the connection lifetime. In the case of WebRTC, the server will never know when a client will send a signaling message so it can be forwarded to the other client.

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
Unity Render streaming is based on the WebRTC protocol. It provides a high level implementation for the sinaling, sending, and receiving process. It allows streaming real-time data on a peer to peer connection using WebRTC. This package also allows sending input data from the browser to Unity by maping the browser Events to Unity Actions. With this package, it is possible to build streaming applications in Unity for both Windows and Andriod.

&nbsp;
&nbsp;

>## AR Camera Streaming 
In order to stream the Unity camera. The following components must be added to the scene:
1. ARSession Origin: This is an origin to the scene when the player start the application. It is responisble for managing all the Trackables that will be added on the run e.g. 3D cubes.
2. AR Session: Controls the lifecycle and configuration options for an AR session

The following scripts are added to the arCamera component which is a subComponent of the ARSession origin component:
#### Rendertreaming: 
This is the base class for the Unity render streaming package. It is responsible for connecting to the signaling server and streaming the provided real-time data. It supports two types of Signaling (HTTP/WebSocket). In this project the Websocket signaling were used as dicussed in the WebRTC part above. This script is expecting inputs of type `SignalingHandlerBase` which is a parent class for the `BroadCast` class used in this project.

#### Broadcast: 
This script is responsible for handling the singlaing messages (offer, answer, ice-candidate) and Sending the input streams to be used in `RenderStreaming` script. This class is expecting inputs of type `StreamSenderBase` which is the parent class for the ARCameraSender used in this project.

#### ARCameraSender: 
This script extends from Unity.RenderStreaming.VideoStreamSender class. It is responsible for sending the video stream as a RenderTexture. This script changes the TargetTexture of the camera to be a RenderTexture instead of rendering to the screen. It should be attached to the arCamera object.

#### CameraTextureMixer : 
This script is responsible for creating a copy from the Rendered image from the camera and print this image to a `RenderTexture` which will be used in the ARCameraSender script. This script should be attached to the arCamera object and should be used only if no other arCamera is used for screen rendering.

#### WSClient (not used)
This script is sample script that holds all the logic for sending and receiving a messages between the Unity client and the Web Server and it was used in the project early stages only.

&nbsp;
&nbsp;

>## Sending Web browser Input To Unity:
In this project the only browser input that is used is the mouse click, although it can be extendend to any browser event. Once the video is received from unity and is shown on the browser video element, the user can start clicking on any place on that video element. The click coordinates is sent to unity as a buffer array of bytes. The following files are used in this part: 

#### Calculating the coordinates:
In the figure, the red point represents the place where the browser client clicks. The browser events will call the onCkick event handler of the video element and attach the coordindates to the mouseEvent. These coordinates are not the projected unity coordinates where the rendered image was projected in the first place, So we needed to calculate the x,y portions colored in blue. `xPortion = X - X'` and `yPortion = Y - Y'`, then we needed to divide by the video scale. Where the video scale is ratio between the original video size and the size displayed on the screen. To calculate the video scale first we need to decide if the video is in lanscape or portrait mode and this is done by checking if `W/H` is greater than `orignalVideo.width/originalVideo.height`. If the video is in landscape mode then `Video Scale = browser video element with / original video with` else it will be `Video Scale = browser video elemnt height / original video element height`. The coordinates in Unity 2d World will be:

<p align="center">
  X = xPortion / Scale
</p>

<p align="center">
  Y = original video height - yPortion / Scale
</p>

The implementation of the coordinate calculation on the browser client side can be found in the files:
1. `WepApp/client/public/js/register-events.js`
2. `registerMouseEvents`

![Target Click coordinates](./TargetClickCoordinates.png)

#### Sending through RTCDataChannel:
Once the coordinates are calculated they will be sent to unity through an `RTCDataChannel` defined in the file `Peer.js`. The channel can send buffer arrays, so it is better for sending both x & y coordinates in one chunck of data instead of sending each one on a different message. The following code is responsible for sending the coordinates 
 

``` let data = new DataView(new ArrayBuffer(8),0);

    data.setFloat32(0,x,true);
    data.setFloat32(4,y,true);
 
    _videoPlayer && _videoPlayer.sendMsg(data.buffer);
```
Then, The bytes array is being parsed at the unity client with the following code:
```
  float x = BitConverter.ToSingle(bytes, 0);
  float y = BitConverter.ToSingle(bytes, 4);
```

#### Receiving the coordinates in unity:
First, the `BrowserInput` script must be added to the camera that holds the `RenderStreaming` and `BroadCast` scripts. `BrowserInput` is inherited from the render streaming class `InputChannelReceiverBase` and contains an implementation for the setChannel function that is called once an RTCDataChannel is created by the RenderStreaming Object. Now, it is possible to register to `onMessage` Action of the data channel. Once a message is received with the coordinated then it can be passed to the `ARRayCasting` object by calling `shootArrow` method.


&nbsp;
&nbsp;

>## RayCasting:
The [ray casting](https://en.wikipedia.org/wiki/Ray_casting) is a method in Graphics to shoot an object along a 3D vector with the promise that it may hit another object. The Ray will hit the object with the least [z-index](https://en.wikipedia.org/wiki/Z-order) along its direction. In this project, the browser client should be able to point on a real world object on the stream it receives. For example the browser client can click on a book and this book should be pointed to with an arrow in the unity scene. In the Prefabs folder of this project, there is an arrow prefab that will be ray casted to point to a certain object. The implmentation of Ray Cating follows the following:
1. Adding a [`ARRaycastManager`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/raycast-manager.html) to the AR Session origin object on the game scene.
2. Adding `ARRayCasting` script to the AR Session origin object which contains the implementation for the shooting a ray. In the `Awake` method the script searches for the `ARRayCastManager` component which will be used in the `shootArrow` method that is being called once a message is received from the the data channel on the Browser Input script. 
3. Providing the arrow prefab to the ARRayCasting script.

&nbsp;
&nbsp;

#### Object Detection localy on the browser client:

&nbsp;
&nbsp;


>## Web Server:
The web server follows the same implementation of the [Webserver](https://docs.unity3d.com/Packages/com.unity.renderstreaming@2.0/manual/en/webapp.html) provided with the unity render streaming package. The server consists of two parts. The first is the signaling server which is responsible for exchanging the signaling messages between the untiy client and the browser client.The second part of the web server is the browser client.

#### Signaling Server:
 The implmentation for the signaling server can be found in the file `WebApp/src`. It consists of two components.
1. `WebSocket.ts` The Websocket that is responsible for creating a websocket connection between the server and the clients and forwarding the messages depending on the message type. 
2. `Server.ts` Web server implementation that is depending on express.js server. It contains the routes for the signaling messages (/offer, /answer, /candidate).

#### Browser Client:
The impentation for the client can be found in the file `WebApp/public`. It contains the implementation for the two receiving modes either receiving video stream without annotaion or receiving the video and anotating that stream with ray casting. 

The components representing the browser client: 
1. `Peer.js` This is the impmentation for the [RTCPeerConnection](https://developer.mozilla.org/en-US/docs/Web/API/RTCPeerConnection) that handles the main functionalities of creating the peer and setting the local and remote descriptions.
2. `register-events.js` This is the component that registers to the mouse events and send the coordinates to the remote peer through the data channel.
