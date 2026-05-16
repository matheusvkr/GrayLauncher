using Client.Core.Services;
using Client.Shared;
using Hexa.NET.KittyUI;
using Hexa.NET.KittyUI.OpenAL;
using Hexa.NET.KittyUI.UI;

namespace Client;

class Program
{
    static void Main()
    {
        var app = AppBuilder.Create()
            .WithOpenAL()
            .EnableLogging(false)
            .EnableDebugTools(false)
            .StyleColorsDark()
            .Build();

        app.SetTitle("Gray Client")
            .AddWindow<View.Windows.MainWindow>()
            .UseTitleBar<TitleBar>()
            .Run();
    }

}
