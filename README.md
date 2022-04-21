# AR_Video_Streaming

## Table of Content:
 - [Overview](#overview)
 - [Dependencies](#dependencies)

## Overview

This Project aims to apply remote guidance using Augmented Reality between the browser and the android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser, then the browser can annotate that stream with AR features. The project is based on the  [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) And [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) packages.


## Dependencies

### [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
Allows real-time, peer-to-peer, media exchange between two devices. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address. The solution for this is to use a signaling server.

The signaling server acts as an interface between the Unity Android application and the browser clients so they can Start sending signaling messages to each other. Using an HTTP server in this case would not help, so it is optimal to use a WebSocket server. WebSocket connection is statefull (FullDuplex) unlike the HTTP connection where the server can not send responses to the client unless the client sends a request. Websocket servers can send and receive requests at any moment in the connection lifetime. In the case of WebRTC, the server will never know when a client will send a signaling message so it can be forwarded to the other client.

#### The singnaling process:
1. The browser client sends and offer message to the websocket server
