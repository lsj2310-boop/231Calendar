# Changes / 2026-11-16

Based on mkazemie/google-calendar-desktop-widget v1.3.0. Original copyright and AGPL-3.0-or-later retained.

- App name 231 Calendar, assembly 231Calendar, version 1.0.1, namespace Calendar231.
- Independent mutex, HKCU startup value and local data directory named 231Calendar.
- Korean settings, account prompts, title-bar tooltips and tray menu.
- Replaced donation button with About and source/license location.
- New NSIS per-user installer with official signed Microsoft runtime bootstrapper and matching uninstaller; source included with installation.
- Preserved upstream calendar rendering, window composition, click-through, WorkerW layering, DPI logic, gradients, theme and account profile management.

The calendar itself remains the Google Calendar website rendered by WebView2. Outlook integration has not been added.

## v1.0.1

- Added English creator and modification copyright notices for Lee Sangil / Lee Sang Il in installer, About, assembly metadata and source headers. Original author copyright retained.
- Locked outer frame, title bar, hover panel and the behind-icons strip to near-black (#101010), regardless of calendar page theme.
- Applied dark DWM border color on Windows 11; Windows 10 ignores unsupported border attributes.
