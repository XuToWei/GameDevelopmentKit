using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(ToolMessage.RemoteBuilderMessage)]
	[ProtoContract]
	public partial class RemoteBuilderMessage: ProtoObject
	{
		[ProtoMember(1)]
		public List<string> Realms { get; set; }

		[ProtoMember(2)]
		public List<string> Routers { get; set; }

		public override void Dispose()
		{
			base.Dispose();
			Realms?.Clear();
			Routers?.Clear();
		}
	}

	public static class ToolMessage
	{
		 public const ushort RemoteBuilderMessage = 32002;
	}
}
