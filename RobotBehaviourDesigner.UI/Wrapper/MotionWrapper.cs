using RobotBehaviourDesigner.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotBehaviourDesigner.UI.Wrapper
{
	public class MotionWrapper : ModelWrapper<Motion>
	{
		private readonly string[] _takenNames;

		public MotionWrapper(Motion model, string[] takenNames) : base(model)
		{
			_takenNames = takenNames;
		}

		public string Id => Model.Id;

		public string Name
		{
			get => GetValue<string>();
			set => SetValue(value);
		}

		protected override IEnumerable<string> ValidateProperty(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Name):
					if (_takenNames.Contains(Name))
						yield return "This name already exists, duplications not allowed";
					break;
			}
		}
	}
}