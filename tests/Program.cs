global using System.Drawing;
using Calendar231;
using System.Text.Json;

if (OperatingSystem.IsWindows() || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_DATA_HOME")) || Directory.Exists(AppSettings.DataFolder))
    throw new Exception("Run on Linux with XDG_DATA_HOME set to an empty temporary test directory. Never run against a real profile.");
var count = 0;
void Check(bool ok, string message) { count++; if (!ok) throw new Exception(message); }
var settings = new AppSettings();
foreach (var percent in Enumerable.Range(0,96))
{
    settings.TransparencyPercent=percent;
    Check(settings.TransparencyPercent == percent, "Transparency percentage drift at " + percent);
    Check(settings.Transparency is >= 13 and <= 255, "Invisible or invalid alpha");
}
settings.TransparencyPercent=-100;
Check(settings.TransparencyPercent==0,"Lower bound");
settings.TransparencyPercent=200;
Check(settings.TransparencyPercent==95,"Upper bound");
settings.WinW=900; settings.WinH=700; settings.WinX=-900; settings.PanelCorner="TopLeft";
settings.BehindDesktopIcons=true; settings.EdgeShadowLeftPercent=15;
var roundTrip=JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(settings))!;
Check(roundTrip.WinW==900 && roundTrip.WinX==-900 && roundTrip.PanelCorner=="TopLeft" && roundTrip.BehindDesktopIcons && roundTrip.EdgeShadowLeftPercent==15,"Settings fields lost");
Check(!JsonSerializer.Serialize(settings).Contains("TransparencyPercent"),"Derived setting must not be serialized");
Check(AppSettings.DataFolder.EndsWith("231Calendar"),"User data isolation");
Check(AppSettings.WebViewDataFolder.StartsWith(AppSettings.DataFolder+Path.DirectorySeparatorChar),"Browser data isolation");
Check(AppSettings.IsFirstRun,"Expected clean test profile");
settings.Save();
Check(!AppSettings.IsFirstRun,"Settings were not persisted");
Check(AppSettings.Load().WinW==900,"Settings reload");
File.WriteAllText(Path.Combine(AppSettings.DataFolder,"settings.json"),"not json");
Check(AppSettings.Load().WinW==0,"Corrupt settings recovery");
Check(Theme.FromBackground(Color.Black).IsDark,"Dark theme");
Check(!Theme.FromBackground(Color.White).IsDark,"Light theme");
foreach(var text in new string?[] {null,"","#GGGGGG","1234567","#123","#12345678"})
    Check(Theme.FromHex(text)==null,"Malformed theme accepted");
foreach(var rgb in new[] { Color.FromArgb(32,33,36),Color.White,Color.FromArgb(100,180,225) })
{
    var theme=Theme.FromBackground(rgb);
    Check(Theme.FromHex(theme.ToHex())==theme,"Theme roundtrip");
    Check(theme.Back.A==255,"Theme alpha");
}
Directory.Delete(AppSettings.DataFolder,true);
Console.WriteLine("PASS: "+count+" assertions. Settings, transparency bounds, persistence, corruption recovery, isolated profile and theme roundtrips.");
