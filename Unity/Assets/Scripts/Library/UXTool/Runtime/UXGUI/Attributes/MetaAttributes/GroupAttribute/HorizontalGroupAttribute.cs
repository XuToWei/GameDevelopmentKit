using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class HorizontalGroupAttribute : MetaAttribute, IGroupAttribute
	{
		private string _name;
		public string Name { get => _name ; set => _name = value; }

		public HorizontalGroupAttribute(string name = "")
		{
			Name = name;
		}
	}
}
