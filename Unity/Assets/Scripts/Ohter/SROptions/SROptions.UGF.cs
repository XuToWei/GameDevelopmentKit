//用来处理SRDebugger的调试命令

using System.ComponentModel;
using Game;

//UGF部分
public partial class SROptions
{
    [Sort(1)]
    [Category("UGF")]
    [DisplayName("GameSpeed")]
    public float UGFGameSpeed
    {
        get => GameEntry.Base.GameSpeed;
        set => GameEntry.Base.GameSpeed = value;
    }
    
    [Sort(2)]
    [Category("UGF")]
    [DisplayName("UnloadUnusedAssets")]
    public void UGFUnloadUnusedAssets()
    {
        GameEntry.Resource.UnloadUnusedAssets(true);
    }
}