using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.Kinect.Face;
using WindowsPreview.Kinect;
using System.Threading;
using Windows.Storage;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KinectEmoji
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        KinectSensor _sensor = null;
        ColorFrameReader _colorReader = null;
        BodyFrameReader _bodyReader = null;
        IList<Body> _bodies = null;

        // 1) Specify a face frame source and a face frame reader
        FaceFrameSource _normalFaceSource = null;
        FaceFrameReader _normalFaceReader = null;

        // from HD
        private HighDefinitionFaceFrameSource _hdFaceSource = null;
        private HighDefinitionFaceFrameReader _hdFaceReader = null;
        private FaceAlignment _faceAlignment = null;
        private FaceModel _faceModel = null;
        private List<Ellipse> _points = new List<Ellipse>();

        // data
        private FaceData _faceData = null;
        
        public MainPage()
        {
            this.InitializeComponent();
            _sensor = KinectSensor.GetDefault();
            _faceData = new FaceData();

            if (_sensor != null)
            {
                _sensor.Open();

                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

                //_colorReader = _sensor.ColorFrameSource.OpenReader();
                //_colorReader.FrameArrived += ColorReader_FrameArrived;
                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                // 2) Initialize the face source with the desired features

                // specify the required face frame results
                
                FaceFrameFeatures faceFrameFeatures =
                    FaceFrameFeatures.BoundingBoxInColorSpace
                    | FaceFrameFeatures.PointsInColorSpace
                    | FaceFrameFeatures.RotationOrientation
                    | FaceFrameFeatures.FaceEngagement
                    | FaceFrameFeatures.Glasses
                    | FaceFrameFeatures.Happy
                    | FaceFrameFeatures.LeftEyeClosed
                    | FaceFrameFeatures.RightEyeClosed
                    | FaceFrameFeatures.LookingAway
                    | FaceFrameFeatures.MouthMoved
                    | FaceFrameFeatures.MouthOpen;

                _normalFaceSource = new FaceFrameSource(_sensor, 0, faceFrameFeatures);
                _normalFaceReader = _normalFaceSource.OpenReader();
                _normalFaceReader.FrameArrived += NormalFaceReader_FrameArrived;
                

                // from HD
                _hdFaceSource = new HighDefinitionFaceFrameSource(_sensor);

                _hdFaceReader = _hdFaceSource.OpenReader();
                _hdFaceReader.FrameArrived += HDFaceReader_FrameArrived;

                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();
            }

            //tmp canvas
            /*
            // Add a Line Element
            var myLine = new Line();
            // myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.Stroke = new SolidColorBrush(Colors.Blue);
            //myLine.Stroke = 

            myLine.X1 = 100;
            myLine.X2 = 150;
            myLine.Y1 = 100;
            myLine.Y2 = 150;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            canvasHD.Children.Add(myLine);
            */
            // tmp
            write_log("llllllllllooooooooooooooooooonnnnnnnnnnnnnnnnnnnnnggggggggggggggg");
            write_log(FaceHD.MouthUpperlipMidbottom.ToString());
            var face = new FaceHD();
            write_log(face.dump_str());

            emoji.Source = Emoji.none;

            var autoEvent = new AutoResetEvent(false);

            //tmp.Text = "tmp";
            var stateTimer = new Timer(tmp_callback, autoEvent, 1000, 1000);
            //var stateTimer = new Timer(tmp_callback);
            //var timer = new System.Timers.Timer(1000);

        }

        //int tmp_count = 0;
        public void tmp_callback(Object stateInfo)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        () =>
        {
        // Your UI update code goes here!
        //tmp.Text = String.Format("{0}\n", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff tt"));
            //tmp.Text += _faceData.dump_str();
        }
        );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            
        }

        public void Record_Button_Click(object sender, RoutedEventArgs e)
        {
            //tmp.Text = "click";
            save();
        }

        public async void save()
        {
            String json_str = _faceData.json(tags.Text);
            //sysLog.Text = json_str;
            //String json_str = "QQ";
            String folder_name = "KinectFrameData";
            String file_name = String.Format(@"{0}_{1}.txt", tags.Text, DateTime.Now.Ticks);

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(json_str);
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dataFolder = await local.CreateFolderAsync(folder_name, CreationCollisionOption.OpenIfExists);

            // Create a new file named DataFile.txt.
            var file = await dataFolder.CreateFileAsync(file_name, CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }

            //string[] lines = { "aa", "bb" };
            //StreamWriter w = new StreamWriter
            //System.IO.Path
            //string path = "QQ";
            //StreamWriter w = new StreamWriter("qq.txt", false);
            //StreamWriter file = File.CreateText(@"C:\Users\Public\test.txt");

        }

        private void write_log(String s)
        {
            //sysLog.Text += '\n';
            //sysLog.Text += s;
            //sysLogViewer.ScrollToBottom();     
        }

        void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    camera.Source = frame.ToBitmap();
                }
            }
        }

        void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    
                    if (!_normalFaceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            // 4) Assign a tracking ID to the face source
                            _normalFaceSource.TrackingId = body.TrackingId;
                        }
                    }
                    

                    if (!_hdFaceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            _hdFaceSource.TrackingId = body.TrackingId;
                        }
                    }
                }
            }
        }

        void NormalFaceReader_FrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // 4) Get the face frame result
                    FaceFrameResult result = frame.FaceFrameResult;

                    if (result != null)
                    {
                        // 5) Do magic!
                        var f = new FaceNormal(result);
                        _faceData.addNormalData(f);
                        infoNormal.Text = f.dump_str();
                        infoNormal.Text += _faceData.dump_str();

                        // Get the face points, mapped in the color space.

                        var eyeLeft = result.FacePointsInColorSpace[FacePointType.EyeLeft];
                        var eyeRight = result.FacePointsInColorSpace[FacePointType.EyeRight];

                        // Position the canvas UI elements
                        Canvas.SetLeft(ellipseEyeLeft, eyeLeft.X - ellipseEyeLeft.Width / 2.0);
                        Canvas.SetTop(ellipseEyeLeft, eyeLeft.Y - ellipseEyeLeft.Height / 2.0);

                        Canvas.SetLeft(ellipseEyeRight, eyeRight.X - ellipseEyeRight.Width / 2.0);
                        Canvas.SetTop(ellipseEyeRight, eyeRight.Y - ellipseEyeRight.Height / 2.0);

                        // Display or hide the ellipses
                        if (f.eyeLeftClosed == DetectionResult.Yes || f.eyeLeftClosed == DetectionResult.Maybe)
                        {
                            ellipseEyeLeft.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ellipseEyeLeft.Visibility = Visibility.Visible;
                        }

                        if (f.eyeRightClosed == DetectionResult.Yes || f.eyeRightClosed == DetectionResult.Maybe)
                        {
                            ellipseEyeRight.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ellipseEyeRight.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void HDFaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null && frame.IsFaceTracked)
                {
                    frame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    UpdateFacePoints();
                    UpdateEmoji();
                }
            }
        }

        private void UpdateFacePoints()
        {
            if (_faceModel == null) return;

            var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);

            if (vertices.Count > 0)
            {
                if (_points.Count == 0)
                {
                    for (int index = 0; index < vertices.Count; index++)
                    {
                        var mycolor = Colors.Blue;
                        var mywidth = 2.0;
                        var myheight = 2.0;

                        //if (index == 91 || index == 687 || index == 19 || index == 1072 || index == 10 || index == 8) {
                        if (FaceHD.isMouthPoint(index))
                        {
                            mycolor = Colors.Red;
                            mywidth = 20.0;
                            myheight = 20.0;
                        }
                        else if (index == FaceHD.LeftcheekCenter || index == FaceHD.RightcheekCenter)
                        {
                            mycolor = Colors.Green;
                            mywidth = 20.0;
                            myheight = 20.0;
                        } /*else if (index == FaceHD.NoseBottom)
                        {
                            mycolor = Colors.Black;
                            mywidth = 20.0;
                            myheight = 20.0;
                        }*/
                            /*else if (Face.isLeftEyePoint(index)) {
                            mycolor = Colors.Green;
                            mywidth = 20.0;
                            myheight = 20.0;
                        } else if (Face.isRightEyePoint(index)) {
                            mycolor = Colors.Purple;
                            mywidth = 20.0;
                            myheight = 20.0;
                        }*/

                        Ellipse ellipse = new Ellipse
                        {
                            Width = mywidth,
                            Height = myheight,
                            Fill = new SolidColorBrush(mycolor)
                        };

                        _points.Add(ellipse);
                    }

                    foreach (Ellipse ellipse in _points)
                    {
                        canvas.Children.Add(ellipse);
                        //canvasHD.Children.Add(ellipse);

                    }
                }

                var face = new FaceHD();
               

                for (int index = 0; index < vertices.Count; index++)
                //for (int i = 0; i < _target_points.Length; i++)
                {
                    //var index = _target_points[i];
                    CameraSpacePoint vertice = vertices[index];
                    //DepthSpacePoint d_point = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice);
                    ColorSpacePoint point = _sensor.CoordinateMapper.MapCameraPointToColorSpace(vertice);

                    if (float.IsInfinity(point.X) || float.IsInfinity(point.Y)) return;

                    Ellipse ellipse = _points[index];

                    Canvas.SetLeft(ellipse, point.X);
                    Canvas.SetTop(ellipse, point.Y);

                    face.addData(index, vertice.X, vertice.Y, vertice.Z);
                }

                //write_log(face.dump_str());
                _faceData.addHDData(face);
                info.Text = face.dump_str();
                info.Text += _faceData.dump_str();
            }
        }

        private void UpdateEmoji()
        {
            if (_faceData.isShakeHead())
            {
                emoji.Source = Emoji.no;
            }
            else if (_faceData.isNodHead())
            {
                emoji.Source = Emoji.yes;
            } else if (_faceData.isSwayHead())
            {
                emoji.Source = Emoji.question;
            } else if (_faceData.isLeftEyeClosed() && _faceData.isRightEyeClosed() && _faceData.isMouthOpen())
            {
                emoji.Source = Emoji.tired;
            }
            else if (_faceData.isHappy())
            {
                emoji.Source = Emoji.happy;
            }
            else if (_faceData.isWink())
            {
                emoji.Source = Emoji.wink;
            }
            else if (_faceData.isMouthOpen())
            {
                emoji.Source = Emoji.shock;
            }
            else if (_faceData.isLeftEyeClosed() && _faceData.isRightEyeClosed() && !_faceData.isHappy() && !_faceData.isMouthOpen())
            {
                emoji.Source = Emoji.annoyed;
            }
            else
            {
                emoji.Source = null;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_colorReader != null)
            {
                _colorReader.Dispose();
                _colorReader = null;
            }

            if (_bodyReader != null)
            {
                _bodyReader.Dispose();
                _bodyReader = null;
            }

            if (_normalFaceReader != null)
            {
                _normalFaceReader.Dispose();
                _normalFaceReader = null;
            }

            if (_normalFaceSource != null)
            {
                _normalFaceSource = null;
            }

            if (_faceModel != null)
            {
                _faceModel.Dispose();
                _faceModel = null;
            }

            GC.SuppressFinalize(this);

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }
    }
}
