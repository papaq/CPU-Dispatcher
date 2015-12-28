using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CpuDispatcher
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph
    {
        public Graph(int graph)
        {
            InitializeComponent();
            switch (graph)
            {
                case 0: BuildGraph1();
                    break;
                case 1: BuildGraph2();
                    break;
                case 2: BuildGraph1_2();
                    break;
                default:
                    BuildGraph3();
                    break;
            }
        }

        private void BuildGraph1()
        {
            BuildBothAxes();

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };

            // Tick time = 50 ms
            var points = MakeCollectionOfAverageTimeDependency(50, 300);

            var polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[0],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 250 ms
            points = MakeCollectionOfAverageTimeDependency(250, 300);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[1],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 1000 ms
            points = MakeCollectionOfAverageTimeDependency(1000, 300);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[2],
                Points = points
            };
            CanGraph.Children.Add(polyline);
        }

        private void BuildGraph2()
        {
            BuildBothAxes();

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };

            // Tick time = 50 ms
            var points = MakeCollectionOfSysIdleDependency(50);

            var polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[0],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 250 ms
            points = MakeCollectionOfSysIdleDependency(250);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[1],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 1000 ms
            points = MakeCollectionOfSysIdleDependency(1000);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[2],
                Points = points
            };
            CanGraph.Children.Add(polyline);
        }

        private void BuildGraph1_2()
        {
            BuildBothAxes();

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };

            // Tick time = 1000 ms
            var points = MakeCollectionOfAverageTimeDependency(1000, 900);

            var polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[2],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 1000 ms
            points = MakeCollectionOfSysIdleDependency(1000);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[2],
                Points = points
            };
            CanGraph.Children.Add(polyline);
        }

        private void BuildGraph3()
        {
            BuildBothAxes();

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };

            // Tick time = 50 ms
            var points = MakeCollectionOfRatio(50);

            var polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[0],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 250 ms
            points = MakeCollectionOfRatio(250);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[1],
                Points = points
            };
            CanGraph.Children.Add(polyline);

            // Tick time = 1000 ms
            points = MakeCollectionOfRatio(1000);

            polyline = new Polyline
            {
                StrokeThickness = 1,
                Stroke = brushes[2],
                Points = points
            };
            CanGraph.Children.Add(polyline);
        }

        private PointCollection MakeCollectionOfAverageTimeDependency(int tick, int n)
        {
            var points = new PointCollection();
            for (var i = 1; i <= n; i++)
            {
                var dispatcher = new Dispatcher()
                {
                    FreqFrom = i,
                    FreqTo = i,
                    Tick = tick,
                    WeightFrom = 250,
                    WeightTo = 250,
                };
                points.Add(new Point(i, dispatcher.CountAverageTime(50)));
            }

            return ConvertPointColl(points);
        }

        private PointCollection MakeCollectionOfSysIdleDependency(int tick)
        {
            var points = new PointCollection();
            for (var i = 1; i <= 900; i++)
            {
                var dispatcher = new Dispatcher()
                {
                    FreqFrom = i,
                    FreqTo = i,
                    Tick = tick,
                    WeightFrom = 250,
                    WeightTo = 250,
                };
                points.Add(new Point(i, dispatcher.CountSystemIdlePart(50)));
            }

            return ConvertPointColl(points);
        }

        private PointCollection MakeCollectionOfRatio(int tick)
        {
            var points = new PointCollection();
            for (var i = 1; i <= 1000; i++)
            {
                var dispatcher = new Dispatcher()
                {
                    FreqFrom = 500,
                    FreqTo = 500,
                    Tick = tick,
                    WeightFrom = i,
                    WeightTo = i,
                };
                points.Add(dispatcher.CountRatio_WaitingTasks_To_AvTimeWaiting(50));
            }

            return ConvertPointColl(points);
        }

        private PointCollection ConvertPointColl(PointCollection pointCollection)
        {
            var maxY = pointCollection[0].Y;
            maxY = pointCollection.Select(point => point.Y).Concat(new[] { maxY }).Max();
            var maxX = pointCollection[0].X;
            maxX = pointCollection.Select(point => point.X).Concat(new[] { maxX }).Max();

            var scaleX = CanGraph.Width / maxX;
            var scaleY = CanGraph.Height / maxY;

            for (var i = 0; i < pointCollection.Count; i++)
                pointCollection[i] = new Point(pointCollection[i].X * scaleX, CanGraph.Height - pointCollection[i].Y * scaleY);

            return pointCollection;
        }

        private void BuildBothAxes()
        {
            const double margin = 10;
            const double xmin = margin;
            var ymax = CanGraph.Height - margin;
            const double step = 10;

            // Make the X axis.
            var xaxisGeom = new GeometryGroup();
            xaxisGeom.Children.Add(new LineGeometry(
                        new Point(0, ymax), new Point(CanGraph.Width, ymax)));
            for (var x = xmin + step; x <= CanGraph.Width - step; x += step)
            {
                xaxisGeom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            var xaxisPath = new Path
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Data = xaxisGeom
            };

            CanGraph.Children.Add(xaxisPath);

            // Make the Y ayis.
            var yaxisGeom = new GeometryGroup();
            yaxisGeom.Children.Add(new LineGeometry(
                            new Point(xmin, 0), new Point(xmin, CanGraph.Height)));
            for (var y = step; y <= CanGraph.Height - step; y += step)
            {
                yaxisGeom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            var yaxisPath = new Path
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Data = yaxisGeom
            };

            CanGraph.Children.Add(yaxisPath);
        }
    }
}
