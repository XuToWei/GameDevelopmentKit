#if UNITY_EDITOR

public abstract class AssetsCheckerBase
{
	public abstract bool Check(string folederPath);
}

public class AssetsCheckerSample :AssetsCheckerBase
{
	public override bool Check(string folederPath)
    {
        return true;
    }
}
#endif
