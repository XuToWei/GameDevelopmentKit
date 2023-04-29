using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[ResponseType(nameof(S2C_CanBuild))]
	[Message(ToolMessage.C2S_CanBuild)]
	[ProtoContract]
	public partial class C2S_CanBuild: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		public override void Dispose()
		{
			base.Dispose();
			RpcId = default;
		}
	}

	[Message(ToolMessage.S2C_CanBuild)]
	[ProtoContract]
	public partial class S2C_CanBuild: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public bool CanBuild { get; set; }

		public override void Dispose()
		{
			base.Dispose();
			RpcId = default;
			Error = default;
			Message = default;
			CanBuild = default;
		}
	}

	[Message(ToolMessage.S2C_TipInfo)]
	[ProtoContract]
	public partial class S2C_TipInfo: ProtoObject, IMessage
	{
		[ProtoMember(1)]
		public string Info { get; set; }

		public override void Dispose()
		{
			base.Dispose();
			Info = default;
		}
	}

	public static class ToolMessage
	{
		 public const ushort C2S_CanBuild = 32002;
		 public const ushort S2C_CanBuild = 32003;
		 public const ushort S2C_TipInfo = 32004;
	}
}
