using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace RobotBehaviourDesigner.UI.View
{
	/// <summary>
	/// Interaction logic for ChartToolTip.xaml
	/// </summary>
	public partial class ChartToolTip : IChartTooltip
	{
		public ChartToolTip()
		{
			InitializeComponent();
			DataContext = this;
		}

		private TooltipData _data;


		public event PropertyChangedEventHandler PropertyChanged;

		public TooltipData Data
		{
			get => _data;
			set
			{
				_data = value;
				OnPropertyChanged("Data");
			}
		}

		public TooltipSelectionMode? SelectionMode { get; set; }

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
