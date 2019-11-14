using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#if ENABLE_WINMD_SUPPORT
using Windows.UI.Xaml;
using HoloLensForCV;
using YoloRuntime;
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

using DrawingUtils;

// https://devblogs.microsoft.com/dotnet/ml-net-and-model-builder-at-net-conf-2019-machine-learning-for-net/?utm_source=vs_developer_news&utm_medium=referral
namespace YoloDetectionHoloLens
{
    // Using the hololens for cv .winmd file for runtime support
    // https://docs.unity3d.com/2018.4/Documentation/Manual/IL2CPP-WindowsRuntimeSupport.html
    public class YoloDetection : MonoBehaviour
    {
        #region UnityVariables
        // Gesture recognizer
        private GestureRecognizer _gestureRecognizer;

        // Texture handler for bounding boxes
        public DrawBoundingBoxes drawBoundingBoxes;

        // Parameters for host connect
        // https://stackoverflow.com/questions/32876966/how-to-get-local-host-name-in-c-sharp-on-a-windows-10-universal-app
        // Connecting to desktop host IP, not the hololens... Get the IP of PC and retry with specified port 
        public string ipAddressForConnect = "142.76.25.8";
        public string hostId = "12345";

        public Text myText;
        int _tapCount;

        // From Tiny YOLO string labels.
        private string[] _labels = {
            "aeroplane", "bicycle", "bird", "boat", "bottle",
            "bus", "car", "cat", "chair", "cow",
            "diningtable", "dog", "horse", "motorbike", "person",
            "pottedplant", "sheep", "sofa", "train", "tvmonitor"
        };

        private bool _holoLensMediaFrameSourceGroupStarted;

        public enum SensorTypeUnity
        {
            Undefined = -1,
            PhotoVideo = 0,
            ShortThrowToFDepth = 1,
            ShortThrowToFReflectivity = 2,
            LongThrowToFDepth = 3,
            LongThrowToFReflectivity = 4,
            VisibleLightLeftLeft = 5,
            VisibleLightLeftFront = 6,
            VisibleLightRightFront = 7,
            VisibleLightRightRight = 8,
            NumberOfSensorTypes = 9
        }
        public SensorTypeUnity sensorTypePv;
        #endregion

#if ENABLE_WINMD_SUPPORT
        // Required for media frame source initialization
        private MediaFrameSourceGroupType _selectedMediaFrameSourceGroupType = MediaFrameSourceGroupType.PhotoVideoCamera;
        private SensorFrameStreamer _sensorFrameStreamer;
        private SpatialPerception _spatialPerception;
        private MediaFrameSourceGroup _holoLensMediaFrameSourceGroup;
        private SensorType _sensorType;
#endif

        #region UnityMethods
        // Use this for initialization
        async void Start()
        {
            // Create the gesture handler
            InitializeHandler();

            // Initialize the bounding box canvas
            drawBoundingBoxes.InitDrawBoundingBoxes();

            // Wait for media frame source groups to be initialized
            await StartHoloLensMediaFrameSourceGroup();
        }

        async void OnApplicationQuit()
        {
            await StopHoloLensMediaFrameSourceGroup();
        }

        #endregion

        /// <summary>
        /// Initialize and start the hololens media frame source groups
        /// </summary>
        /// <returns>Task result</returns>
        async Task StartHoloLensMediaFrameSourceGroup()
        {
#if ENABLE_WINMD_SUPPORT
            // Plugin doesn't work in the Unity editor
            myText.text = "Initalizing MediaFrameSourceGroup.";

            Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up sensor frame streamer");
            _sensorType = (SensorType)sensorTypePv;
            _sensorFrameStreamer = new SensorFrameStreamer();
            _sensorFrameStreamer.Enable(_sensorType);

            Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up spatial perception");
            _spatialPerception = new SpatialPerception();

            Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up the media frame source group");
            _holoLensMediaFrameSourceGroup = new MediaFrameSourceGroup(
                _selectedMediaFrameSourceGroupType,
                _spatialPerception,
                _sensorFrameStreamer);
            _holoLensMediaFrameSourceGroup.Enable(_sensorType);

            Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Starting the media frame source group");
            await _holoLensMediaFrameSourceGroup.StartAsync();
            _holoLensMediaFrameSourceGroupStarted = true;

            myText.text = "MediaFrameSourceGroup started. Once desktop client is connected, double tap to connect to host socket.";

#endif
        }

        async Task StopHoloLensMediaFrameSourceGroup()
        {
#if ENABLE_WINMD_SUPPORT
            if (_holoLensMediaFrameSourceGroup == null || 
                !_holoLensMediaFrameSourceGroupStarted)
            {
                return;
            }

            await _holoLensMediaFrameSourceGroup.StopAsync();
            _holoLensMediaFrameSourceGroup = null;
            _sensorFrameStreamer = null;
            _holoLensMediaFrameSourceGroupStarted = false;
#endif
        }

        /// <summary>
        /// Connect to the desktop client and begin receiving 
        /// bounding box information.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ConnectSocket()
        {
#if ENABLE_WINMD_SUPPORT

            myText.text = "Connecting to host socket.";

            Debug.Log("Connecting to host socket.");

            // c++ call to connect to socket, handlers now in c++ code
            InteropDeviceReceiver interopDeviceReceiver
                = new InteropDeviceReceiver();

            // Connect to provided IP address and host port
            interopDeviceReceiver.ConnectSocket_Click(ipAddressForConnect, hostId);

            // Loop indefinitely
            while (true)
            {
                // Get new updated bounding box results
                var dataBuffer = interopDeviceReceiver.GetBoundingBoxResults();

                List<BoundingBox> boundingBoxes =
                    new List<BoundingBox>();

                // Iterate across data buffer, which is the total number of 
                // elements in the buffer
                const int boxSize = 6;
                var textString = "";
                if (dataBuffer.Count != 0)
                {
                    var numBoxes = (int)(dataBuffer.Count / (float)boxSize);

                    for (var boxCount = 0; boxCount < numBoxes; boxCount++)
                    {
                        BoundingBox box = new BoundingBox
                        {
                            TopLabel = dataBuffer[(boxCount * boxSize) + 0], // TopLabel is int
                            X = dataBuffer[(boxCount * boxSize) + 1],
                            Y = dataBuffer[(boxCount * boxSize) + 2],

                            Height = dataBuffer[(boxCount * boxSize) + 3],
                            Width = dataBuffer[(boxCount * boxSize) + 4],

                            Confidence = dataBuffer[(boxCount * boxSize) + 5]
                        };

                        // Set top label from the label string by index.
                        box.Label = _labels[box.TopLabel];

                        // Add the filled box to list
                        boundingBoxes.Add(box);
                    }

                    // Draw the list of boxes
                    drawBoundingBoxes.DrawBoxes(boundingBoxes);

                    // Debug the text string outside of loop
                    myText.text = $"Received {boundingBoxes.Count} bounding boxes.";
                    yield return new WaitForSeconds(0.05f);
                    Debug.Log(textString);
                }
                else
                {
                    // Draw the list of empty boxes to clear
                    // prior elements
                    boundingBoxes.Add(new BoundingBox()
                    {
                        Confidence = 0,
                        Label = "",
                        Height = 0,
                        Width = 0,
                        X = 0,
                        Y = 0});
                    drawBoundingBoxes.DrawBoxes(boundingBoxes);

                    myText.text = "No bounding boxes received.";
                    yield return new WaitForSeconds(0.05f);
                }
            }
#endif
            yield return new WaitForSeconds(0.05f);
        }

        #region TapGestureHandler
        private void InitializeHandler()
        {
            // New recognizer class
            _gestureRecognizer = new GestureRecognizer();

            // Set tap as a recognizable gesture
            _gestureRecognizer.SetRecognizableGestures(GestureSettings.DoubleTap);

            // Begin listening for gestures
            _gestureRecognizer.StartCapturingGestures();

            // Capture on gesture events with delegate handler
            _gestureRecognizer.Tapped += GestureRecognizer_Tapped;

            Debug.Log("Gesture recognizer initialized.");
        }

        public void GestureRecognizer_Tapped(TappedEventArgs obj)
        {
            // Connect to socket on tapped event
            _tapCount += obj.tapCount;

            Debug.LogFormat("OnTappedEvent: tapCount = {0}", _tapCount);
            StartCoroutine(ConnectSocket());
        }

        void CloseHandler()
        {
            _gestureRecognizer.StopCapturingGestures();
            _gestureRecognizer.Dispose();
        }
        #endregion
    }
}



