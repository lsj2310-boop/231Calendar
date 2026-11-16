// 231 Calendar
// Modified 2026-11-16 for 231 Calendar: branding, Korean UI and separate user data.
// Copyright (C) 2026 Lee Sang Il (231 Calendar modifications)
// Copyright (C) 2026 Mahdi Kazemiesfahani
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Calendar231;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // re-running the app silently replaces nothing; the second instance just exits
        using var mutex = new Mutex(true, "231Calendar_SingleInstance", out bool createdNew);
        if (!createdNew)
            return;

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
