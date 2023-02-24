namespace ET.Editor
{
    public abstract class AEntityMenuHandler
    {
        internal string menuName;

        public abstract void OnClick(Entity entity);
    }
}