using System.Reflection;

public static class HostExtensions
{
    public static IHost AddWindowsServiceInstaller(this IHost host, string? serviceName = null)
    {
        var env = host.Services.GetRequiredService<IHostEnvironment>();
        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        var assemblyName = Path.GetFileNameWithoutExtension(appAssembly.Location);
        serviceName ??= (appAssembly.GetName().Name ?? env.ApplicationName).Replace(".", "_");
        var dir = Path.GetDirectoryName(appAssembly.Location) ?? throw new NullReferenceException("No directory for assembly path");
        var installPath = Path.Combine(dir, "install.bat");
        var uninstallPath = Path.Combine(dir, "uninstall.bat");

        if (File.Exists(installPath))
        {
            return host;
        }

        var installTemplate = $@"@echo off
setlocal

set ServiceName=""{serviceName}""
set ServiceExe=%~dp0{assemblyName}.exe

:: Create the service
sc create %ServiceName% binPath= %ServiceExe% start= auto

:: Configure failure and recovery options
sc failure %ServiceName% reset= 86400 actions= restart/60000/restart/60000/restart/60000

:: Start the service
sc start %ServiceName%

:: Display a message indicating the status
sc query %ServiceName%

endlocal";
        File.WriteAllText(installPath, installTemplate);

        var uninstallTemplate = $@"@echo off
sc stop ""{serviceName}""
timeout 5 > NUL
sc delete ""{serviceName}""
";
        File.WriteAllText(uninstallPath, uninstallTemplate);

        return host;
    }
}