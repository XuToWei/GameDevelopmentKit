namespace StateController
{
    public class SateActive : BaseBooleanLogicState
    {
        protected override void OnSateChanged(bool logicResult)
        {
            gameObject.SetActive(logicResult);
        }
    }
}
