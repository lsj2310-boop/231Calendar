# 231 Calendar 1.0.1

## 다운로드

- [설치 파일 EXE](https://github.com/lsj2310-boop/231Calendar/raw/refs/heads/main/downloads/231Calendar_Setup_v1.0.1.exe)
- [설치 패키지 ZIP](https://github.com/lsj2310-boop/231Calendar/raw/refs/heads/main/downloads/231Calendar_v1.0.1_Install.zip)
- [수정 소스 ZIP](https://github.com/lsj2310-boop/231Calendar/raw/refs/heads/main/downloads/231Calendar_v1.0.1_Source.zip)

Created by Lee Sangil. Copyright (C) 2026 Lee Sang Il (231 Calendar modifications).


Google Calendar Desktop Widget v1.3.0 기반 Windows 10/11 x64 바탕화면 위젯.

## 사용 방법

1. `231Calendar_Setup_v1.0.1.exe`를 실행합니다.
2. 필수 구성 요소가 없으면 설치 과정에서 Microsoft의 .NET 8 Desktop x64 / WebView2 Runtime을 내려받습니다. 인터넷 연결과 관리자 승인이 필요할 수 있습니다.
3. 바탕화면의 **231 Calendar**를 실행하고 Google 계정에 직접 로그인합니다. 별도 API 키는 필요 없습니다.
4. 창 위치와 크기를 조정한 뒤 상단 **마우스 모양 버튼**을 누르면 위젯 모드로 전환됩니다. 위젯 모드에서는 캘린더 아래의 바탕화면으로 마우스 클릭이 통과합니다.
5. 같은 버튼을 다시 누르면 일정 추가·수정이 가능한 편집 모드로 돌아옵니다.
6. 상단 **메뉴 버튼** 또는 알림 영역 아이콘 우클릭 → **설정**에서 투명도(0~95%), 바탕화면 아이콘 뒤 배치, 좌우 그림자, 모서리 패널 위치, Windows 자동 시작을 조절합니다.
7. 어두운 테마는 Google 캘린더 자체 설정 → 모양에서 선택합니다. 캘린더 화면 테마와 관계없이 상단 바와 바깥 테두리는 검정색(#101010)을 유지합니다.
8. 종료는 알림 영역 우클릭 → 종료, 제거는 Windows 설정 → 앱 → 설치된 앱 → 231 Calendar에서 합니다. 제거해도 로그인 프로필과 설정은 보관됩니다.

## 기능과 범위

- 실제 Google 캘린더 웹 화면을 WebView2로 표시합니다. 월/주/일 보기, 일정 작성·수정, 여러 캘린더 표시는 Google 캘린더가 제공하는 기능을 사용합니다.
- 투명도, 클릭 통과, 아이콘 뒤 배치, 검정색 테두리·상단 바, 4개 모서리 패널, 화면 배율 대응, 창 위치 저장, 자동 시작, 계정 로그아웃을 원본에서 유지했습니다.
- 메뉴를 한국어로 바꾸고 이름·실행 파일·설정 폴더·자동 시작 항목을 분리했습니다.
- Outlook 계정 직접 연동 기능은 원본에 없으며 이 버전에도 포함되어 있지 않습니다.
- 인터넷 연결과 Google 계정이 필요합니다. 로그인·2단계 인증은 Google 화면에서 사용자가 직접 진행합니다.
- Google 또는 회사의 보안 정책이 내장 브라우저 로그인을 막으면 로그인할 수 없습니다. 이 버전은 로그인 정책을 우회하지 않습니다.
- 코드 서명 인증서가 적용되지 않은 빌드입니다.

## 문제 해결

- 실행되지 않음: 설치 프로그램을 다시 실행해 .NET 8 **Desktop** Runtime (x64)과 Microsoft WebView2 Runtime을 확인합니다. 일반 .NET Runtime만으로는 실행되지 않습니다.
- 로그인 오류: 인터넷 연결과 회사 계정 정책을 확인합니다. Google이 '안전하지 않은 브라우저' 오류를 표시하면 오류 내용을 전달해 주세요.
- 흐려서 안 보임: 알림 영역 → 설정에서 투명도를 낮추고 편집 모드로 전환합니다.
- 아이콘 뒤 배치가 안 됨: Windows Explorer의 바탕화면 구성에 따라 다를 수 있습니다. 설정에서 '바탕화면 아이콘 뒤에 표시'를 해제하면 일반 배경 위젯으로 사용할 수 있습니다.
- 설정 위치: `%LOCALAPPDATA%\231Calendar\settings.json`
- 설치 오류 기록: `%LOCALAPPDATA%\231Calendar\setup.log`
- 계정/쿠키 위치: `%LOCALAPPDATA%\231Calendar\WebView2Data` (공유하지 마세요.)
- 로그인 정보 삭제: 설정 → 로그아웃 → '로그아웃 및 브라우저 데이터 삭제'. 원본 CalendarWidget의 데이터는 건드리지 않습니다.

## 빌드

.NET 8 SDK와 NSIS 3.x가 필요합니다. 소스 폴더에서 PowerShell로 `./build.ps1`을 실행합니다. NuGet과 Microsoft 다운로드 서버 연결이 필요합니다.

## 제작자 표기

Created by Lee Sangil.

Copyright (C) 2026 Lee Sang Il (231 Calendar modifications).

위 표기는 231 Calendar의 수정·추가 부분에 적용됩니다. 원본 소스의 저작권과 AGPL 라이선스는 유지됩니다.

## 원본과 라이선스

Upstream: https://github.com/mkazemie/google-calendar-desktop-widget (tag v1.3.0)

원저작자: Copyright (C) 2026 Mahdi Kazemiesfahani.

AGPL-3.0-or-later. `LICENSE`와 원저작자 표기를 유지했습니다. 수정일: 2026-11-16.
이 배포본의 완전한 수정 소스를 Source.zip으로 함께 제공합니다. 재배포 시 같은 라이선스와 소스 제공 조건을 유지해야 합니다. Google의 공식 제품이 아닙니다.

첨부 원본 설치 파일 SHA-256:
`0681a17406d2eb633bd2d76359044794d610cfd485bb37d52e7677bfd6278fd5`
공식 v1.3.0 배포 파일과 일치함을 확인했습니다.

## 검증 범위

Linux에서 Windows x64 대상 Release 빌드와 설치 프로그램 컴파일을 수행했습니다. 설정 수치 및 테마 처리의 자동 검증과 배포 파일 누락 검사를 수행했습니다. Windows 실제 UI 실행, Google 계정 로그인, 동기화, 바탕화면 배치와 화면 배율별 시각 검증은 이 환경에서 수행하지 못했습니다. 실제 Windows PC에서 해당 동작을 확인해야 합니다.
