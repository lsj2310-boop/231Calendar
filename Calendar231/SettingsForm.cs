// 231 Calendar
// Modified 2026-11-16 for 231 Calendar: branding, Korean UI and separate user data.
// Copyright (C) 2026 Lee Sang Il (231 Calendar modifications)
// Copyright (C) 2026 Mahdi Kazemiesfahani
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Win32;

namespace Calendar231;

/// <summary>
/// Settings window. Laid out in code at 96 DPI and scaled to the DPI of the monitor it is
/// on (<see cref="LayoutForDpi"/>): positions and font sizes scale together, and each row
/// is placed below the measured height of the one above, so nothing overlaps at any scale.
/// </summary>
public class SettingsForm : Form
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string RunValueName = "231Calendar";

    // 96-DPI design metrics
    private const int LogicalWidth = 340;  // client width
    private const int Gutter = 20;         // left/right margin

    private readonly MainForm main;

    private readonly Label lblAlpha = SectionLabel("투명도");
    private readonly Label valAlpha = ValueLabel(ContentAlignment.TopRight);
    private readonly TrackBar sldAlpha;
    private readonly CheckBox cbBehind;
    private readonly Label lblShadow = SectionLabel("가장자리 그림자");
    private readonly EdgeShadowSlider sldShadow;
    private readonly Label valShadowLeft = ValueLabel(ContentAlignment.TopLeft);
    private readonly Label valShadowRight = ValueLabel(ContentAlignment.TopRight);
    private readonly Label lblCorner = SectionLabel("모서리 제어 패널");
    private readonly CheckBox cbCorner;
    private readonly ComboBox cmbCorner;
    private readonly CheckBox cbStartup;
    private readonly Label lblTip;
    private readonly Button btnDonate;
    private readonly Button btnExit;
    private readonly Button btnAccount;

    private static readonly Color Back = Color.FromArgb(32, 33, 36);
    private static readonly Color CardBack = Color.FromArgb(48, 49, 52);
    private static readonly Color Border = Color.FromArgb(94, 99, 104);
    private static readonly Color Fore = Color.FromArgb(232, 234, 237);
    private static readonly Color Muted = Color.FromArgb(154, 160, 166);
    private static readonly Color Accent = Color.FromArgb(138, 180, 248);

    private static readonly string[] CornerIds = ["BottomRight", "BottomLeft", "TopRight", "TopLeft"];
    private static readonly string[] CornerNames = ["오른쪽 아래", "왼쪽 아래", "오른쪽 위", "왼쪽 위"];

    public SettingsForm(MainForm main, AppSettings settings)
    {
        this.main = main;
        Text = "231 Calendar";
        AutoScaleMode = AutoScaleMode.None;  // LayoutForDpi scales everything itself
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;  // centered by ShowAndActivate / OnHandleCreated
        BackColor = Back;
        ForeColor = Fore;
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { /* keep default */ }

        // ---- transparency ----
        valAlpha.Text = settings.TransparencyPercent + "%";
        sldAlpha = new TrackBar
        {
            Minimum = 0,
            Maximum = 95,
            Value = Math.Clamp(settings.TransparencyPercent, 0, 95),
            TickStyle = TickStyle.None,
            BackColor = Back,
        };
        sldAlpha.ValueChanged += (_, _) =>
        {
            valAlpha.Text = sldAlpha.Value + "%";
            main.SetTransparencyPercent(sldAlpha.Value);
        };

        // ---- live-wallpaper mode ----
        cbBehind = Check("바탕화면 아이콘 뒤에 표시", settings.BehindDesktopIcons);
        cbBehind.CheckedChanged += (_, _) => main.SetBehindDesktopIcons(cbBehind.Checked);

        // ---- edge shadows: dark fades behind desktop icons ----
        sldShadow = new EdgeShadowSlider
        {
            BackColor = Back,
            LeftPercent = settings.EdgeShadowLeftPercent,
            RightPercent = settings.EdgeShadowRightPercent,
        };
        void ShowShadowValues()
        {
            valShadowLeft.Text = SideValue("왼쪽", sldShadow.LeftPercent);
            valShadowRight.Text = SideValue("오른쪽", sldShadow.RightPercent);
        }
        ShowShadowValues();
        sldShadow.ValuesChanged += (_, _) =>
        {
            ShowShadowValues();
            main.SetEdgeShadows(sldShadow.LeftPercent, sldShadow.RightPercent);
        };

        // ---- corner hover panel ----
        cbCorner = Check("화면 모서리에 제어 패널 표시", settings.CornerPanelEnabled);
        cmbCorner = new ComboBox
        {
            Enabled = settings.CornerPanelEnabled,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            BackColor = CardBack,
            ForeColor = Fore,
        };
        cmbCorner.Items.AddRange(CornerNames);
        cmbCorner.SelectedIndex = Math.Max(0, Array.IndexOf(CornerIds, settings.PanelCorner));
        cmbCorner.SelectedIndexChanged += (_, _) => main.SetPanelCorner(CornerIds[cmbCorner.SelectedIndex]);
        cbCorner.CheckedChanged += (_, _) =>
        {
            cmbCorner.Enabled = cbCorner.Checked;
            main.SetCornerPanelEnabled(cbCorner.Checked);
        };

        // ---- startup ----
        cbStartup = Check("Windows 시작 시 자동 실행", IsStartupEnabled());
        cbStartup.CheckedChanged += (_, _) => SetStartup(cbStartup.Checked);

        // ---- tip ----
        lblTip = new Label
        {
            Text = "어두운 테마는 Google 캘린더 설정(톱니바퀴)에서 변경할 수 있습니다.",
            ForeColor = Muted,
        };

        // ---- support the project ----
        btnDonate = ActionButton("231 Calendar  ·  프로그램 정보", Accent);
        btnDonate.Click += (_, _) => MessageBox.Show(this,
            "231 Calendar 1.0.1\n\nCreated by Lee Sangil.\nCopyright (C) 2026 Lee Sang Il (231 Calendar modifications).\n\nGoogle Calendar Desktop Widget 1.3.0 기반\n" +
            "원저작자: Mahdi Kazemiesfahani (2026)\n\n" +
            "AGPL-3.0-or-later / 수정 소스: 설치 폴더의 Source.zip\n" +
            "라이선스: 설치 폴더의 LICENSE\n\n" +
            "Google의 공식 제품이 아닙니다.",
            "231 Calendar", MessageBoxButtons.OK, MessageBoxIcon.Information);

        btnExit = ActionButton("종료", Fore);
        btnExit.Click += (_, _) => Application.Exit();

        btnAccount = ActionButton("로그인", Fore);  // refreshed from the real cookie state whenever the form is shown
        btnAccount.Click += OnAccountClick;

        Controls.AddRange([lblAlpha, valAlpha, sldAlpha, cbBehind, lblShadow, sldShadow, valShadowLeft, valShadowRight,
            lblCorner, cbCorner, cmbCorner, cbStartup, lblTip, btnDonate, btnExit, btnAccount]);

        LayoutForDpi(DeviceDpi);  // provisional (system DPI); redone once the window's monitor is known
    }

    /// <summary>
    /// Lay the window out for <paramref name="dpi"/>. Every size is a 96-DPI value scaled by
    /// dpi/96 and fonts are sized in pixels for the same DPI, so text and boxes always scale
    /// together. Rows advance by measured heights rather than fixed offsets.
    /// </summary>
    private void LayoutForDpi(int dpi)
    {
        int S(int v) => UiScale.Px(v, dpi);

        var body = UiScale.Font(10f, dpi);
        var small = UiScale.Font(9f, dpi);
        var section = UiScale.Font(8.5f, dpi, FontStyle.Bold);

        SuspendLayout();
        // every control gets its font explicitly: on a DPI change WinForms hands children
        // LOCAL fonts of its own, after which they would no longer follow the form's font
        Font = body;
        foreach (Control c in Controls)
            c.Font = body;
        lblAlpha.Font = valAlpha.Font = lblShadow.Font = lblCorner.Font = section;
        valShadowLeft.Font = valShadowRight.Font = lblTip.Font = small;

        int x = S(Gutter);
        int w = S(LogicalWidth - 2 * Gutter);
        int half = w / 2;

        // place a control at its preferred height for the given width; returns the next free y
        int Row(Control c, int left, int top, int width, int gapAfter)
        {
            int h = c.GetPreferredSize(new Size(width, 0)).Height;
            c.SetBounds(left, top, width, h);
            return top + h + S(gapAfter);
        }

        int y = S(16);
        Row(valAlpha, x + half, y, half, 0);  // same line as its section label, right-aligned
        y = Row(lblAlpha, x, y, half, 4);
        // the native trackbar insets its thumb travel; widen it so the track lines up with the text
        sldAlpha.SetBounds(x - S(6), y, w + S(12), 0);  // height: the trackbar's own (AutoSize)
        y += sldAlpha.Height + S(8);
        y = Row(cbBehind, x, y, w, 20);

        y = Row(lblShadow, x, y, w, 4);
        int shadowPad = S(EdgeShadowSlider.LogicalHeight / 2);  // the slider pads its track ends by its half height
        sldShadow.SetBounds(x - shadowPad, y, w + 2 * shadowPad, S(EdgeShadowSlider.LogicalHeight));
        y += sldShadow.Height + S(2);
        Row(valShadowRight, x + half, y, half, 0);
        y = Row(valShadowLeft, x, y, half, 20);

        y = Row(lblCorner, x, y, w, 6);
        y = Row(cbCorner, x, y, w, 8);
        cmbCorner.SetBounds(x, y, w, cmbCorner.PreferredHeight);
        y += cmbCorner.PreferredHeight + S(16);

        y = Row(cbStartup, x, y, w, 12);
        y = Row(lblTip, x, y, w, 12);  // wraps to as many lines as the text needs

        btnDonate.SetBounds(x, y, w, S(36));
        y += S(36 + 12);
        int bw = (w - S(20)) / 2;
        btnExit.SetBounds(x, y, bw, S(36));
        btnAccount.SetBounds(x + w - bw, y, bw, S(36));
        y += S(36 + 16);

        ClientSize = new Size(S(LogicalWidth), y);
        ResumeLayout();
    }

    /// <summary>Show — the first time centered on the monitor under the mouse — and bring to front.</summary>
    public void ShowAndActivate()
    {
        if (!IsHandleCreated)
            CenterIn(Screen.FromPoint(Cursor.Position));  // so the window is created at that monitor's DPI
        Show();
        Activate();
    }

    private void CenterIn(Screen screen)
    {
        var wa = screen.WorkingArea;
        Location = new Point(wa.Left + Math.Max(0, (wa.Width - Width) / 2), wa.Top + Math.Max(0, (wa.Height - Height) / 2));
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        NativeMethods.ApplyDarkTitleBar(Handle);
        // the handle carries the real monitor DPI: final layout, re-centered for the new size
        LayoutForDpi(DeviceDpi);
        CenterIn(Screen.FromHandle(Handle));
    }

    // dragged onto a monitor with another scale. Cancelled so WinForms doesn't also rescale
    // controls and fonts; we lay out from scratch and take only the suggested position.
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        e.Cancel = true;
        base.OnDpiChanged(e);
        LayoutForDpi(e.DeviceDpiNew);
        Location = e.SuggestedRectangle.Location;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible)
            RefreshAccountButton();
        else
            main.EndPreview();  // interactive mode returns to fully opaque once Settings closes
    }

    private async void RefreshAccountButton()
    {
        try
        {
            btnAccount.Text = await main.IsSignedInAsync() ? "로그아웃" : "로그인";
        }
        catch
        {
            btnAccount.Text = "로그인";  // webview not ready yet; signing in is the sane default
        }
    }

    private async void OnAccountClick(object? sender, EventArgs e)
    {
        if (!await main.IsSignedInAsync())
        {
            main.BeginSignIn();
            Hide();  // get out of the way of the sign-in page
            return;
        }

        var keepBtn = new TaskDialogButton("로그아웃 (계정 정보 유지)");
        var wipeBtn = new TaskDialogButton("로그아웃 및 브라우저 데이터 삭제");
        var page = new TaskDialogPage
        {
            Caption = "231 Calendar",
            Heading = "Google 계정에서 로그아웃하시겠습니까?",
            Text = "계정 유지: 계정 선택 정보가 남아 다음 로그인 시 편리합니다.\n\n"
                 + "데이터 삭제: 계정, 쿠키, 캐시 등 이 프로그램의 브라우저 프로필을 삭제합니다.",
            Icon = TaskDialogIcon.ShieldBlueBar,
            Buttons = { keepBtn, wipeBtn, TaskDialogButton.Cancel },
            AllowCancel = true,
        };

        var result = TaskDialog.ShowDialog(this, page);
        if (result == keepBtn)
            await main.SignOutKeepAccountAsync();
        else if (result == wipeBtn)
            await main.SignOutAsync();
        else
            return;
        RefreshAccountButton();
    }

    // closing the settings window only hides it; the widget keeps running
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnFormClosing(e);
    }

    private static Label SectionLabel(string text) => new() { Text = text, ForeColor = Muted };

    // live value next to a section label, or under one end of the edge-shadow slider
    private static Label ValueLabel(ContentAlignment align) => new() { ForeColor = Accent, TextAlign = align };

    private static CheckBox Check(string text, bool isChecked) => new() { Text = text, ForeColor = Fore, Checked = isChecked };

    private static Button ActionButton(string text, Color fore)
    {
        var b = new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            BackColor = CardBack,
            ForeColor = fore,
        };
        b.FlatAppearance.BorderColor = Border;
        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 64, 67);
        return b;
    }

    private static string SideValue(string side, int percent) => percent == 0 ? side + " 없음" : $"{side} {percent}%";

    private static bool IsStartupEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey);
        return key?.GetValue(RunValueName) is not null;
    }

    private static void SetStartup(bool enable)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKey);
        if (enable)
            key.SetValue(RunValueName, $"\"{Application.ExecutablePath}\"");
        else
            key.DeleteValue(RunValueName, throwOnMissingValue: false);
    }
}
