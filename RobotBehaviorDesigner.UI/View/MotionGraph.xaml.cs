using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using RobotBehaviourDesigner.Model;

namespace RobotBehaviourDesigner.UI.View
{
	/// <summary>
	/// Interaction logic for MotionGraph.xaml
	/// </summary>
	public partial class MotionGraph : UserControl
	{
		public MotionGraph()
		{
			InitializeComponent();
			
			SeriesCollection = new SeriesCollection
			{
				new LineSeries
				{
					Title = "Servo 1",
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 16,
					LineSmoothness = 0,
					Fill = Brushes.Transparent,
					Values = new ChartValues<ServoPoint>
					{
						new ServoPoint(0,120),
						new ServoPoint(1,160),
						new ServoPoint(2,180),
						new ServoPoint(3,100),
						new ServoPoint(4,60),
						new ServoPoint(5,60),
						new ServoPoint(6,60),
						new ServoPoint(7,60),
						new ServoPoint(8,60),
						new ServoPoint(9,80),
						new ServoPoint(10,160)
					}
				},
				new LineSeries
				{
					Title = "Servo 2",
					Fill = Brushes.Transparent,
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 16,
					LineSmoothness = 0,
					Values = new ChartValues<ServoPoint>
					{
						new ServoPoint(0,20),
						new ServoPoint(1,60),
						new ServoPoint(2,80),
						new ServoPoint(3,100),
						new ServoPoint(4,120),
						new ServoPoint(5,120),
						new ServoPoint(6,120),
						new ServoPoint(7,60),
						new ServoPoint(8,60),
						new ServoPoint(9,180),
						new ServoPoint(10,60)
					}
				}
			};
 
			Labels = new[] {"1", "2", "3", "4", "5","6","7","8","9","10"};
			YFormatter = value => value.ToString("G");
			
			CartesianMapper<ServoPoint> servoPointMapper = Mappers
				.Xy<ServoPoint>()
				.X((value, index) => index)
				.Y(value => value.Position);
 
			Charting.For<ServoPoint>(servoPointMapper);
			DataContext = this;
		}

		public SeriesCollection SeriesCollection { get; set; }
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }

		private ISeriesView _selectedSeries;
		private int _selectedPointIndex;

		private void ChartMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				_selectedSeries = null;
				_selectedPointIndex = -1;
				Mouse.OverrideCursor = null;
				return;
			}

			Mouse.OverrideCursor = Cursors.Hand;
			Point point = Chart.ConvertToChartValues(e.GetPosition(Chart));
			if(_selectedSeries!=null) SetNewValue(point);
		}

		private void ChartOnDataClick(object sender, ChartPoint point)
		{
			if(point==null) return;
			_selectedSeries = SeriesCollection.First(x => x.Title == point.SeriesView.Title);
			_selectedPointIndex = ((ServoPoint) point.Instance).Index;
		}
		
		private void SetNewValue(Point point)
		{
			if (point.Y > 180)
			{
				_selectedSeries.Values[_selectedPointIndex] = new ServoPoint(_selectedPointIndex, 180);
				return;
			}

			if (point.Y <= 0)
			{
				_selectedSeries.Values[_selectedPointIndex] = new ServoPoint(_selectedPointIndex, 0);
				return;
			} 

			_selectedSeries.Values[_selectedPointIndex] = new ServoPoint(_selectedPointIndex, point.Y);
		}
	}
}

