set SOURCE=..\..\..\ET
set DESTINATION=..\..\..\GameDevelopmentKit

rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Runtime
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\ThirdParty\Runtime
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Editor\ComponentView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Library\ET\ThirdParty\Editor\RecastNavDataExporter
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\PostProcessor
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\LogRedirection
rd/s /q %DESTINATION%\DotNet
rd/s /q %DESTINATION%\Share
rd/s /q %DESTINATION%\Design
rd/s /q %DESTINATION%\Tools\Config
rd/s /q %DESTINATION%\Tools\cwRsync
rd/s /q %DESTINATION%\Tools\RecastNavExportor

del %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Editor\Helper\ShellHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\DockDefine.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorLogHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorResHelper.cs
del %DESTINATION%\Unity\Assets\Scripts\Game\ET\Loader\UnityLogger.cs
del %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll
del %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta

xcopy %SOURCE%\Unity\Assets\Scripts\ModelView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\ModelView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Model %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Model /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\HotfixView %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\HotfixView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Hotfix %DESTINATION%\Unity\Assets\Scripts\Game\ET\Code\Hotfix /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Core %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Runtime /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\ThirdParty %DESTINATION%\Unity\Assets\Scripts\Library\ET\ThirdParty\Runtime /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\ComponentViewEditor %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Editor\ComponentView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\RecastNavDataExporter %DESTINATION%\Unity\Assets\Scripts\Library\ET\ThirdParty\Editor\RecastNavDataExporter /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\AssetPostProcessor %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\PostProcessor /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Editor\LogRedirection %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\LogRedirection /s /e /i
xcopy %SOURCE%\DotNet %DESTINATION%\DotNet /s /e /i
xcopy %SOURCE%\Share %DESTINATION%\Share /s /e /i
xcopy %SOURCE%\Unity\Assets\Config %DESTINATION%\Design /s /e /i
xcopy %SOURCE%\Tools\Config %DESTINATION%\Tools\Config /s /e /i
xcopy %SOURCE%\Tools\cwRsync %DESTINATION%\Tools\cwRsync /s /e /i
xcopy %SOURCE%\Tools\RecastNavExportor %DESTINATION%\Tools\RecastNavExportor /s /e /i

copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\ShellHelper.cs %DESTINATION%\Unity\Assets\Scripts\Library\ET\Core\Editor\Helper\ShellHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\DockDefine.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\DockDefine.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\EditorLogHelper.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorLogHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Editor\Helper\EditorResHelper.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Editor\Helper\EditorResHelper.cs
copy %SOURCE%\Unity\Assets\Scripts\Loader\UnityLogger.cs %DESTINATION%\Unity\Assets\Scripts\Game\ET\Loader\UnityLogger.cs
copy %SOURCE%\Unity\Assets\Plugins\Share.SourceGenerator.dll %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll
copy %SOURCE%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta %DESTINATION%\Unity\Assets\Plugins\Share.SourceGenerator.dll.meta

rd/s /q %DESTINATION%\Unity\Assets\Plugins\MongoDB

xcopy %SOURCE%\Unity\Assets\Plugins\MongoDB %DESTINATION%\Unity\Assets\Plugins\MongoDB /s /e /i

pause