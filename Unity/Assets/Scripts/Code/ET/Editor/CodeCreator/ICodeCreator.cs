namespace ET.Editor
{
    internal interface ICodeCreator
    {
        void OnEnable();
        void OnGUI();
        void GenerateCode(string codeName);
    }
}
