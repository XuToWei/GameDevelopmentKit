set SOURCE=..\..\..\ET
set DESTINATION=..\..\..\GameDevelopmentKit

rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Runtime\Core
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Runtime\ThirdParty
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\ComponentView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\RecastNavDataExporter
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\PostProcessor
rd/s /q %DESTINATION%\DotNet
rd/s /q %DESTINATION%\Share
rd/s /q %DESTINATION%\Design
rd/s /q %DESTINATION%\Tools\Config
rd/s /q %DESTINATION%\Tools\cwRsync
rd/s /q %DESTINATION%\Tools\RecastNavExportor

del %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\Helper\ShellHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\DockDefine.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorLogHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorResHelper.cs

xcopy %SOURCE%\Unity\Assets\Scripts\ModelView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Model %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\HotfixView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Hotfix %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Core %DESTINATION%\Unity\Assets\Scripts\Library\ET\Runtime\Core /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\ThirdParty %DESTINATION%\Unity\Assets\Scripts\Library\ET\Runtime\ThirdParty /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\ComponentViewEditor %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\ComponentView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\RecastNavDataExporter %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\RecastNavDataExporter /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\AssetPostProcessor %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\PostProcessor /s /e /i
xcopy %SOURCE%\DotNet %DESTINATION%\DotNet /s /e /i
xcopy %SOURCE%\Share %DESTINATION%\Share /s /e /i
xcopy %SOURCE%\Unity\Assets\Config %DESTINATION%\Design /s /e /i
xcopy %SOURCE%\Tools\Config %DESTINATION%\Tools\Config /s /e /i
xcopy %SOURCE%\Tools\cwRsync %DESTINATION%\Tools\cwRsync /s /e /i
xcopy %SOURCE%\Tools\RecastNavExportor %DESTINATION%\Tools\RecastNavExportor /s /e /i

copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\ShellHelper.cs %DESTINATION%\Unity\Assets\Scripts\Library\ET\Editor\Helper\ShellHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\DockDefine.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\DockDefine.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\EditorLogHelper.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorLogHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\EditorResHelper.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorResHelper.cs

rd/s /q %DESTINATION%\Unity\Assets\Plugins\MongoDB

xcopy %SOURCE%\Unity\Assets\Plugins\MongoDB %DESTINATION%\Unity\Assets\Plugins\MongoDB /s /e /i

pause