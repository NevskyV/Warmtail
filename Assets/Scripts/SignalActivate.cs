using UnityEngine;

public class SignalActivate : MonoBehaviour
{
    public GameObject Logo;
    public GameObject Menu;

    public void SetUIActive(bool isActive)
    {
        if (Logo != null)
            Logo.SetActive(isActive);

        if (Menu != null)
            Menu.SetActive(isActive);
    }
}