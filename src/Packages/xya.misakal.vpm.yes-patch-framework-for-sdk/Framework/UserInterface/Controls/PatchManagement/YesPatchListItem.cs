using UnityEditor;
using UnityEngine.UIElements;
using YesPatchFrameworkForVRChatSdk.PatchManagement;

namespace YesPatchFrameworkForVRChatSdk.UserInterface.Controls.PatchManagement;

internal sealed class YesPatchListItem : VisualElement
{
    private readonly Label patchIdLabel;
    private readonly Label patchDisplayNameLabel;
    
    public YesPatchListItem(YesPatch yesPatch, YesPatchManager yesPatchManager)
    {
        var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/xyz.misakal.vpm.yes-patch-framework-for-sdk/Framework/UserInterface/Controls/PatchManagement/" +
            nameof(YesPatchListItem) + ".uxml");
        tree.CloneTree(this);

        patchIdLabel = this.Q<Label>("patch-id");
        patchDisplayNameLabel = this.Q<Label>("patch-display-name");
        
        patchIdLabel.text = yesPatch.Id;
        patchDisplayNameLabel.text = yesPatch.DisplayName;
    }
}