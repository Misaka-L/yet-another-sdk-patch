using System;
using System.Net;

namespace YetAnotherPatchForVRChatSdk.Patches.NetworkResilience;

internal sealed class FollowSystemWebProxy : IWebProxy
{
    public Uri GetProxy(Uri destination)
    {
        return WebRequest.GetSystemWebProxy().GetProxy(destination);
    }

    public bool IsBypassed(Uri host)
    {
        return WebRequest.GetSystemWebProxy().IsBypassed(host);
    }

    public ICredentials Credentials { get; set; } = CredentialCache.DefaultCredentials;
}