using TrueSync;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class XunLuoPathComponent: Entity, IAwake
    {
        public TSVector[] path = new TSVector[] { new TSVector(0, 0, 0), new TSVector(20, 0, 0), new TSVector(20, 0, 20), new TSVector(0, 0, 20), };
        public int Index;
    }
}