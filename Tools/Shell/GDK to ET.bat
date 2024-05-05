set SOURCE=..\..\..\GameDevelopmentKit
set DESTINATION=..\..\..\ET

rd/s /q %DESTINATION%\Unity\Assets\Scripts\ModelView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Model
rd/s /q %DESTINATION%\Unity\Assets\Scripts\HotfixView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Hotfix
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Core
rd/s /q %DESTINATION%\Unity\Assets\Scripts\ThirdParty
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Editor\ComponentViewEditor
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Editor\RecastNavDataExporter
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Editor\AssetPostProcessor
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Editor\LogRedirection
rd/s /q %DESTINATION%\DotNet
rd/s /q %DESTINATION%\Share
rd/s /q %DESTINATION%\Unity\Assets\Config
rd/s /q %DESTINATION%\Config
rd/s /q %DESTINATION%\Tools\Config
rd/s /q %DESTINATION%\Tools\cwRsync
rd/s /q %DESTINATION%\Tools\RecastNavExportor

del %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\ShellHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\DockDefine.cs
del %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\EditorLogHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\EditorResHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Loader\UnityLogger.cs
del %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll
del %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta

xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\ModelView %DESTINATION%\Unity\Assets\Scripts\ModelView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\Model %DESTINATION%\Unity\Assets\Scripts\Model /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\HotfixView %DESTINATION%\Unity\Assets\Scripts\HotfixView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\Hotfix %DESTINATION%\Unity\Assets\Scripts\Hotfix /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Library\ET\Core\Runtime %DESTINATION%\Unity\Assets\Scripts\Core /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Library\ET\ThirdParty\Runtime %DESTINATION%\Unity\Assets\Scripts\ThirdParty /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Library\ET\Core\Editor\ComponentView %DESTINATION%\Unity\Assets\Scripts\Editor\ComponentViewEditor /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Library\ET\ThirdParty\Editor\RecastNavDataExporter %DESTINATION%\Unity\Assets\Scripts\Editor\RecastNavDataExporter /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Editor\PostProcessor %DESTINATION%\Unity\Assets\Scripts\Editor\AssetPostProcessor /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Editor\LogRedirection %DESTINATION%\Unity\Assets\Scripts\Editor\LogRedirection /s /e /i
xcopy %SOURCE%\DotNet %DESTINATION%\DotNet /s /e /i
xcopy %SOURCE%\Share %DESTINATION%\Share /s /e /i
xcopy %SOURCE%\Design %DESTINATION%\Unity\Assets\Config /s /e /i
xcopy %SOURCE%\Config %DESTINATION%\Config /s /e /i
xcopy %SOURCE%\Tools\Config %DESTINATION%\Tools\Config /s /e /i
xcopy %SOURCE%\Tools\cwRsync %DESTINATION%\Tools\cwRsync /s /e /i
xcopy %SOURCE%\Tools\RecastNavExportor %DESTINATION%\Tools\RecastNavExportor /s /e /i

copy %SOURCE%\Unity\Assets\Scripts\Library\ET\Core\Editor\Helper\ShellHelper.cs %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\ShellHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Game\ET\Editor\Helper\DockDefine.cs %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\DockDefine.cs
copy %SOURCE%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorLogHelper.cs %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\EditorLogHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorResHelper.cs %DESTINATION%\Unity\Assets\Scripts\Editor\Helper\EditorResHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Game\ET\Loader\UnityLogger.cs %DESTINATION%\Unity\Assets\Scripts\Loader\UnityLogger.cs
copy %SOURCE%\Unity\Assets\Plugins\Share.SourceGenerator.dll %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll
copy %SOURCE%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta

pause