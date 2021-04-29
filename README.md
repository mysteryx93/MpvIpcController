# MpvIpcController
Provides remote control of MPV video player via IPC protocol via .NET

### How to Use

    var factory = new MpvApiFactory();
    // var api = await factory.ConnectAsync("MpvPipeName");
    var api = await factory.StartAsync("mpv.exe", "MpvPipeName");
    await api.LoadFileAsync("video.mp4");

MpvApi contains all commands, properties and options. MpvApi.Controller provides the raw communication protocols.

### Debugging

For development, you can set

    api.Controller.LogEnabled = true;

You can then view the full communication log using

    api.Controller.Log.ToString();

### Options

A series of options can be set for each API message. It can be set in two ways. You can pass ApiOptions to the request, or you can set MpvApi.Controller.DefaultOptions

### License

This library is under [MIT license](https://github.com/mysteryx93/MpvIpcController/blob/master/LICENSE). You can use it freely.

Library provided by Etienne Charland
