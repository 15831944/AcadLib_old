﻿namespace AcadLib.Statistic
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using AutoCAD_PIK_Manager.Settings;
    using Autodesk.AutoCAD.ApplicationServices.Core;
    using Autodesk.AutoCAD.DatabaseServices;
    using JetBrains.Annotations;
    using Model.Statistic.DataSetStatisticTableAdapters;
    using NetLib;
    using Yandex.Metrica;
    using General = AcadLib.General;

    [PublicAPI]
    public static class PluginStatisticsHelper
    {
        private static string _app;
        private static string _acadLibVer;
        private static bool? _isCivil = GetIsCivil();

        [NotNull]
        public static string AcadYear => HostApplicationServices.Current.releaseMarketVersion;

        public static bool IsCivil => _isCivil ?? false;

        [NotNull]
        public static string App => _app ?? (_app = IsCivil ? "Civil" : "AutoCAD");

        public static void AddStatistic()
        {
            try
            {
                var caller = new StackTrace().GetFrame(1).GetMethod();
                PluginStart(CommandStart.GetCallerCommand(caller));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStatisticsHelper.AddStatistic");
            }
        }

        public static void PluginStart(string command)
        {
            var com = new CommandStart(command, Assembly.GetCallingAssembly());
            PluginStart(com);
        }

        public static void PluginStart(CommandStart command)
        {
            if (!IsUserStatistic())
                return;
            try
            {
                var version = command.Assembly != null
                    ? FileVersionInfo.GetVersionInfo(command.Assembly.Location).ProductVersion
                    : string.Empty;
                if (command.Plugin.IsNullOrEmpty())
                    command.Plugin = command.Assembly?.GetName().Name;
                if (command.Doc.IsNullOrEmpty())
                    command.Doc = Application.DocumentManager.MdiActiveDocument?.Name;
                InsertStatistic(App, command.Plugin, command.CommandName, version, command.Doc);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStart.");
            }
        }

        public static void StartAutoCAD()
        {
            try
            {
                InsertStatistic($"{App} {AcadYear} Run", "AcadLib", $"{App} Run", Commands.AcadLibVersion.ToString(), string.Empty);

                // Статистика обновления настроек
                UpdateSettings();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "StartAutoCAD.");
            }
        }

        /// <summary>
        /// Запись статистики обновления настроек
        /// </summary>
        private static void UpdateSettings()
        {
            try
            {
                if (PikSettings.IsUpdatedSettings)
                {
                    InsertStatistic($"{App} Update", "AcadLib",
                        PikSettings.IsDisabledSettings ? "Настройки отключены" : "Настройки последние",
                        Commands.AcadLibVersion.ToString(), string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "PluginStatisticsHelper.UpdateSettings");
            }
        }

        private static bool IsUserStatistic()
        {
            return !General.IsCadManager() && !General.IsBimUser;
        }

        public static void InsertStatistic(string appName, string plugin, string command, string version, string doc)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var pg = new C_PluginStatisticTableAdapter())
                    {
                        pg.Insert(appName, plugin ?? string.Empty, command ?? string.Empty, version ?? string.Empty,
                            doc ?? string.Empty, Environment.UserName, DateTime.Now, null, Path.GetFileName(doc));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, "PluginStatisticsHelper Insert.");
                }

                if (!appName.EndsWith(" Run") && !appName.EndsWith(" Update"))
                    YandexMetrica.ReportEvent($"{plugin} {command}");
            });
        }

        private static bool GetIsCivil()
        {
            try
            {
                return CivilTest.IsCivil();
            }
            catch
            {
                return false;
            }
        }
    }
}
