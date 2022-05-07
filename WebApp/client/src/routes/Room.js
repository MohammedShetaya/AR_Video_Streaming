import React , {useRef,useEffect} from "react";
import io from 'socket.io-client';

const Room = (props) => {
    
   const userVideo = useRef() ;
   const userStream = useRef() ;
   const userSocket = useRef() ;

   const peerSocketID = useRef() ;
   const peerRef = useRef() ;
   const peerVideo = useRef() ;

   useEffect(()=>{

       navigator.mediaDevices.getUserMedia({video:true}).then(stream => {

           userVideo.current.srcObject  = stream;
           userStream.current = stream;

           userSocket.current = io('/') ;
           userSocket.current.emit('join-room',"55555asca") ;
           
           userSocket.current.on('room-user' , userID => {
               console.log(userID+">> is inside the room"); 
               callUser(userID);
               peerSocketID.current = userID;
           })

           userSocket.current.on('user-joined' , userID => {
               console.log(userID+">> joined the room") ;
               peerSocketID.current = userID; 
           })

           userSocket.current.on('offer', handleRecieveCall); 

           userSocket.current.on('answer', handleAnswer);

           userSocket.current.on('ice-candidate' , handleICECandidateMsg);
       })

   });

   function callUser (userID) {
       peerRef.current = createPeer(userID); 
       userStream.current.getTracks().forEach(track => peerRef.current.addTrack(track,userStream.current));

       console.log("caller peer created")
       console.log(peerRef.current);
   }

   function createPeer(userID) {
       const peer = new RTCPeerConnection({
           iceServers: [
            {
                urls: 'stun:stun.l.google.com:19302'
            },
            {
                urls: 'turn:numb.viagenie.ca',
                credential: 'muazkh',
                username: 'webrtc@live.com'
            },
        ]
        });

        peer.onnegotiationneeded = () => handleNegotiantionNeededEvent(userID) ;  
        peer.onicecandidate = handleICECandidateEvent;
        peer.ontrack = handleTrackEvent;
        peer.onconnectionstatechange = handleConnectionChange ;

        return peer;  
    }

    function handleNegotiantionNeededEvent (userID)  {

        peerRef.current.createOffer().then( offer => {
            console.log("offer"); 
            console.log(offer);    
            return peerRef.current.setLocalDescription(offer); 
        }).then(() => {
            const offerData = {
                target: userID,
                caller: userSocket.current.id,
                sdp: peerRef.current.localDescription
            }
            userSocket.current.emit('offer', offerData); 
        }).catch(err => console.log(err));
    }

    function handleRecieveCall (offerData) {

        peerRef.current = createPeer(undefined) ;
        console.log('callee peer created');
        console.log(peerRef);
        const remoteDescription = new RTCSessionDescription(offerData.sdp) ;
        peerRef.current.setRemoteDescription(remoteDescription).then(()=>{
            userStream.current.getTracks().forEach(track => peerRef.current.addTrack(track,userStream.current));
        }).then(()=>{
            return peerRef.current.createAnswer() ;
        }).then( answer => {
            console.log("answer: " );
            console.log(answer);
            return peerRef.current.setLocalDescription(answer);
        }).then(()=>{
            const answerData = {
                target: offerData.caller,
                caller: userSocket.current.id,
                sdp: peerRef.current.localDescription
            }
            userSocket.current.emit('answer' ,answerData);
        }).catch(err => {
            console.log(err);
            console.log(offerData);
        });
         
    }


    function handleAnswer(answerData) {
        const description = new RTCSessionDescription(answerData.sdp);
        peerRef.current.setRemoteDescription(description);
    }

    function handleICECandidateEvent (e) {
        if(e.candidate) {
            const payload = {
                target: peerSocketID.current,
                candidate: e.candidate,
            }
            userSocket.current.emit("ice-candidate", payload);
        }
    }

    function handleICECandidateMsg (newIceCandidate) {
        const candidate = new RTCIceCandidate(newIceCandidate);
        peerRef.current.addIceCandidate(candidate); 
     }

    function handleTrackEvent (e) {
        console.log("Fuckkkk"); 
        console.log(e);
        peerVideo.current.srcObject = e.streams[0];
    }
    
    function handleConnectionChange (e) {
        if(peerRef.current.connectionState === 'connected'){
            console.log("connected") ;
        }
    }

    
    return (
        <>
        <div>
        <video autoPlay ref={userVideo} />
        </div>
        <hr />
        <div>
        <video autoPlay ref={peerVideo} />
        </div>
        </>   
     );
}


export default Room; 