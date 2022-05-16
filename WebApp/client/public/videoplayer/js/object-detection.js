let net ;

export default async function runModel (video,canvas) {
  net = await cocoSsd.load();
  console.log("Handpose model loaded.");
  //  Loop and detect hands
  setInterval(() => {
    detect(video,canvas);
  }, 10);
};

async function detect (video,canvas) {
  // Check data is available
  if (
    typeof video !== "undefined" &&
    video !== null &&
    video.readyState === 4
  ) {
     // Get Video Properties
     const videoWidth = video.videoWidth;
     const videoHeight = video.videoHeight;

     // Set video width
     video.width = videoWidth;
     video.height = videoHeight;

     // Set canvas height and width
     canvas.width = videoWidth;
     canvas.height = videoHeight;

     // Make Detections
     const obj = await net.detect(video);
     console.log(obj) ;
     // Draw mesh
     const ctx = canvas.getContext("2d");
     drawRect(obj, ctx);
  }
}


function drawRect (detections, ctx) {
  // Loop through each prediction
  detections.forEach(prediction => {

    // Extract boxes and classes
    const [x, y, width, height] = prediction['bbox'];
    const text = prediction['class'];

    // Set styling
    const color = Math.floor(Math.random()*16777215).toString(16);
    ctx.strokeStyle = '#' + color;
    ctx.font = '25px Arial';

    // Draw rectangles and text
    ctx.beginPath();
    ctx.fillStyle = '#' + color;
    ctx.fillText(text, x, y);
    ctx.rect(x, y, width, height);
    ctx.stroke();
  });
}
