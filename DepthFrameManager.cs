using System.Windows.Media.Imaging;

using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Drawing;
using System;
using System.Windows.Controls;
using System.Media;
//this part below refers to Lecture4-5 inlass exercise
namespace NUI3D
{
    class DepthFrameManager
    {
        private KinectSensor sensor;

        private FrameDescription depthFrameDescription = null;
        private ushort[] depthData = null;
        private int bytesPerPixel = 4;
        private byte[] depthPixels = null;
        private WriteableBitmap depthImageBitmap = null;
        private System.Windows.Controls.Image wpfImage = null;
        private Boolean interacts = false;
        private String[] net=new String[7];
        private String[] temp = new String[7];
        public float dBody;
        private String net1;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        //public 
        private byte[] bodyIndexData = null;
        System.Drawing.Color[] hl = new System.Drawing.Color[7];
        
        private CircleF[] cir = new CircleF[7];
        private Random rand = new Random(); // need using System;

        // class exercise 2: 3D hit test
        private float[] cirDepth = new float[7];
        //cirDepth[0] = 0.6f,
        private float avgDepth = 0,  DepthNep=1.2f, DepthUra=0.8f;
        private int count = 0;
        // to interact with an WPF image 
        
        private System.Windows.Controls.Image uranus = null;
        private T5_BodilyInteraction.MainWindow mainWin = null;
        private System.Windows.Controls.Image[] planets = new System.Windows.Controls.Image[7];
        private System.Windows.Controls.TextBox intro = null;
        private String[] intr = new String[7];
public void SetIntro(System.Windows.Controls.TextBox introduction){
            intro = introduction;
        }
        public void SetPlanetImage1(System.Windows.Controls.Image img)
        {
            planets[0] = img;
        }
        public void SetPlanetImage2(System.Windows.Controls.Image img)
        {
            planets[1] = img;
        }
        public void SetPlanetImage3(System.Windows.Controls.Image img)
        {
            planets[2] = img;
        }
        public void SetPlanetImage4(System.Windows.Controls.Image img)
        {
            planets[3] = img;
        }
        public void SetPlanetImage5(System.Windows.Controls.Image img)
        {
            planets[4] = img;
        }
        public void SetPlanetImage6(System.Windows.Controls.Image img)
        {
            planets[5] = img;
        }
        public void SetPlanetImage7(System.Windows.Controls.Image img)
        {
            planets[6] = img;
        }
        public void SetMainWindow(T5_BodilyInteraction.MainWindow win)
        {
            mainWin = win;
        }


        public void Init(KinectSensor s, System.Windows.Controls.Image wpfImageForDisplay)
        {
            sensor = s;
            wpfImage = wpfImageForDisplay;
            //wpfImage1 = wpfImageForDisplay;
            DepthFrameReaderInit();
            net1 = "";
            for (int i=0;i<=6;i++){
                net[i] = "";
            }
             
            BodyIndexFrameReaderInit();
            
        }
        //this part below is my own contribution
        public void CircleInit(){
            hl[0] = System.Drawing.Color.Black;
            hl[1] = System.Drawing.Color.Black;
            hl[2] = System.Drawing.Color.Black;
            hl[3] = System.Drawing.Color.Black;
            hl[4] = System.Drawing.Color.Black;
            hl[5] = System.Drawing.Color.Black;
            hl[6] = System.Drawing.Color.Black;
            cir[0].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[0])+planets[0].ActualWidth/2-5.2), (float)(Canvas.GetTop(planets[0]) + planets[0].ActualHeight / 2-4.5));
            cir[1].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[1]) + planets[1].ActualWidth / 2 - 10.2), (float)(Canvas.GetTop(planets[1]) + planets[1].ActualHeight / 2 - 6.5));
            cir[2].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[2]) + planets[2].ActualWidth / 2 - 23), (float)(Canvas.GetTop(planets[2]) + planets[2].ActualHeight / 2 - 13));
            cir[3].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[3]) + planets[3].ActualWidth / 2 - 33.5), (float)(Canvas.GetTop(planets[3]) + planets[3].ActualHeight / 2 - 8));
            cir[4].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[4]) + planets[4].ActualWidth / 2 - 44), (float)(Canvas.GetTop(planets[4]) + planets[4].ActualHeight / 2 - 13));
            cir[5].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[5]) + planets[5].ActualWidth / 2 - 55), (float)(Canvas.GetTop(planets[5]) + planets[5].ActualHeight / 2 - 5));
            cir[6].Center = new System.Drawing.PointF((float)(Canvas.GetLeft(planets[6]) + planets[6].ActualWidth / 2 - 62), (float)(Canvas.GetTop(planets[6]) + planets[6].ActualHeight / 2 - 12));
            cirDepth[0] = (float)(0.3); // depth in [1, 3)
            cirDepth[1] = (float)(0.2);
            cirDepth[2] = (float)(0.3);
            cirDepth[3] = (float)(0.4);
            cirDepth[4] = (float)(0.5);
            cirDepth[5] = (float)(0.6);
            cirDepth[6] = (float)(0.7);
            cir[0].Radius = 18; // size in (15, 50]; the farther, the smaller
            cir[1].Radius = 25; // size in (15, 50]; the farther, the smaller
            cir[2].Radius = 35;
            cir[3].Radius = 43.5f;
            cir[4].Radius = 27;
            cir[5].Radius = 36;
            cir[6].Radius = 27;
            temp[0] = "Mercury";
            temp[1] = "Venus";
            temp[2] = "Mars";
            temp[3] = "Jupiter";
            temp[4] = "Saturn";
            temp[5] = "Uranus";
            temp[6] = "Neptune";
            intr[0] = "Mercury is the smallest planet in the solar system and the closest to the Sun. It has a thin atmosphere, a heavily cratered surface, and rocky terrain. The planet's surface is extremely hot during the day and extremely cold at night. Mercury has a large iron core and has been visited by two NASA spacecraft.";
            intr[1] = "Venus is the second planet from the Sun and is Earth's closest planetary neighbor. It's one of the four inner, terrestrial (or rocky) planets, and it's often called Earth's twin because it's similar in size and density. These are not identical twins, however – there are radical differences between the two worlds.";
            intr[2] = "Mars is the fourth planet from the Sun – a dusty, cold, desert world with a very thin atmosphere. Mars is also a dynamic planet with seasons, polar ice caps, canyons, extinct volcanoes, and evidence that it was even more active in the past.";
            intr[3] = "Jupiter is the fifth planet from our Sun and is, by far, the largest planet in the solar system – more than twice as massive as all the other planets combined. Jupiter's stripes and swirls are actually cold, windy clouds of ammonia and water, floating in an atmosphere of hydrogen and helium.";
            intr[4] = "Saturn is the sixth planet from the Sun and the second-largest planet in our solar system. Like fellow gas giant Jupiter, Saturn is a massive ball made mostly of hydrogen and helium. Saturn is not the only planet to have rings, but none are as spectacular or as complex as Saturn's. Saturn also has dozens of moons.";
            intr[5] = "Uranus is the seventh planet from the Sun, and has the third-largest diameter in our solar system. It was the first planet found with the aid of a telescope, Uranus was discovered in 1781 by astronomer William Herschel, although he originally thought it was either a comet or a star.";
            intr[6] = "Neptune is one of two ice giants in the outer solar system (the other is Uranus). Most (80% or more) of the planet's mass is made up of a hot dense fluid of icy materials – water, methane, and ammonia – above a small, rocky core. Of the giant planets, Neptune is the densest.";
        }
        private void BodyIndexFrameReaderInit() // call it in Init() 
        {
            // Open the reader for body index frames 
            BodyIndexFrameReader bodyIndexFrameReader = sensor.BodyIndexFrameSource.OpenReader();

            bodyIndexFrameReader.FrameArrived += BodyIndexFrameReader_FrameArrived;

            // Body index frame has the same resolution as the depth frame 
            // Each pixel is represented by an 8-bit unsigned integer 
            bodyIndexData = new byte[sensor.DepthFrameSource.FrameDescription.LengthInPixels];
        }

        private void BodyIndexFrameReader_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            using (BodyIndexFrame bodyIndexFrame =
                    e.FrameReference.AcquireFrame())
            {
                if (bodyIndexFrame == null) return;

                bodyIndexFrame.CopyFrameDataToArray(
bodyIndexData);
            }
        }

        private void DepthFrameReaderInit()
        {
            // Open the reader for the depth frames
            DepthFrameReader depthFrameReader = sensor.DepthFrameSource.OpenReader();

            // register an event handler for FrameArrived 
            depthFrameReader.FrameArrived += DepthFrameReader_FrameArrived;

            // allocate storage for depth data
            depthFrameDescription = sensor.DepthFrameSource.FrameDescription;
            // 16 - bit unsigned integer per pixel
            depthData = new ushort[depthFrameDescription.LengthInPixels];

            // initialization for displaying depth data
            // to associate 4-byte color for each pixel             
            depthPixels = new byte[depthFrameDescription.LengthInPixels * bytesPerPixel];

            depthImageBitmap = new WriteableBitmap(
                                       depthFrameDescription.Width, // 512 
                                       depthFrameDescription.Height, // 424
                                       96, 96, PixelFormats.Bgr32, null);
            wpfImage.Source = depthImageBitmap;
            //wpfImage1.Source = depthImageBitmap;
        }

        private void DepthFrameReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            // using statement automatically takes care of disposing of 
            // the DepthFrame object when you are done using it
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame == null) return;

                // DepthVisualization(depthFrame);        

                // class exercise 2
                // depthData is needed for calculating the average depth of each contour 
                depthFrame.CopyFrameDataToArray(depthData);
                CircleInit();
                // class exercise 1 
                // System.Drawing.Bitmap bmp = BodySegmentation();
                System.Drawing.Bitmap bmp = HandSegmentation();
                if (bmp != null) ContourDetectionAndVisualization(bmp);
                
            }
        }

        private void DepthVisualization(DepthFrame depthFrame)
        {
            depthFrame.CopyFrameDataToArray(depthData);
            
            // depthData --> depthPixels 
            for (int i = 0; i < depthData.Length; ++i)
            {
                if (bodyIndexData[i] != 255) // non-background 
                {
                    //if(depthData<dBody-offset)
                    depthPixels[bytesPerPixel * i] = 255;
                    depthPixels[bytesPerPixel * i + 1] = 0;
                    depthPixels[bytesPerPixel * i + 2] = 0;
                }
                else
                {
                    ushort depth = depthData[i];

                    ushort minDepth = depthFrame.DepthMinReliableDistance; // 500 
                    ushort maxDepth = depthFrame.DepthMaxReliableDistance; // 4500 

                    byte depthByte = (byte)((depth - minDepth) * 255.0 / (maxDepth - minDepth));
                    depthByte = (byte)(255 - depthByte);

                    depthPixels[bytesPerPixel * i] = depthByte;
                    depthPixels[bytesPerPixel * i + 1] = depthByte;
                    depthPixels[bytesPerPixel * i + 2] = depthByte;
                }
            }

            depthImageBitmap.WritePixels(
              new Int32Rect(0, 0, depthFrameDescription.Width, depthFrameDescription.Height),
              depthPixels, // BGR32 pixel data
                           // stride: width in bytes of a single row of pixel data
              depthFrameDescription.Width * bytesPerPixel, 0);
        }

        private System.Drawing.Bitmap BodySegmentation() // to be called in DepthFrameReader_FrameArrived
        {
            
            for (int i = 0; i < bodyIndexData.Length; ++i)
            {
                ushort depth = depthData[i];
                if (bodyIndexData[i] != 255) // player exists in space 
                {
                    depthPixels[bytesPerPixel * i] = 255;
                    depthPixels[bytesPerPixel * i + 1] = 255;
                    depthPixels[bytesPerPixel * i + 2] = 255;
                }
                else
                {
                    depthPixels[bytesPerPixel * i] = 0;
                    depthPixels[bytesPerPixel * i + 1] = 0;
                    depthPixels[bytesPerPixel * i + 2] = 0;
                }
            }
            BitmapSource bmpSrc = BitmapSource.Create(depthFrameDescription.Width, depthFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null,
                        depthPixels, depthFrameDescription.Width * 4);
            // BitmapSource -> Bitmap 
            return ImageHelpers.ToBitmap(bmpSrc);
        }
        
        private void ContourDetectionAndVisualization(System.Drawing.Bitmap bmp)
        {
            if (bmp == null) return;

            // code adapted from T4_BlobDetection 
            // Bitmap -> Image (using the extension method from Emgu.CV.BitmapExtension) 
            Image<Bgr, byte> openCVImg = bmp.ToImage<Bgr, byte>();

            // convert from Bgr to gray 
            Image<Gray, byte> grayImg = openCVImg.Convert<Gray, byte>();

            // Image processing using Emgu CV 
            // contour detection and visualization 
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(grayImg, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple); // outer contours only

                for (int i = 0; i < contours.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    // filter out contours of small size
                    if ( area <8*10 || area> 100*100) continue;

                    openCVImg.Draw(contours, i, new Bgr(System.Drawing.Color.Blue), 2); ;
                    System.Drawing.Rectangle aabb = CvInvoke.BoundingRectangle(contours[i]);
                    
                    //openCVImg.Draw(aabb, new Bgr(System.Drawing.Color.Blue), 2);

                    // display the depth information 
                    avgDepth = CalculateAverageDepth(grayImg, contours[i]) / 1000;

                    String s = String.Format("avg depth: {0:0.0}", avgDepth);
                    openCVImg.Draw(s, new System.Drawing.Point(aabb.X, aabb.Y), new FontFace(), 0.5,
                     new Bgr(System.Drawing.Color.Yellow));
                    
                    // check if the circle center is inside the contour or not
                    for (int a =0; a<=6;a++){
                        if (CvInvoke.PointPolygonTest(contours[i], cir[a].Center, false) > 0)
                        {
                            if (Math.Abs((dBody/1000-avgDepth)*1.4- cirDepth[a]) < 0.2)
                            {
                                // 0.2 is a threshold 
                                HighlightCircle(a);
                                //count++;
                                
                            }
                           
                        }
                        
                    }
                    

                }
            }
            openCVImg.Draw(net[0], new System.Drawing.Point(26,78), new FontFace(), 0.5,new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[1], new System.Drawing.Point(64, 136), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[2], new System.Drawing.Point(165, 242), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[3], new System.Drawing.Point(232, 150), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[4], new System.Drawing.Point(318, 269), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[5], new System.Drawing.Point(399, 69), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            openCVImg.Draw(net[6], new System.Drawing.Point(440, 237), new FontFace(), 0.5, new Bgr(System.Drawing.Color.Yellow));
            // draw a circle 
            for (int i=0;i<=6;i++){
                openCVImg.Draw(cir[i], new Bgr(hl[i]), 0 /* fill up */);
            }
          

            openCVImg.Draw(net1, new System.Drawing.Point(10, 40), new FontFace(), 0.5,
                        new Bgr(System.Drawing.Color.Yellow));
            // display the processed image 
            // Image -> Bitmap -> BitmapSource
            bmp = openCVImg.ToBitmap<Bgr, byte>(); // extension method
  
            wpfImage.Source = ImageHelpers.ToBitmapSource(bmp);
            //wpfImage1.Source = ImageHelpers.ToBitmapSource(bmp);
        }

        private void UpdateCircle(float sY=2)
        {
            
                float newY = cir[0].Center.Y ;
                float newX = cir[0].Center.X;
        }

        private void HighlightCircle(int i)
        {
            net[i] =temp[i];
            hl[i] = System.Drawing.Color.Yellow;
            intro.Text = intr[i];
            SoundPlayer player = new SoundPlayer();
            player = new System.Media.SoundPlayer(@"C:\Users\WENYi\Downloads\Breath of the Wild.wav");
            //player.Play();
        }
        /*private void StopCircle1(int i)
        {
            
            net1 = "Murcury," +
                " is the first planet from the Sun and the nearest known planet in the Solar System.";
            hl[1] = System.Drawing.Color.Yellow;

        }*/
        // Calculate the average depth for all the depth pixels within a given contour. 
        // You should make sure depthData contains the most updated depth data.
        private float CalculateAverageDepth(Image<Gray, byte> binaryImg, IInputArray contour)
        {
            float avgDepth = 0;
            int count = 0;
            System.Drawing.Rectangle aabb = CvInvoke.BoundingRectangle(contour);
           //CircleF center = new CircleF(new System.Drawing.PointF(aabb., (int)Canvas.GetTop(bug)), 10);
            for (int col = aabb.Left; col < aabb.Right; col++)
                for (int row = aabb.Top; row < aabb.Bottom; row++)
                {
                    byte pixel = binaryImg.Data[row, col, 0]; // get corresponding pixel 
                    if (pixel == 255) // white
                    {
                        avgDepth += depthData[row * depthFrameDescription.Width + col];
                        count++;
                    }
                }
            if (count != 0) return avgDepth / count;
            else return 0;
        }


        private float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return (value - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        }

        private System.Drawing.Bitmap HandSegmentation()
        {
            // estimate the depth of human body                         
            // estimate the depth of human body                         
            dBody = 0;
            int playerPixelNumber = 0; // for Attempt 2; class exercise 3           

            for (int i = 0; i < depthData.Length; ++i)
            {
                if (bodyIndexData[i] != 255) // player                
                {
                    // Attempt 1:
                    // dBody = maximum depth of player segmentation data 
                    //if (depthData[i] > dBody)
                    //    dBody = depthData[i];

                    // Attempt 2:
                    // dBody = average depth of player segmentation
                    dBody += depthData[i];
                    playerPixelNumber++;
                }
            }

            // Attempt 2: class exercise 3           
            // dBody = average depth of player segmentation data
            dBody /= playerPixelNumber;

            float offset = 200; // mm 

            for (int i = 0; i < depthData.Length; ++i)
            {
                ushort depth = depthData[i];

                if (bodyIndexData[i] != 255)//player
                {
                    if (depth < dBody - offset) {
                        // highlight hand segmentation in white 
                        depthPixels[4 * i] =255;
                        depthPixels[4 * i + 1] = 255;
                        depthPixels[4 * i + 2] = 255;
                    }
                    else // non-hand
                    {
                        depthPixels[4 * i] = 0;
                        depthPixels[4 * i + 1] = 0;
                        depthPixels[4 * i + 2] = 0;
                    }
                }
                else
                {
                    depthPixels[4 * i] = 0;
                    depthPixels[4 * i + 1] = 0;
                    depthPixels[4 * i + 2] = 0;
                }
            }

            BitmapSource bmpSrc = BitmapSource.Create(depthFrameDescription.Width, depthFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null,
                    depthPixels, depthFrameDescription.Width * 4);
            // BitmapSource -> Bitmap 
            return ImageHelpers.ToBitmap(bmpSrc);
        }
    }
}
