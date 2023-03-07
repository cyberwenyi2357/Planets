using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;

using NUI3D;

namespace T5_BodilyInteraction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor = null;

        // ---------------------------------------------
        // depth frame
        DepthFrameManager depthFrameManager = null;

        // ---------------------------------------------
        // color frame
        ColorFrameManager colorFrameManger = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("window loaded");

            sensor = KinectSensor.GetDefault(); // get the default Kinect sensor 
            if (sensor == null) return;
            
            // depth stream 
            depthFrameManager = new DepthFrameManager();
            depthFrameManager.Init(sensor, depthImg);

            // color stream 
            colorFrameManger = new ColorFrameManager();
            colorFrameManger.Init(sensor, colorImg);

            sensor.Open();
            depthFrameManager.SetMainWindow(this);
            depthFrameManager.SetPlanetImage1(murcury);
            depthFrameManager.SetPlanetImage2(venus);
            depthFrameManager.SetPlanetImage3(mars);
            depthFrameManager.SetPlanetImage4(jupiter);
            depthFrameManager.SetPlanetImage5(saturn);
            depthFrameManager.SetPlanetImage6(uraImg);
            depthFrameManager.SetPlanetImage7(neptune);
            depthFrameManager.SetIntro(introduction);
            //depthFrameManager.SetBugImage(bugImg);
            //depthFrameManager.SetUraImage(uraImg);

        }

        private void introduction_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
