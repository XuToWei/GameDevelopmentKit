using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class BoxGroupAttribute : MetaAttribute, IGroupAttribute
	{
        private string _name;
        public string Name { get => _name; set => _name = value; }

        public BoxGroupAttribute(string name = "")
		{
			Name = name;
		}
	}
}
