using RobotBehaviourDesigner.Model;
using System;
using System.Collections.Generic;

namespace RobotBehaviourDesigner.UI.Wrapper
{
	public class MotionWrapper : ModelWrapper<Motion>
	{
		public MotionWrapper(Motion model) : base(model)
		{
		}

		public string Id => Model.Id.ToString();

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
					if (string.Equals(Name, "Robot", StringComparison.OrdinalIgnoreCase))
						yield return "Robots are not valid servos";
					break;
			}
		}
	}
}