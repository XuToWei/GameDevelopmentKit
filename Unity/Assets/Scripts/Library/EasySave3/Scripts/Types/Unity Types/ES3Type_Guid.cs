using System;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("value")]
	public class ES3Type_Guid : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3Type_Guid() : base(typeof(Guid))
		{
			Instance = this;
		}

		public override void Write(object obj, ES3Writer writer)
		{
			Guid casted = (Guid)obj;
			writer.WriteProperty("value", casted.ToString(), ES3Type_string.Instance);
		}

		public override object Read<T>(ES3Reader reader)
		{
			return Guid.Parse(reader.ReadProperty<string>(ES3Type_string.Instance));
		}
	}

	public class ES3Type_GuidArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_GuidArray() : base(typeof(Guid[]), ES3Type_Guid.Instance)
		{
			Instance = this;
		}
	}
}