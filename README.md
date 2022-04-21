# AR_Video_Streaming

## Table of Content:
 - [Overview](#overview)
 - [Dependencies](#dependencies)

## Overview

This Project aims to apply remote guidance using Augmented Reality between the browser and the android phone. The android application streams an [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html) scene to the browser, then the browser can annotate that stream with AR features. The project is based on the  [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html) And [UnityRenderStreaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/index.html) packages.


## Dependencies

1. [WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html):
allows real-time, peer-to-peer, media exchange between two devices. A connection is established through a discovery and negotiation process called signaling. The signaling between two peers is not supported by WebRTC protocol because every peer is connecting to the Internet behind a [NAT](https://en.wikipedia.org/wiki/Network_address_translation) so each peer has no information about his public IP address. The solution for this is to use a signaling server.
