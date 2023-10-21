set SOURCE=..\..\..\GameDevelopmentKit
set DESTINATION=..\..\..\ET

rd/s /q %DESTINATION%\Unity\Assets\Scripts\ModelView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Model
rd/s /q %DESTINATION%\Unity\Assets\Scripts\HotfixView
rd/s /q %DESTINATION%\Unity\Assets\Scripts\Hotfix

xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\ModelView %DESTINATION%\Unity\Assets\Scripts\ModelView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\Model %DESTINATION%\Unity\Assets\Scripts\Model /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\HotfixView %DESTINATION%\Unity\Assets\Scripts\HotfixView /s /e /i
xcopy %SOURCE%\Unity\Assets\Scripts\Game\ET\Code\Hotfix %DESTINATION%\Unity\Assets\Scripts\Hotfix /s /e /i

pause