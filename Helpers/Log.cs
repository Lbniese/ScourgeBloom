/*
 * This file is part of the ScourgeBloom Combat Routine.
 *
 * Copyright (C) 2016 Lbniese <Lbniese@lupra.org>
 *
 * Licensed under Microsoft Reference Source License (Ms-RSL)
 */

using System;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.Helpers;

namespace ScourgeBloom.Helpers
{
    public static class Log
    {
        public static void WriteLog(string text)
        {
            Logging.Write(text);
        }

        public static void WriteLog(string text, Color color)
        {
            Logging.Write(color, text);
        }

        public static void WriteLog(LogLevel level, string text)
        {
            if (GlobalSettings.Instance.LogLevel >= level)
                Logging.Write(text);
        }

        public static void WriteLog(LogLevel level, string text, Color color)
        {
            if (GlobalSettings.Instance.LogLevel >= level)
                Logging.Write(color, text);
        }

        public static void WriteQuiet(string text)
        {
            Logging.WriteQuiet(text);
        }

        public static void WriteQuiet(string text, Color color)
        {
            Logging.WriteQuiet(color, text);
        }

        public static void WriteQuiet(LogLevel level, string text)
        {
            if (GlobalSettings.Instance.LogLevel >= level)
                Logging.WriteQuiet(text);
        }

        public static void WriteQuite(LogLevel level, string text, Color color)
        {
            if (GlobalSettings.Instance.LogLevel >= level)
                Logging.WriteQuiet(color, text);
        }

        public static void WritetoFile(string text)
        {
            Logging.WriteToFileSync(LogLevel.Normal, text);
        }

        public static void WritetoFile(LogLevel level, string text)
        {
            if (GlobalSettings.Instance.LogLevel >= level)
                Logging.WriteToFileSync(level, text);
        }

        public static void Toast(string template, params object[] args)
        {
            var msg = string.Format(template, args);

            StyxWoW.Overlay.AddToast(() => msg,
                TimeSpan.FromSeconds(1.5),
                Colors.White,
                Colors.Black,
                new FontFamily("Segoe UI"));
        }

        public static void ToastEnabled(string template, params object[] args)
        {
            Toast(template, Colors.SeaGreen, Colors.LightSlateGray, args);
        }

        public static void ToastDisabled(string template, params object[] args)
        {
            Toast(template, Colors.Tomato, Colors.LightSlateGray, args);
        }

        public static void Toast(string template, Color color1, Color color2, params object[] args)
        {
            var msg = string.Format(template, args);

            StyxWoW.Overlay.AddToast(() => msg,
                TimeSpan.FromSeconds(1.5),
                color1,
                color2,
                new FontFamily("Segoe UI"));
        }

        public static class LogColor
        {
            public static Color Normal = Colors.Green;
            public static Color Hilite = Colors.White;
            public static Color SpellHeal = Colors.LightGreen;
            public static Color SpellNonHeal = Colors.DodgerBlue;
            public static Color Debug = Colors.Orange;
            public static Color Diagnostic = Colors.Yellow;
            public static Color Cancel = Colors.OrangeRed;
            public static Color Init = Colors.Cyan;
            public static Color Targeting = Colors.LightCoral;
            public static Color Info = Colors.LightPink;
        }
    }
}