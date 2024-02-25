using UnityEngine;
public class Reddot : MonoBehaviour
{
    [SerializeField]
    private string path;
    public string Path
    {
        get { return path; }
        set
        {
            UnRegisterReddot();
            path = value;
            if (bAwaked)
            {
                RegisterReddot();
            }
        }
    }

    [SerializeField]
    public GameObject reddotFlag;

    private bool bAwaked;

    private void Awake()
    {
        bAwaked = true;
        RegisterReddot();
        ReddotManager.RefreshShown(path);
    }

    private void OnDestroy()
    {
        UnRegisterReddot();
    }

    public void SetReddotShow(bool isShown)
    {
        reddotFlag?.SetActive(isShown);
    }

    private void RegisterReddot()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        ReddotManager.RegisterRedDotUI(this);
    }

    private void UnRegisterReddot()
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        ReddotManager.UnRegisterRedDotUI(this);
    }
}