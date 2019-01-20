﻿using System;
using Avalonia;
using Avalonia.Gtk3;
using Avalonia.Platform;
using Splat;
using Tel.Egram.Application;
using Tel.Egram.Views.Application;
using Tel.Egram.Model.Application;

namespace Tel.Egram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureServices(Locator.CurrentMutable);
            Run(Locator.Current);
        }

        private static void ConfigureServices(
            IMutableDependencyResolver services)
        {
            services.AddUtils();
            services.AddTdLib();
            services.AddPersistance();
            services.AddServices();
            
            services.AddComponents();
            services.AddApplication();
            services.AddAuthentication();
            services.AddWorkspace();
            services.AddSettings();
            services.AddMessenger();
        }

        private static void Run(
            IDependencyResolver resolver)
        {
            var app = resolver.GetService<MainApplication>();
            var builder = AppBuilder.Configure(app);
            var os = builder.RuntimePlatform.GetRuntimeInfo().OperatingSystem;
            var model = new MainWindowModel();
            
            if (os == OperatingSystemType.OSX)
            {
                builder.UseAvaloniaNative(null, opt =>
                {
                    opt.MacOptions.ShowInDock = true;
                    opt.UseDeferredRendering = true;
                    opt.UseGpu = true;
                }).UseSkia();
            }
            else if (os == OperatingSystemType.Linux)
            {
                builder.UseGtk3(new Gtk3PlatformOptions
                {
                    UseDeferredRendering = true,
                    UseGpuAcceleration = true
                }).UseSkia();
            }
            else
            {
                builder.UseWin32(
                    deferredRendering: true
                ).UseSkia();
            }

            builder.UseReactiveUI();

            model.Activator.Activate();
            builder.Start<MainWindow>(() => model);
            model.Activator.Deactivate();
        }
    }
}