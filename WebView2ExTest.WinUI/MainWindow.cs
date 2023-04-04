// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using WebView2Ex;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebView2ExTest.WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : MicaWindow
{
    public MainWindow()
    {
        var wv2 = new WebView2ExSimplified(this);
        Content = wv2;
        ExtendsContentIntoTitleBar = true;
        wv2.RuntimeInitialized += delegate
        {
            WebView2Runtime w = wv2.WebView2Runtime;
            w.Controller.DefaultBackgroundColor =
#if NonWinRTWebView2
            System.Drawing.Color.Transparent
#else
            Microsoft.UI.Colors.Transparent
#endif
            ;
        };
    }
}

// Simplified API
partial class WebView2ExSimplified : WebView2Ex.UI.WebView2Ex
{
public WebView2ExSimplified(Window Window)
{
    // Assuming we are on core window only
    SetWindow(Window);

    InitializeAsync(Window);
}
async void InitializeAsync(Window Window)
{
    // Assuming we create our own runtime
    WebView2Runtime = await WebView2Runtime.CreateAsync(await WebView2Environment.CreateAsync(), WindowNative.GetWindowHandle(Window));
    WebView2Runtime.CoreWebView2!.Navigate(InitialUri);
    RuntimeInitialized?.Invoke();
}
public event Action RuntimeInitialized;
// I'm lazy to implement normal observable property for Uri,
// so I create one for initial Uri for simplicity.
// You can technically do this by subscribing to WebView2Runtime.CoreWebView2.SourceChanged event
[ObservableProperty]
string _InitialUri = "about:blank";
}