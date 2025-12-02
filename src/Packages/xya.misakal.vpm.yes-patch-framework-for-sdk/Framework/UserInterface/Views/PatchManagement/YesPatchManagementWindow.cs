using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using YesPatchFrameworkForVRChatSdk.PatchManagement;
using YesPatchFrameworkForVRChatSdk.UserInterface.Controls.PatchManagement;

public class YesPatchManagementWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

    private readonly YesPatchManager _yesPatchManager;

    public YesPatchManagementWindow()
    {
        _yesPatchManager = YesPatchManager.Instance;
    }

    [MenuItem("Window/Yes! Patch Framework/Patch Management")]
    public static void ShowExample()
    {
        YesPatchManagementWindow wnd = GetWindow<YesPatchManagementWindow>();
        wnd.titleContent = new GUIContent("Patch Management");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(root);

        var patchesListView = root.Q<ListView>("patches-list-view");

        var patches = _yesPatchManager.Patches.ToList();
        patchesListView.makeItem = () => new VisualElement();
        patchesListView.bindItem = (view, index) =>
        {
            var item = new YesPatchListItem(patches[index], _yesPatchManager);
            view.Add(item);
        };

        patchesListView.fixedItemHeight = 50;
        patchesListView.itemsSource = patches;
    }
}