using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ConfirmationDialog _confirmationDialogPrefab;
    [SerializeField] private EditBuilding _editBuildingPrefab;

    Dictionary<RTSMenus, DialogBase> _dialogInstances = new();

    Stack<DialogBase> _dialogStack = new Stack<DialogBase>();
    Dictionary<RTSMenus, DialogBase> _disabledDialogs = new();

    private int _topSortingOrder = 0;
    private const int _sortOrderGap = 10;

    public void ShowDialog(RTSMenus dialogType)
    {
        PushDialog(dialogType);
    }

    public void PushDialog(RTSMenus dialogType)
    {
        if (!_dialogInstances.ContainsKey(dialogType))
        {
            DialogBase created = null;
            switch (dialogType)
            {
                case RTSMenus.ConfirmationDialog:
                    created = CreateDialogFromPrefab(_confirmationDialogPrefab);
                    break;
            }
            if (created == null)
            {
                Debug.LogError($"Could not created dialog from prefab: {dialogType}");
            }
            else
            {
                _dialogInstances.Add(dialogType, created);
            }
        }
        DialogBase instance = _dialogInstances[dialogType];
        if (_dialogStack.Contains(instance))
        {
            Debug.LogError($"Dialog is already pushed: {dialogType}");
        }
        else
        {
            if (_disabledDialogs.ContainsKey(dialogType))
            {
                _disabledDialogs.Remove(dialogType);
            }
            _dialogStack.Push(instance);
            instance.gameObject.SetActive(true);
            _topSortingOrder += _sortOrderGap;
            instance.DialogCanvas.sortingOrder = _topSortingOrder;
        }
    }

    private DialogBase CreateDialogFromPrefab(DialogBase dialogPrefab)
    {
        return Instantiate(dialogPrefab, transform);
    }

    public void HideDialog(RTSMenus dialogType)
    {
        PopDialog(dialogType);
    }

    private void PopDialog(RTSMenus dialogType)
    {
        if (!_dialogInstances.ContainsKey(dialogType))
        {
            Debug.LogError($"Tried to pop dialog, but dialog was never created {dialogType}");
            return;
        }
        DialogBase instance = _dialogInstances[dialogType];

        if (_dialogStack.TryPeek(out DialogBase topDialogPeek))
        {
            if (topDialogPeek == instance)
            {
                DialogBase topDialog = _dialogStack.Pop();
                topDialog.gameObject.SetActive(false);
                _disabledDialogs.Add(topDialog.MenuType(), topDialog);
                _topSortingOrder -= _sortOrderGap;
            }
            else
            {
                Debug.LogError($"Tried to pop the dialog type {dialogType} but the top dialog was {topDialogPeek.MenuType()}");
            }
        }
        else
        {
            Debug.LogError($"Failed to peek the top dialog");
        }
    }
}
