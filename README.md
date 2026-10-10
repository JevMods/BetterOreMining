# BetterOreMining

This mod adds a ripple that outlines the ore around you on each hit, even when it's buried. No ore left behind!

Short clips are in [DEMONSTRATION.md](https://github.com/JevMods/BetterOreMining/blob/main/DEMONSTRATION.md).

## Features

- Hitting the ground, a rock or ore sends a glowing ring across all ore within 8 meters, through the ground too.
- The ring wraps the outer shape of the vein and the walls you've already dug out.
- Green means the ore is at or above the ground, red means it's buried 2 meters or more.
- Works on every ore, plus scrap iron, copper scrap, soft tissue and obsidian.

## Configuration

The settings are in `BepInEx/config/JevMods.BetterOreMining.cfg`, created on first launch.

- `Ripple.Radius`: how far the ripple spreads from where you hit, in meters (default 8).
- `Ripple.WaveSpeed`: how fast it spreads, in meters per second (default 4.5).
- `Ripple.RingWidth`: how thick the ring is, in meters (default 2).
- `Ripple.Opacity`: how strong the ring is at its brightest, from 0 to 1 (default 0.8).
- `Colors.Visible`: ring color on ore at or above the ground (default green, `33FF59FF`).
- `Colors.Blocked`: ring color on ore buried 2 meters or deeper (default red, `FF261AFF`). Ore in between gets a mix of the two.

Colors are RGBA hex values written as `RRGGBBAA`, for example `FF0000FF` for solid red. The last pair is the alpha, but the ring takes its strength from `Ripple.Opacity`.

## Feedback

Found a bug or have an idea for a change? Open an issue on the [GitHub issues page](https://github.com/JevMods/BetterOreMining/issues). I'll go through everything as fast as I can.

## Building from source

To build you need Windows, the .NET SDK 8 or newer (with the .NET Framework 4.8 developer pack), Valheim and BepInEx. The easiest way to get BepInEx is to install BepInExPack_Valheim in a Thunderstore Mod Manager profile.

```
git clone https://github.com/JevMods/BetterOreMining.git
cd BetterOreMining
dotnet build src/BetterOreMining -c Release
```

Then copy `src/BetterOreMining/bin/Release/BetterOreMining.dll` and the `src/BetterOreMining/bin/Release/assets` folder into a `BepInEx/plugins/BetterOreMining` folder.

The build looks for Valheim through Steam and for BepInEx in the Default profile of the Thunderstore Mod Manager. If yours live elsewhere, pass the paths:

```
dotnet build src/BetterOreMining -c Release -p:ValheimDir="D:\Games\Valheim" -p:BepInExDir="D:\Games\Valheim\BepInEx"
```

You can also set the `VALHEIM_INSTALL` environment variable. With no Valheim install at all, point `ManagedDir` at the `valheim_server_Data/Managed` folder of the free Valheim Dedicated Server (Steam app 896660). The GitHub workflow does exactly that.

The ripple shader comes prebuilt in `src/BetterOreMining/assets`, and its source is in `shaders`. To rebuild it, make a Unity 6000.0.75f1 project, put `Ripple.shader` in its `Assets` folder and `BuildBundle.cs` in `Assets/Editor`, set the `BUNDLE_OUT` environment variable to an output folder and run:

```
Unity -batchmode -quit -projectPath <project> -buildTarget Win64 -executeMethod BuildBundle.Build
```

## Notes

- Client-side: only the player who wants the ripple needs to install it.
- Source: https://github.com/JevMods/BetterOreMining
