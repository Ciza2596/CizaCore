using CizaCore.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropdownExample : MonoBehaviour
{
    [SerializeField]
    private Dropdown _dropdown;

    [Button]
    private void Show() =>
        _dropdown.Show();
    
    [Button]
    private void MoveToUp() =>
        _dropdown.MoveToUp();

    [Button]
    private void MoveToDown() =>
        _dropdown.MoveToDown();
}