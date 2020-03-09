using RadarSimulator;
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

namespace WPFTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, Color> trackableObjects = new Dictionary<int, Color>();
        private Random rand;
        public MainWindow()
        {
            rand = new Random(5);
            InitializeComponent();
            RadarSimulator.RadarSimulator.Instance.RadarEvent += RadarEventReceived;
            Task.Run(() => RadarSimulator.RadarSimulator.Instance.Start());
        }

        private void RadarEventReceived(object sender, TrackableObject e)
        {
            if (e != null)
            {
                Color color;
                if (!trackableObjects.TryGetValue(e.Id, out color))
                {
                    color = Color.FromRgb((byte)rand.Next(0, 256), (byte)rand.Next(0, 256), (byte)rand.Next(0, 256));
                    trackableObjects.Add(e.Id, color);
                }

                DrawObject(e, color);
            }
        }

        private void DrawObject(TrackableObject obj, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                
                var textBlock = CreateObjectUI(color, obj.Id.ToString());

                RemoveObjectFromCanvas(obj);

                AddObjectToCanvas(obj, textBlock);

            });
        }

        private void AddObjectToCanvas(TrackableObject c, TextBlock t)
        {
            if (float.IsNaN(c.Location.X) || float.IsNaN(c.Location.Y)) return;
            MainCanvas.Children.Add(t);
            Canvas.SetLeft(t, c.Location.X * 100 + 500);
            Canvas.SetTop(t, c.Location.Y * 100 + 500);
        }

        private void RemoveObjectFromCanvas(TrackableObject obj)
        {
            if (trackableObjects.TryGetValue(obj.Id, out var color))
            {
                var e = (UIElement)LogicalTreeHelper.FindLogicalNode(MainCanvas, "e" + obj.Id.ToString());
                MainCanvas.Children.Remove(e);
                e = null;
            }
        }

        private static TextBlock CreateObjectUI(Color color, string id)
        {
            var ellipse = new Ellipse();
            ellipse.Width = 20;
            ellipse.Height = 20;
            ellipse.Fill = new SolidColorBrush() { Color = color };

            var t = new TextBlock();
            t.Text = id;
            t.Name = "e" + id;
            t.Width = 20;
            t.Height = 20;

            t.Foreground = new SolidColorBrush() { Color = Color.FromRgb((byte)255, (byte)255, (byte)255) };
            t.TextAlignment = TextAlignment.Center;
            var v = new VisualBrush();
            v.Visual = ellipse;
            t.Background = v;

            return t;
        }
    }
}
